using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Data;
using OnlyBalds.Api.Interfaces.Repositories;

namespace OnlyBalds.Api.Repositories;

public class CommentsRepository<T> : ICommentsRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsRepository{T}"/> class.
    /// </summary>
    /// <param name="context"><see cref="ThreadDataContext"/></param>
    public CommentsRepository(CommentDataContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    /// <inheritdoc/>
    public IEnumerable<T> GetAll()
    {
        var items = _dbSet.ToList();
        ArgumentNullException.ThrowIfNull(items);
        
        return items;
    }
    
    /// <inheritdoc/>
    public T GetById(Guid id)
    {
        var item = _dbSet.Find(id);
        ArgumentNullException.ThrowIfNull(item);
        
        return item;
    }

    /// <inheritdoc/>
    public async Task Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var addedEntity = _dbSet.Add(entity);
        ArgumentNullException.ThrowIfNull(addedEntity);

        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateById(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var item = _dbSet.Find(id);
        ArgumentNullException.ThrowIfNull(item);
        var updatedEntity = _dbSet.Update(item);
        ArgumentNullException.ThrowIfNull(updatedEntity);

        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteById(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var itemToReomve = _dbSet.Find(id);
        ArgumentNullException.ThrowIfNull(itemToReomve);
        var removedEntity = _dbSet.Remove(itemToReomve);
        ArgumentNullException.ThrowIfNull(removedEntity);

        await _context.SaveChangesAsync();
    }
}
