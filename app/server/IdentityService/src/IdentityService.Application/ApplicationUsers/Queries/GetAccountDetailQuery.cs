using Contract.Common;
using IdentityService.Application.DTOs;
using IndentityService.Domain.Errors;
using IndentityService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.ApplicationUsers.Queries;

public record GetAccountDetailQuery : IRequest<Result<AccountDTO?>>
{
    [Required]
    public Guid Id { get; init; } 
}
public class GetAccountDetailQueryHandler : IRequestHandler<GetAccountDetailQuery, Result<AccountDTO?>>
{
    private readonly IApplicationDbContext _context;
    public GetAccountDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AccountDTO?>> Handle(GetAccountDetailQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        if (id == Guid.Empty)
        {
            return Result<AccountDTO?>.Failure(AccountError.NullParameters, "Id is null or empty.");
        }

        var account = await (from user in _context.Users
                             where user.Id == id
                             join ur in _context.UserRoles on user.Id equals ur.UserId
                             join role in _context.Roles on ur.RoleId equals role.Id
                             select new AccountDTO
                             {
                                 Id = user.Id,
                                 Email = user.Email ?? "",
                                 Username = user.UserName ?? "",
                                 IsActive = user.IsActive,
                                 Role = role.Name!
                             }).FirstOrDefaultAsync();

        if (account == null)
        {
            return Result<AccountDTO?>.Failure(AccountError.NotFound, "Account not found.");
        }

        return Result<AccountDTO?>.Success(account);
    }
}

