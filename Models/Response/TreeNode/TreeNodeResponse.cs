namespace WizardSoftTestService.Models.Response.TreeNode;

public class TreeNodeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<TreeNodeResponse> Children { get; set; } = new();
}
