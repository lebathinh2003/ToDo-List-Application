using Contract.Common;
using IndentityService.Domain.Errors;
using IndentityService.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.ApplicationUsers.Commands;

public record UpdateAccountCommand : IRequest<Result>
{
    [Required]
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? IsActive { get; init; }
}
public class GetAccountDetailQueryHandler : IRequestHandler<UpdateAccountCommand, Result>
{
    private UserManager<ApplicationUser> _userManager;

    public GetAccountDetailQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        try {
            var id = request.Id;
            if (id == Guid.Empty)
            {
                return Result.Failure(AccountError.NullParameters, "Id is null or empty.");
            }

            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.IsActive))
            {
                return Result.Failure(AccountError.NullParameters, "Email and IsActive are null or empty.");
            }

            if (!string.IsNullOrEmpty(request.IsActive) && (request.IsActive != "true" && request.IsActive != "false"))
            {
                return Result.Failure(AccountError.InvalidParameters, "IsActive must be 'true' or 'false'.");
            }

            var account = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == request.Id);

            if (account == null)
            {
                return Result.Failure(AccountError.NotFound, "Account not found.");
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Id != account.Id)
                {
                    return Result.Failure(AccountError.EmailAlreadyExists, "Email already exists.");
                }
                account.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Username))
            {
                var existingUser = await _userManager.FindByNameAsync(request.Username);
                if (existingUser != null && existingUser.Id != account.Id)
                {
                    return Result.Failure(AccountError.UsernameAlreadyExists, "Username already exists.");
                }
                account.UserName = request.Username;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(account);
                var result = await _userManager.ResetPasswordAsync(account, token, request.Password);
            }

            if (!string.IsNullOrEmpty(request.IsActive))
            {
                account.IsActive = request.IsActive == "true";
            }

            await _userManager.UpdateAsync(account);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(AccountError.UpdateAccountFail, ex.Message);
        }
    }
}

