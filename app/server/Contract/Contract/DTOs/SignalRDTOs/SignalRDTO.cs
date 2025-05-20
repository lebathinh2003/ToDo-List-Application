namespace Contract.DTOs.SignalRDTOs;
public class SignalRTaskItemDTO
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public string AssigneeName { get; set; } = null!;
    public string AssigneeUsername { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
}

public class SignalRTaskItemWithRecipentsDTO
{
    public SignalRTaskItemDTO TaskItem { get; set; } = null!;
    public List<Guid> Recipients { get; set; } = null!;
    public List<Guid> ExcludeRecipients { get; set; } = null!;

}

public class RecipentsDTO
{
    public List<Guid> Recipients { get; set; } = null!;
    public List<Guid> ExcludeRecipients { get; set; } = null!;
}
