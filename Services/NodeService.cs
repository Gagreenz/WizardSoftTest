using Microsoft.EntityFrameworkCore;
using WizardSoftTestService.Db;
using WizardSoftTestService.Infrastructure.TransactionManager;
using WizardSoftTestService.Models.Entity;
using WizardSoftTestService.Models.Response.TreeNode;

namespace WizardSoftTestService.Services;

public class NodeService
{
    private readonly AppDbContext _db;
    private readonly IDbTransactionManager _tm;

    public NodeService(AppDbContext db, IDbTransactionManager uow)
    {
        _db = db;
        _tm = uow;
    }

    public async Task<Node> GetAsync(Guid id)
    {
        var node = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == id);
        if (node == null)
            throw new KeyNotFoundException("Node not found");

        return node;
    }

    public Task<Node> CreateAsync(string data, Guid? parentId)
        => _tm.RunAsync(async () =>
        {
            var newNodeId = Guid.NewGuid();

            if (parentId != null)
                await EnsureNoCycle(newNodeId, parentId.Value);

            if (parentId != null && !await _db.Nodes.AnyAsync(n => n.Id == parentId))
                throw new InvalidOperationException("Parent not found");

            var node = new Node { Id = newNodeId, Data = data, ParentId = parentId };
            _db.Nodes.Add(node);
            await _db.SaveChangesAsync();
            return node;
        });

    public Task UpdateAsync(Guid id, string data, Guid? parentId)
        => _tm.RunAsync(async () =>
        {
            if (parentId != null)
                await EnsureNoCycle(id, parentId.Value);

            var node = await GetAsync(id);
            node.Data = data;
            node.ParentId = parentId;
            node.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        });

    public Task DeleteAsync(Guid id)
        => _tm.RunAsync(async () =>
        {
            var nodes = await _db.Nodes.ToListAsync();

            var toDelete = new HashSet<Guid>();
            CollectDescendants(id, nodes, toDelete);

            var entities = nodes.Where(n => toDelete.Contains(n.Id));
            _db.Nodes.RemoveRange(entities);

            await _db.SaveChangesAsync();
        });

    public async Task<List<TreeNodeResponse>> GetTreeAsync()
    {
        var nodes = await _db.Nodes.AsNoTracking().ToListAsync();
        var lookup = nodes.ToDictionary(n => n.Id, n => new TreeNodeResponse
        {
            Id = n.Id,
            Name = n.Data
        });

        var roots = new List<TreeNodeResponse>();

        foreach (var node in nodes)
        {
            if (node.ParentId == null)
            {
                roots.Add(lookup[node.Id]);
            }
            else if (lookup.TryGetValue(node.ParentId.Value, out var parent))
            {
                parent.Children.Add(lookup[node.Id]);
            }
        }

        return roots;
    }

    private static void CollectDescendants(Guid id, List<Node> nodes, HashSet<Guid> result)
    {
        result.Add(id);

        var children = nodes.Where(n => n.ParentId == id);
        foreach (var child in children)
        {
            CollectDescendants(child.Id, nodes, result);
        }
    }

    private async Task EnsureNoCycle(Guid nodeId, Guid newParentId)
    {
        var nodes = await _db.Nodes.AsNoTracking().ToListAsync();
        var lookup = nodes.ToDictionary(n => n.Id, n => n.ParentId);

        Guid? current = newParentId;

        while (current != null)
        {
            if (current.Value == nodeId)
                throw new InvalidOperationException("Setting parent would create a cycle");

            lookup.TryGetValue(current.Value, out current);
        }
    }
}