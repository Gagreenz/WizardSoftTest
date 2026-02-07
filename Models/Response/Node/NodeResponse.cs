namespace WizardSoftTestService.Models.Response.Node;
public class NodeResponse
{
    public Guid Id { get; set; }
    public string Data { get; set; } = null!;
    public Guid? ParentId { get; set; }
}