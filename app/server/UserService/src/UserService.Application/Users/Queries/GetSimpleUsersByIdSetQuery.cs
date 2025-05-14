using Contract.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using UserService.Application.DTOs;
using UserService.Domain.Errors;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Queries;

public record GetSimpleUsersByIdSetQuery : IRequest<Result<List<UserDTO>?>>
{
    [Required]
    public HashSet<Guid> Ids { get; init; } = null!;
}
public class GetSimpleUsersByIdSetQueryHandler : IRequestHandler<GetSimpleUsersByIdSetQuery, Result<List<UserDTO>?>>
{
    private readonly IApplicationDbContext _context;

    public GetSimpleUsersByIdSetQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserDTO>?>> Handle(GetSimpleUsersByIdSetQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids;
        if (ids == null || ids.Count == 0)
        {
            return Result<List<UserDTO>?>.Failure(UserError.NullParameters, "Ids is null or empty.");
        }

        var users = await _context.Users
            .Where(user => ids.Contains(user.Id))
            .Select(user => new UserDTO
            {
                Id = user.Id,
                Address = user.Address,
                FullName = user.FullName,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
            }).ToListAsync();

        return Result<List<UserDTO>?>.Success(users);
    }
}

