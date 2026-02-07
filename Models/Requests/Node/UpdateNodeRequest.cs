namespace WizardSoftTestService.Models.Requests.Node;

public class UpdateNodeRequest
{
    public string Data { get; set; } = null!;
    public Guid? ParentId { get; set; }
}