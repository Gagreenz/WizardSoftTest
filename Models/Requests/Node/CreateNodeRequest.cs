namespace WizardSoftTestService.Models.Requests.Node;
public class CreateNodeRequest
{
    public string Data { get; set; } = null!;
    public Guid? ParentId { get; set; }
}