using Contract.Common;
using Contract.Event.UserEvent;
using Contract.Interfaces;
using IdentityProto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
using static IdentityProto.GrpcIdentity;
namespace UserService.Application.Users.Commands;
public record DeleteUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }

}
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IServiceBus _serviceBus;
    private readonly GrpcIdentity.GrpcIdentityClient _grpcIdentityClient;

    public DeleteUserCommandHandler(IApplicationDbContext context, IServiceBus serviceBus, GrpcIdentityClient grpcIdentityClient)
    {
        _context = context;
        _serviceBus = serviceBus;
        _grpcIdentityClient = grpcIdentityClient;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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

            if (user.IsActive == false)
            {
                return Result.Failure(UserError.UserAlreadyInactive);
            }

            var response = await _grpcIdentityClient.UpdateAccountAsync(new GrpcUpdateAccountRequest
            {
                Id = user.Id.ToString(),
                Email = "",
                Password = "",
                Username = "",
                IsActive = false.ToString(),

            }, cancellationToken: cancellationToken);

            if (response == null || response.IsSuccess == false)
            {
                return Result.Failure(UserError.UpdateUserFail, "Update user isactive in identity service fail");
            }

            user.IsActive = false;

            _context.Users.Update(user);

            await _context.Instance.SaveChangesAsync(cancellationToken);

            await _serviceBus.Publish(new DeleteUserEvent
            {
                UserId = user.Id
            });

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(UserError.DeleteUserFail, ex.Message);
        }
        
    }
}
