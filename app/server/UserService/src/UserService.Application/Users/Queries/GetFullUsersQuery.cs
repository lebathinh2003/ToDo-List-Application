using AutoMapper;
using Contract.Common;
using Contract.DTOs;
using Contract.Interfaces;
using Google.Protobuf.Collections;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using UserService.Application.DTOs;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
using IdentityProto;

namespace UserService.Application.Users.Queries;

public class GetFullUserQuery : IRequest<Result<PaginatedGetFullUserDTO?>>
{
    [Required]
    public PaginatedDTO PaginatedDTO { get; init; } = null!;
}

public class GetFullUserQueryHandler : IRequestHandler<GetFullUserQuery, Result<PaginatedGetFullUserDTO?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcIdentity.GrpcIdentityClient _grpcIdentityClient;
    private readonly IPaginateDataUtility<UserDetailDTO, NumberedPaginatedMetadata> _paginateDataUtility;
    private readonly IMapper _mapper;

    public GetFullUserQueryHandler(IApplicationDbContext context, GrpcIdentity.GrpcIdentityClient grpcIdentityClient, IPaginateDataUtility<UserDetailDTO, NumberedPaginatedMetadata> paginateDataUtility, IMapper mapper)
    {
        _context = context;
        _grpcIdentityClient = grpcIdentityClient;
        _paginateDataUtility = paginateDataUtility;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedGetFullUserDTO?>> Handle(GetFullUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paginatedDTO = request.PaginatedDTO;

            if (paginatedDTO.Skip == null)
            {
                return Result<PaginatedGetFullUserDTO?>.Failure(UserError.NullParameters, "Skip is null");
            }

            var userQuery = _context.Users.Where(u => !u.IsAdmin).AsQueryable();

            if(paginatedDTO.IncludeInactive != null && paginatedDTO.IncludeInactive.Value == false)
            {
                userQuery = userQuery.Where(u => u.IsActive).AsQueryable();
            }

            if (!string.IsNullOrEmpty(paginatedDTO.Keyword))
            {
                var keyword = paginatedDTO.Keyword.ToLower();

                var searchAccountResponse = await _grpcIdentityClient.SearchSimpleAccountAsync(new GrpcKeywordRequest
                {
                    Keyword = keyword,
                }, cancellationToken: cancellationToken);

                var searchAccountIds = searchAccountResponse.Ids.ToHashSet();

                userQuery = userQuery.Where(u => u.FullName.ToLower().Contains(keyword) ||
                                                 u.Address.ToLower().Contains(keyword) ||
                                                 searchAccountIds.Contains(u.Id.ToString())
                );
            }

            var userList = await userQuery.Select(t => new UserDetailDTO
            {
                Id = t.Id,
                Address = t.Address,
                FullName = t.FullName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                IsActive = t.IsActive,
                Role = t.IsAdmin ? "Admin" : "Staff",
                Email = "",
                Username = "",
            }).ToListAsync();

            if (userList == null || !userList.Any())
            {
                return Result<PaginatedGetFullUserDTO?>.Success(new PaginatedGetFullUserDTO
                {
                    PaginatedData = [],
                    Metadata = new NumberedPaginatedMetadata
                    {
                        CurrentPage = paginatedDTO.Skip!.Value,
                        TotalPage = 0,
                        TotalRow = 0,
                    }
                });
            }

            var userIds = userQuery
            .Select(t => t.Id)
            .Distinct()
            .ToHashSet();

            var response = await _grpcIdentityClient.GetSimpleAccountsAsync(new GrpcIdsSetRequest
            {
                Ids = { _mapper.Map<RepeatedField<string>>(userIds) }
            }, cancellationToken: cancellationToken);

            if (response == null || response.Accounts.Count != userIds.Count)
            {
                return Result<PaginatedGetFullUserDTO>.Failure(UserError.NotFound, "Get account grpc fail.");
            }

            foreach (var t in userList)
            {
                t.Username = response.Accounts[t.Id.ToString()].Username;
                t.Email = response.Accounts[t.Id.ToString()].Email;
            }

            var userResponseQuery = userList.AsQueryable();

            var limit = 10;
            if (paginatedDTO.Limit != null)
            {
                limit = paginatedDTO.Limit.Value;
            }

            var totalRow = userResponseQuery.Count();
            var totalPage = (totalRow + limit - 1) / limit;

            userResponseQuery = _paginateDataUtility.PaginateQuery(userResponseQuery, new PaginateParam
            {
                Offset = (paginatedDTO.Skip ?? 0) * limit,
                Limit = limit,
                SortBy = paginatedDTO.SortBy != null ? paginatedDTO.SortBy : "CreatedAt",
                SortOrder = paginatedDTO.SortOrder
            });

            userList = userResponseQuery.ToList();

            var paginatedResponse = new PaginatedGetFullUserDTO
            {
                Metadata = new NumberedPaginatedMetadata
                {
                    CurrentPage = (paginatedDTO.Skip ?? 0) + 1,
                    TotalPage = totalPage,
                    TotalRow = totalRow,
                },
                PaginatedData = userList,
            };

            return Result<PaginatedGetFullUserDTO?>.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return Result<PaginatedGetFullUserDTO?>.Failure(UserError.GetUserFail, ex.Message);
        }

    }
}
