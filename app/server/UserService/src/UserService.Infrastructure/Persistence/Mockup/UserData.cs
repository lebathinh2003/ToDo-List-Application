namespace UserService.Infrastructure.Persistence.Mockup;

public static class UserData
{
    public static readonly List<UserMockup> Data = [
        new UserMockup{
            Id = Guid.Parse("52b2bde6-0e51-4277-979a-42b5ad86d1ae"),
            Fullname = "Alice Le",
            Address = "123 Admin St, Hanoi",
            IsAdmin = true
        },
        new UserMockup{
            Id = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            Fullname = "Bob Nguyen",
            Address = "456 Staff Rd, Da Nang",
            IsAdmin = false
        },
        new UserMockup{
            Id = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            Fullname = "Charlie Tran",
            Address = "789 Worker Ln, Ho Chi Minh City",
            IsAdmin = false
        },
        new UserMockup{
            Id = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            Fullname = "Diana Le",
            Address = "321 District Ave, Can Tho",
            IsAdmin = false
        },
        new UserMockup{
            Id = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            Fullname = "Ethan Pham",
            Address = "654 Alley St, Hai Phong",
            IsAdmin = false
        },
        new UserMockup{
            Id = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            Fullname = "Fiona Do",
            Address = "147 Flower Rd, Hue",
            IsAdmin = false
        },
        new UserMockup{
            Id = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            Fullname = "George Vo",
            Address = "963 West Side, Nha Trang",
            IsAdmin = false
        },
    ];
}

public class UserMockup
{
    public Guid Id { get; set; }
    public string Fullname { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool IsAdmin { get; set; } = false;
}
