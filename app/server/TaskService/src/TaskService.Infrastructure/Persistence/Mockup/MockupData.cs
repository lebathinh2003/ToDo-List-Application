using TaskService.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;
namespace TaskService.Infrastructure.Persistence.Mockup;
public class MockupData
{
    private readonly IApplicationDbContext _context;

    public MockupData(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllData()
    {
        Console.WriteLine("Seed task service data...");
        await SeedTaskAsync();
    }

    public async Task SeedTaskAsync()
    {
        Console.WriteLine("Seed task...");
        if (_context.Tasks.Any())
        {
            return;
        }
        foreach (var seedTask in TaskData.Data)
        {
            var task = new Domain.Models.Task
            {
                Id = Guid.NewGuid(),
                Title = seedTask.Title,
                Description = seedTask.Description,
                AssigneeId = seedTask.AssigneeId,
                Status = Domain.Models.TaskStatus.NotStarted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _context.Tasks.AddAsync(task);
        }
        await _context.Instance.SaveChangesAsync();
        Console.WriteLine("Done seed task...");

    }
}
