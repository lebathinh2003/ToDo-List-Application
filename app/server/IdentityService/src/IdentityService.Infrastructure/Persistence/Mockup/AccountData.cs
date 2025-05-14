namespace IdentityService.Infrastructure.Persistence.Mockup;

public static class AccountData
{
    public static readonly List<Account> Data = [
        new Account{
            Id = Guid.Parse("52b2bde6-0e51-4277-979a-42b5ad86d1ae"),
            Username = "admin", 
            Email = "admin@email.com",
            Password = "Pass123$",
            Role = "Admin"
        },
        new Account{
            Id = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            Username = "staff1",
            Email = "staff1@email.com",
            Password = "Pass123$",
            Role = "Staff"
        },
        new Account{
            Id = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            Username = "staff2",
            Email = "staff2@email.com",
            Password = "Pass123$",
            Role = "Staff"
        },
        new Account{
            Id = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            Username = "staff3",
            Email = "staff3@email.com",
            Password = "Pass123$",
            Role = "Staff"
        },
        new Account{
            Id = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            Username = "staff4",
            Email = "staff4@email.com",
            Password = "Pass123$",
            Role = "Staff"
        },
        new Account{
            Id = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            Username = "staff5",
            Email = "staff5@email.com",
            Password = "Pass123$",
            Role = "Staff"
        },
        new Account{
            Id = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            Username = "staff6",
            Email = "staff6@email.com",
            Password = "Pass123$",
            Role = "Staff"
        },
    ];
}

public class Account
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;

}
