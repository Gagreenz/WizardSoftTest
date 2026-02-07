namespace WizardSoftTestService.Models.Entity;

public class Node
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Data { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public Node? Parent { get; set; }
    public ICollection<Node> Children { get; set; } = new List<Node>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}