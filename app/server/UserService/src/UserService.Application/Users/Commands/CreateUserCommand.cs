using Contract.Common;
using IdentityProto;
using MediatR;
using UserService.Application.DTOs;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
namespace UserService.Application.Users.Commands;
public record CreateUserCommand : IRequest<Result<UserDetailDTO?>>
{
    public string Address { get; set; } = null!;
    public string Fullname { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; } = true;


}
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDetailDTO?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcIdentity.GrpcIdentityClient _grpcIdentityClient;

    public CreateUserCommandHandler(IApplicationDbContext context, GrpcIdentity.GrpcIdentityClient grpcIdentityClient)
    {
        _context = context;
        _grpcIdentityClient = grpcIdentityClient;
    }

    public async Task<Result<UserDetailDTO?>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
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
                IsActive = request.IsActive,
            };

            var response = await _grpcIdentityClient.CreateAccountAsync(new GrpcCreateAccountRequest
            {
                Id = id.ToString(),
                Email = request.Email,
                Password = request.Password,
                Username = request.Username,
                IsActive = request.IsActive,
            }, cancellationToken: cancellationToken);

            if (response == null)
            {
                return Result<UserDetailDTO?>.Failure(UserError.AddUserFail, "Create user in identity service fail");
            }

            _context.Users.Add(user);
            await _context.Instance.SaveChangesAsync(cancellationToken);

            return Result<UserDetailDTO?>.Success(new UserDetailDTO
            {
                Id = user.Id,
                Address = user.Address,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                Role = response.Role,
                Email = request.Email,
                Username = request.Username,
            });
        }
        catch (Exception ex)
        {
            return Result<UserDetailDTO?>.Failure(UserError.AddUserFail, ex.Message);
        }
        
    }
}
