using Contract.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Queries;

public record SearchSimpleUserQuery : IRequest<Result<List<string>?>>
{
    public string Keyword { get; init; } = null!;
}
public class SearchSimpleUserQueryHandler : IRequestHandler<SearchSimpleUserQuery, Result<List<string>?>>
{
    private readonly IApplicationDbContext _context;

    public SearchSimpleUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<string>?>> Handle(SearchSimpleUserQuery request, CancellationToken cancellationToken)
    {
        var keyword = request.Keyword.ToLower();

        var userIds = await _context.Users
            .Where(u => u.FullName.ToLower().Contains(keyword) ||
                        u.Address.ToLower().Contains(keyword))
            .Select(u => u.Id.ToString())
            .ToListAsync();

        return Result<List<string>?>.Success(userIds);
    }
}

