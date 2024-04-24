using OnlyBalds.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlyBalds.Api.Data;

/// <summary>
/// Represents the database context for the application, which is used to manage the entity objects during run time, 
/// which includes populating objects with data from a database, change tracking, and persisting data to the database.
/// </summary>
public class ThreadDataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadDataContext"/> class using the specified options.
    /// The <see cref="DbContextOptions"/> can be used to configure certain behaviors of the context.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public ThreadDataContext(DbContextOptions<ThreadDataContext> options)
        : base(options) { }

    /// <summary>
    /// Gets the set of all <see cref="ThreadItem"/> entities in the context.
    /// </summary>
    public DbSet<ThreadItem> ThreadItems => Set<ThreadItem>();
}
