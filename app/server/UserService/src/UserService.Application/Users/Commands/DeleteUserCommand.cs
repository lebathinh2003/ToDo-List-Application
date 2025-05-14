using Contract.Common;
using Contract.Event.UserEvent;
using Contract.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
namespace UserService.Application.Users.Commands;
public record DeleteUserCommand : IRequest<Result<User?>>
{
    public Guid UserId { get; set; }

}
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<User?>>
{
    private readonly IApplicationDbContext _context;
    private readonly IServiceBus _serviceBus;

    public DeleteUserCommandHandler(IApplicationDbContext context, IServiceBus serviceBus)
    {
        _context = context;
        _serviceBus = serviceBus;
    }

    public async Task<Result<User?>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return Result<User?>.Failure(UserError.NotFound);
            }

            if (user.IsAdmin)
            {
                return Result<User?>.Failure(UserError.PermissionDenied);
            }

            if (user.IsActive == false)
            {
                return Result<User?>.Failure(UserError.UserAlreadyInactive);
            }

            user.IsActive = false;

            _context.Users.Update(user);

            await _context.Instance.SaveChangesAsync(cancellationToken);

            await _serviceBus.Publish(new DeleteUserEvent
            {
                UserId = user.Id
            });

            return Result<User?>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User?>.Failure(UserError.AddUserFail, ex.Message);
        }
        
    }
}
