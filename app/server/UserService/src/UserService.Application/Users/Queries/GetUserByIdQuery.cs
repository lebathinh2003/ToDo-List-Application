using Contract.Common;
using IdentityProto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.DTOs;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
namespace UserService.Application.Users.Queries;
public record GetUserByIdQuery : IRequest<Result<UserDetailDTO?>>
{
    public Guid Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDTO?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcIdentity.GrpcIdentityClient _grpcIdentityClient;

    public GetUserByIdQueryHandler(IApplicationDbContext context, GrpcIdentity.GrpcIdentityClient grpcIdentityClient)
    {
        _context = context;
        _grpcIdentityClient = grpcIdentityClient;
    }

    public async Task<Result<UserDetailDTO?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.Id);

        if (user == null) {

            return Result<UserDetailDTO?>.Failure(UserError.NotFound, $"Not found user:{request.Id}");
        }

        var userResponse = await _grpcIdentityClient.GetAccountDetailAsync(new GrpcIdRequest
        {
            Id = user.Id.ToString(),
        }, cancellationToken: cancellationToken);

        if (userResponse == null)
        {
            return Result<UserDetailDTO?>.Failure(UserError.NotFound, $"Not found user's account:{request.Id}");
        }

        return Result<UserDetailDTO?>.Success(new UserDetailDTO
        {
            Id = user.Id,
            Username = userResponse.Username,
            Email = userResponse.Email,
            Role = userResponse.Role,
            FullName = user.FullName,
            Address = user.Address,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive,
        });
    }
}

