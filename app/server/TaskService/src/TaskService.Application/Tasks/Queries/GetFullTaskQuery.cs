using AutoMapper;
using Contract.Common;
using Contract.DTOs;
using Contract.Interfaces;
using Google.Protobuf.Collections;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskService.Application.DTOs;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
using UserProto;
using TaskStatus = TaskService.Domain.Models.TaskStatus;

namespace TaskService.Application.Tasks.Queries;

public class GetFullTaskQuery : IRequest<Result<PaginatedGetFullTaskDTO?>>
{
    [Required]
    public PaginatedDTO PaginatedDTO { get; init; } = null!;
    public TaskStatus? Status { get; set; } = null!;

    public Guid? AssigneeId { get; set; } = null!;
}

public class GetFullTaskQueryHandler : IRequestHandler<GetFullTaskQuery, Result<PaginatedGetFullTaskDTO?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcUser.GrpcUserClient _grpcUserClient;
    private readonly IPaginateDataUtility<TaskDTO, NumberedPaginatedMetadata> _paginateDataUtility;
    private readonly IMapper _mapper;

    public GetFullTaskQueryHandler(IApplicationDbContext context, GrpcUser.GrpcUserClient grpcUserClient, IPaginateDataUtility<TaskDTO, NumberedPaginatedMetadata> paginateDataUtility, IMapper mapper)
    {
        _context = context;
        _grpcUserClient = grpcUserClient;
        _paginateDataUtility = paginateDataUtility;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedGetFullTaskDTO?>> Handle(GetFullTaskQuery request, CancellationToken cancellationToken)
    {
        try
        {


            var paginatedDTO = request.PaginatedDTO;

            if (paginatedDTO.Skip == null)
            {
                return Result<PaginatedGetFullTaskDTO?>.Failure(TaskError.NullParameters, "Skip is null");
            }

            var taskQuery = _context.Tasks.AsQueryable();

            if (request.AssigneeId != null && request.AssigneeId != Guid.Empty)
            {
                taskQuery = taskQuery.Where(t => t.AssigneeId == request.AssigneeId && t.IsActive == true);
            }


            if(request.Status != null)
            {
                taskQuery = taskQuery.Where(t => t.Status == request.Status);
            }

            if (!string.IsNullOrEmpty(paginatedDTO.Keyword))
            {
                var keyword = paginatedDTO.Keyword.ToLower();

                var searchAccountResponse = await _grpcUserClient.SearchSimpleUserAsync(new GrpcKeywordRequest
                {
                    Keyword = keyword,
                }, cancellationToken: cancellationToken);

                var searchAccoountIds = searchAccountResponse.Ids.ToHashSet();

                var searchUserResponse = await _grpcUserClient.SearchSimpleUserAsync(new GrpcKeywordRequest
                {
                    Keyword = keyword,
                }, cancellationToken: cancellationToken);

                var searchUserIds = searchUserResponse.Ids.ToHashSet();

                taskQuery = taskQuery.Where(t => t.Title.ToLower().Contains(keyword) ||
                                                 t.Description.ToLower().Contains(keyword) ||
                                                 t.Code.ToLower().Contains(keyword) ||
                                                 searchAccoountIds.Contains(t.AssigneeId.ToString()) ||
                                                 searchUserIds.Contains(t.AssigneeId.ToString())
                );
            }

            var taskList = await taskQuery.Select(t => new TaskDTO
            {
                Id = t.Id,
                Code = t.Code,
                Title = t.Title,
                Description = t.Description,
                AssigneeId = t.AssigneeId,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                IsActive = t.IsActive,
                AssigneeName = "",
                AssigneeUsername = "",
            }).ToListAsync();

            if (taskList == null || !taskList.Any())
            {
                return Result<PaginatedGetFullTaskDTO?>.Success(new PaginatedGetFullTaskDTO
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

            var userIds = taskQuery
            .Select(t => t.AssigneeId)
            .Distinct()
            .ToHashSet();

            var response = await _grpcUserClient.GetSimpleUserAsync(new GrpcIdsSetRequest
            {
                Ids = { _mapper.Map<RepeatedField<string>>(userIds) }
            }, cancellationToken: cancellationToken);

            if (response == null || response.Users.Count != userIds.Count)
            {
                return Result<PaginatedGetFullTaskDTO>.Failure(TaskError.NotFound, "Get user grpc fail.");
            }

            foreach (var t in taskList)
            {
                t.AssigneeName = response.Users[t.AssigneeId.ToString()].FullName;
                t.AssigneeUsername = response.Users[t.AssigneeId.ToString()].Address;
            }
            var taskResponseQuery = taskList.AsQueryable();

            var limit = 10;
            if (paginatedDTO.Limit != null)
            {
                limit = paginatedDTO.Limit.Value;
            }

            var totalRow = taskResponseQuery.Count();
            var totalPage = (totalRow + limit - 1) / limit;

            taskResponseQuery = _paginateDataUtility.PaginateQuery(taskResponseQuery, new PaginateParam
            {
                Offset = (paginatedDTO.Skip ?? 0) * limit,
                Limit = limit,
                SortBy = paginatedDTO.SortBy != null ? paginatedDTO.SortBy : "CreatedAt",
                SortOrder = paginatedDTO.SortOrder
            });

            taskList = taskResponseQuery.ToList();

            var paginatedResponse = new PaginatedGetFullTaskDTO
            {
                Metadata = new NumberedPaginatedMetadata
                {
                    CurrentPage = (paginatedDTO.Skip ?? 0) + 1,
                    TotalPage = totalPage,
                    TotalRow = totalRow,
                },
                PaginatedData = taskList,
            };

            return Result<PaginatedGetFullTaskDTO?>.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return Result<PaginatedGetFullTaskDTO?>.Failure(TaskError.GetTaskFail, ex.Message);
        }

    }
}
