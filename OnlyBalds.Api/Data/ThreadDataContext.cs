using OnlyBalds.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlyBalds.Api.Data;

/// <summary>
/// Represents the data context for the threads.
/// </summary>
/// <remarks>
/// This class represents the data context for the threads.
/// </remarks>
public class ThreadDataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadDataContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ThreadDataContext"/> class using the specified options.
    /// </remarks>
    public ThreadDataContext(DbContextOptions<ThreadDataContext> options)
        : base(options) { }

    /// <summary>
    /// Gets the set of all <see cref="ThreadItem"/> entities in the context.
    /// </summary>
    public DbSet<ThreadItem> ThreadItems => Set<ThreadItem>();
}
