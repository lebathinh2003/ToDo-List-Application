using Contract.Common;
using IdentityService.Application.DTOs;
using IndentityService.Domain.Errors;
using IndentityService.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.AccountDTOs.Commands;

public record CreateStaffAccountCommand : IRequest<Result<AccountDTO?>>
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool IsActive { get; init; }
}
public class CreateStaffAccountCommandHandler : IRequestHandler<CreateStaffAccountCommand, Result<AccountDTO?>>
{
    private UserManager<ApplicationUser> _userManager;

    public CreateStaffAccountCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<AccountDTO?>> Handle(CreateStaffAccountCommand request, CancellationToken cancellationToken)
    {
        try {
            var account = new ApplicationUser
            {
                Id = request.Id,
                Email = request.Email,
                UserName = request.Username,
                IsActive = request.IsActive,
            };

            var result = _userManager.CreateAsync(account, request.Password).Result;

            if (!result.Succeeded)
            {
                return Result<AccountDTO?>.Failure(AccountError.AddAccountFail, result.Errors.First().Description);
            }

            Console.WriteLine($"Account added: {account.UserName}");

            await _userManager.AddToRoleAsync(account, "Staff");

            return Result<AccountDTO?>.Success(new AccountDTO
            {
                Id = account.Id,
                Email = account.Email ?? "",
                Username = account.UserName ?? "",
                IsActive = account.IsActive,
                Role = "Staff"
            });
        }
        catch (Exception ex)
        {
            return Result<AccountDTO?>.Failure(AccountError.AddAccountFail, ex.Message);
        }
    }
}

