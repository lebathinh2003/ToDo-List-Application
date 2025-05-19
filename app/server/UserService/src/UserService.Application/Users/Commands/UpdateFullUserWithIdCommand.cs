using Contract.Common;
using Contract.Constants;
using Contract.Interfaces;
using IdentityProto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;
namespace UserService.Application.Users.Commands;
public record UpdateFullUserWithIdCommand : IRequest<Result<bool?>>
{
    public Guid Id { get; set; }
    public string? Address { get; set; } 
    public string? Fullname { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }

}
public class UpdateFullUserWithIdCommandHandler : IRequestHandler<UpdateFullUserWithIdCommand, Result<bool?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcIdentity.GrpcIdentityClient _grpcIdentityClient;
    private readonly ISignalRService _signalRService;

    public UpdateFullUserWithIdCommandHandler(IApplicationDbContext context, GrpcIdentity.GrpcIdentityClient grpcIdentityClient, ISignalRService signalRService)
    {
        _context = context;
        _grpcIdentityClient = grpcIdentityClient;
        _signalRService = signalRService;
    }

    public async Task<Result<bool?>> Handle(UpdateFullUserWithIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if(request == null || request.Id == Guid.Empty)
            {
                return Result<bool?>.Failure(UserError.NullParameters, "Id is null.");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
            {
                return Result<bool?>.Failure(UserError.NotFound, "User not found.");
            }

            if (!string.IsNullOrEmpty(request.Address))
            {
                user.Address = request.Address;
            }

            if (!string.IsNullOrEmpty(request.Fullname))
            {
                user.FullName = request.Fullname;
            }

            if (request.IsActive.HasValue || !string.IsNullOrEmpty(request.Email) || !string.IsNullOrEmpty(request.Password) || !string.IsNullOrEmpty(request.Username))
            {
                var response = await _grpcIdentityClient.UpdateAccountAsync(new GrpcUpdateAccountRequest
                {
                    Id = request.Id.ToString(),
                    Username = request.Username ?? "",
                    Email = request.Email ?? "",
                    Password = request.Password ?? "",
                    IsActive = request.IsActive?.ToString().ToLower() ?? "",
                }, cancellationToken: cancellationToken);

                if (response == null || response.IsSuccess == false)
                {
                    return Result<bool?>.Failure(UserError.UpdateUserFail, "Update account information fail.");
                }

                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }
            }

            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.Instance.SaveChangesAsync(cancellationToken);

            if(user.IsActive == false)
            {
                await _signalRService.InvokeAction(SignalRAction.TriggerLogout.ToString(), user.Id);
            }

            return Result<bool?>.Success(true);

        }
        catch (Exception ex)
        {
            return Result<bool?>.Failure(UserError.AddUserFail, ex.Message);
        }
    }
}
