using Contract.Common;
using IdentityProto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
namespace UserService.Application.Users.Commands;
public record RestoreUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }

}
public class RestoreUserCommandHandler : IRequestHandler<RestoreUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcIdentity.GrpcIdentityClient _grpcIdentityClient;

    public RestoreUserCommandHandler(IApplicationDbContext context, GrpcIdentity.GrpcIdentityClient grpcIdentityClient)
    {
        _context = context;
        _grpcIdentityClient = grpcIdentityClient;
    }

    public async Task<Result> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return Result.Failure(UserError.NotFound);
            }

            if (user.IsAdmin)
            {
                return Result.Failure(UserError.PermissionDenied);
            }

            if (user.IsActive == true)
            {
                return Result.Failure(UserError.UserAlreadyActive);
            }

            var response = await _grpcIdentityClient.UpdateAccountAsync(new GrpcUpdateAccountRequest
            {
                Id = user.Id.ToString(),
                Email = "",
                Password = "",
                Username = "",
                IsActive = true.ToString(),

            }, cancellationToken: cancellationToken);

            if (response == null || response.IsSuccess == false)
            {
                return Result.Failure(UserError.UpdateUserFail, "Update user isactive in identity service fail");
            }

            user.IsActive = true;

            _context.Users.Update(user);

            await _context.Instance.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(UserError.RestoreUserFail, ex.Message);
        }
        
    }
}
