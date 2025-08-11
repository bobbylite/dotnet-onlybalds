using Microsoft.EntityFrameworkCore;

namespace OnlyBalds.Api.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets the database context.
    /// </summary>
    /// <returns></returns>
    DbSet<T> GetDbSet();

    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <param name="id">Id of type <see cref="Guid"/></param>
    /// <returns></returns>
    T GetById(Guid id);

    /// <summary>
    /// Gets all entities of type <see cref="T"/>
    /// </summary>
    /// <returns><see cref="IEnumerable"/> of type <typeparamref name="T"/>.</returns>
    IEnumerable<T> GetAll();

    /// <summary>
    /// Adds an entity of type <see cref="T"/> to the database.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Added entity of type <see cref="T"/></returns>
    Task Add(T entity);

    /// <summary>
    /// Updates an entity of type <see cref="T"/> in the database.
    /// </summary>
    /// <param name="id">Id of type <see cref="Guid"/></param>
    /// <returns>Updated entity of type <see cref="T"/></returns>
    Task UpdateById(Guid id);

    /// <summary>
    /// Deletes an entity of type <see cref="T"/> from the database.
    /// </summary>
    /// <param name="id">Id of type <see cref="Guid"/></param>
    /// <returns>Deleted entity of type <see cref="T"/></returns>
    Task DeleteById(Guid id);
}
