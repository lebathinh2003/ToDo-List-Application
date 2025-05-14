using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.DTOs;
using UserService.Domain.Interfaces;
namespace UserService.Application.Users.Queries;
public record GetUserByIdQuery : IRequest<UserDTO?>
{
    public Guid Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDTO?>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDTO?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.Id);

        if (user == null) {
            return null;
        }

        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive,
        };
    }
}

