namespace WizardSoftTestService.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WizardSoftTestService.Models.Requests.Node;
using WizardSoftTestService.Models.Response.Node;
using WizardSoftTestService.Models.Response.TreeNode;
using WizardSoftTestService.Services;

[ApiController]
[Route("api/node")]
public class NodeController : ControllerBase
{
    private readonly NodeService _service;

    public NodeController(NodeService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<NodeResponse> Get(Guid id)
    {
        var node = await _service.GetAsync(id);
        return new NodeResponse
        {
            Id = node.Id,
            Data = node.Data,
            ParentId = node.ParentId
        };
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateNodeRequest req)
    {
        await _service.UpdateAsync(id, req.Data, req.ParentId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost]
    [Authorize]
    public async Task<NodeResponse> Create(CreateNodeRequest req)
    {
        var node = await _service.CreateAsync(req.Data, req.ParentId);
        return new NodeResponse
        {
            Id = node.Id,
            Data = node.Data,
            ParentId = node.ParentId
        };
    }

    [HttpGet("tree")]
    public async Task<List<TreeNodeResponse>> GetTree()
    {
        return await _service.GetTreeAsync();
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportTree()
    {
        var tree = await _service.GetTreeAsync();
        var json = JsonSerializer.Serialize(tree, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return File(
            Encoding.UTF8.GetBytes(json),
            "application/json",
            "tree-export.json"
        );
    }
}