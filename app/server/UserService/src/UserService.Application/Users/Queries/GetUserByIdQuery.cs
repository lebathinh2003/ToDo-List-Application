using Contract.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.DTOs;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
namespace UserService.Application.Users.Queries;
public record GetUserByIdQuery : IRequest<Result<UserDTO?>>
{
    public Guid Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDTO?>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDTO?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.Id);

        if (user == null) {

            return Result<UserDTO?>.Failure(UserError.NotFound, $"Not found user:{request.Id}");
        }

        return Result<UserDTO?>.Success(new UserDTO
        {
            Id = user.Id,
            FullName = user.FullName,
            Address = user.Address,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive,
        });
    }
}

