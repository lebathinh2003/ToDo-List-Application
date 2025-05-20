using Contract.Common;
using IndentityService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace IdentityService.Application.ApplicationUsers.Queries;

public record SearchSimpleAccountQuery : IRequest<Result<List<string>?>>
{
    public string Keyword { get; init; } = null!;
}
public class SearchSimpleAccountQueryHandler : IRequestHandler<SearchSimpleAccountQuery, Result<List<string>?>>
{
    private readonly IApplicationDbContext _context;

    public SearchSimpleAccountQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<string>?>> Handle(SearchSimpleAccountQuery request, CancellationToken cancellationToken)
    {
        var keyword = request.Keyword.ToLower();

        var userIds = await _context.Users
            .Where(u => u.UserName!.ToLower().Contains(keyword) ||
                        u.Email!.ToLower().Contains(keyword))
            .Select(u => u.Id.ToString())
            .ToListAsync();

        return Result<List<string>?>.Success(userIds);
    }
}

