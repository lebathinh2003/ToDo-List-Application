using Contract.Common;
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

    public RestoreUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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

            user.IsActive = true;

            _context.Users.Update(user);

            await _context.Instance.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(UserError.AddUserFail, ex.Message);
        }
        
    }
}
