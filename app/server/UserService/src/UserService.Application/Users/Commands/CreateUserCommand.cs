using Contract.Common;
using MediatR;
using UserService.Application.DTOs;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
namespace UserService.Application.Users.Commands;
public record CreateUserCommand : IRequest<Result<UserDTO?>>
{
    public string Address { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

}
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDTO?>>
{
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDTO?>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                FullName = request.Fullname,
                Address = request.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            await _context.Users.AddAsync(user);

            return Result<UserDTO?>.Success(new UserDTO
            {
                Id = user.Id,
                Address = user.Address,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
            });
        }
        catch (Exception ex)
        {
            return Result<UserDTO?>.Failure(UserError.AddUserFail, ex.Message);
        }
        
    }
}
