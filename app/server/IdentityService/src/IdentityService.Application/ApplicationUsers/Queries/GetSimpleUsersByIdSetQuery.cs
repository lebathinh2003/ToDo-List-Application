using Contract.Common;
using IdentityService.Application.DTOs;
using IndentityService.Domain.Errors;
using IndentityService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.ApplicationUsers.Queries;

public record GetSimpleAccountsByIdSetQuery : IRequest<Result<List<AccountDTO>?>>
{
    [Required]
    public HashSet<Guid> Ids { get; init; } = null!;
}
public class GetSimpleAccountsByIdSetQueryHandler : IRequestHandler<GetSimpleAccountsByIdSetQuery, Result<List<AccountDTO>?>>
{
    private readonly IApplicationDbContext _context;
    public GetSimpleAccountsByIdSetQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AccountDTO>?>> Handle(GetSimpleAccountsByIdSetQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids;
        if (ids == null || ids.Count == 0)
        {
            return Result<List<AccountDTO>?>.Failure(AccountError.NullParameters, "Ids is null or empty.");
        }

        var query = from user in _context.Users
                    where ids.Contains(user.Id)
                    join ur in _context.UserRoles on user.Id equals ur.UserId
                    join role in _context.Roles on ur.RoleId equals role.Id
                    select new AccountDTO
                    {
                        Id = user.Id,
                        Email = user.Email ?? "",
                        Username = user.UserName ?? "",
                        IsActive = user.IsActive,
                        Role = role.Name!
                    };

        var accounts = await query.ToListAsync();

        return Result<List<AccountDTO>?>.Success(accounts);
    }
}

