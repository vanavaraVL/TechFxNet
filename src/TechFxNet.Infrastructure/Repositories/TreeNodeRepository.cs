using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TechFxNet.Domain.Entities;
using TechFxNet.Domain.Exceptions;

namespace TechFxNet.Infrastructure.Repositories;

public interface ITreeNodeRepository
{
    Task<TreeEntity?> GetTreeByName(string name, CancellationToken ct);

    Task<TreeEntity> CreateTree(string name, CancellationToken ct);

    Task<NodeEntity> CreateNode(long treeId, long parentNodeId, string nodeName, CancellationToken ct);

    Task<NodeEntity?> GetParentNode(long treeId, long parentNodeId, CancellationToken ct);

    Task<NodeEntity?> GetNodeById(long treeId, long nodeId, CancellationToken ct);

    Task<IReadOnlyCollection<NodeEntity>> GetTreeNodes(long treeId, CancellationToken ct);

    Task RenameNode(long nodeId, string newName, CancellationToken ct);

    Task DeleteNode(NodeEntity nodeEntity, CancellationToken ct);
}

public class TreeNodeRepository: ITreeNodeRepository
{
    private readonly TechDbContext _context;

    public TreeNodeRepository(TechDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<TreeEntity?> GetTreeByName(string name, CancellationToken ct)
    {
        var nameParam = name.ToLower();

        return _context.Trees.AsNoTracking()
            .Include(t => t.Nodes)
            .FirstOrDefaultAsync(t => t.TreeName.ToLower() == nameParam, ct);
    }

    public async Task<IReadOnlyCollection<NodeEntity>> GetTreeNodes(long treeId, CancellationToken ct)
    {
        var nodeList = await _context.Nodes.AsNoTracking()
            .Where(n => n.TreeId == treeId)
            .ToListAsync(ct);

        return nodeList;
    }

    public Task<NodeEntity?> GetNodeById(long treeId, long nodeId, CancellationToken ct)
    {
        return _context.Nodes.AsNoTracking()
            .FirstOrDefaultAsync(n => n.TreeId == treeId && n.Id == nodeId, ct);
    }

    public async Task RenameNode(long nodeId, string newName, CancellationToken ct)
    {
        try
        {
            var entity = await _context.Nodes.FirstAsync(n => n.Id == nodeId, ct);

            entity.NodeName = newName;

            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new SecureException($"Node {newName} already exists");
        }
    }

    public Task<NodeEntity?> GetParentNode(long treeId, long parentNodeId, CancellationToken ct)
    {
        return _context.Nodes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.TreeId == treeId && n.Id == parentNodeId, ct);
    }

    public async Task<NodeEntity> CreateNode(long treeId, long parentNodeId, string nodeName, CancellationToken ct)
    {
        try
        {
            var newNode = new NodeEntity
            {
                NodeName = nodeName,
                TreeId = treeId,
                ParentNodeId = parentNodeId
            };

            await _context.Nodes.AddAsync(newNode, ct);
            await _context.SaveChangesAsync(ct);

            return newNode;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new SecureException($"Node {nodeName} already exists");
        }
    }

    public async Task DeleteNode(NodeEntity nodeEntity, CancellationToken ct)
    {
        try
        {
            _context.Nodes.Remove(nodeEntity);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            throw new SecureException($"Can't delete node: {ex.Message}::{ex.InnerException?.Message}");
        }
    }

    public async Task<TreeEntity> CreateTree(string name, CancellationToken ct)
    {
        var entity = new TreeEntity() { TreeName = name };
            
        await _context.Trees.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        var defaultNode = new NodeEntity()
        {
            NodeName = "DefaultRoot",
            TreeId = entity.Id
        };

        await _context.Nodes.AddAsync(defaultNode, ct);
        await _context.SaveChangesAsync(ct);

        return (await GetTreeByName(name, ct))!;
    }
}