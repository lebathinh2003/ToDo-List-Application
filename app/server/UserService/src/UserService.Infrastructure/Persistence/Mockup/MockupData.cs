using UserService.Domain.Interfaces;
using UserService.Domain.Models;
namespace UserService.Infrastructure.Persistence.Mockup;

public class MockupData
{
    private readonly IApplicationDbContext _context;

    public MockupData(IApplicationDbContext context)
    {
        _context = context;
    }    

    public async Task SeedAllData()
    {
        Console.WriteLine("Seed user service data...");
        await SeedUserAsync();
    }

    public async Task SeedUserAsync()
    {
        Console.WriteLine("Seed user...");
        foreach (var seedUser in UserData.Data)
        {
            var user = new User
            {
                Id = seedUser.Id,
                FullName = seedUser.Fullname,
                Address = seedUser.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                IsAdmin = seedUser.IsAdmin,
            };

            var isUserExist = _context.Users.Any(u => u.Id == seedUser.Id);
            if (!isUserExist)
            {
                await _context.Users.AddAsync(user);
                Console.WriteLine($"User added: {user.FullName}");
            }
        }
        await _context.Instance.SaveChangesAsync();
    }
}
