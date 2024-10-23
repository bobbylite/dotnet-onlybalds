using OnlyBalds.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlyBalds.Api.Data;

/// <summary>
/// Represents the data context for threads, posts, and comments.
/// </summary>
/// <remarks>
/// This class represents the data context for the threads, posts, and comments.
/// </remarks>
public class OnlyBaldsDataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnlyBaldsDataContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="OnlyBaldsDataContext"/> class using the specified options.
    /// </remarks>
    public OnlyBaldsDataContext(DbContextOptions<OnlyBaldsDataContext> options)
        : base(options) { }

    /// <summary>
    /// Gets the set of all <see cref="ThreadItem"/> entities in the context.
    /// </summary>
    public DbSet<ThreadItem> ThreadItems => Set<ThreadItem>();

    /// <summary>
    /// Gets the set of all <see cref="PostItem"/> entities in the context.
    /// </summary>
    public DbSet<PostItem> PostItems => Set<PostItem>();

    /// <summary>
    /// Gets the set of all <see cref="CommentItem"/> entities in the context.
    /// </summary>
    public DbSet<CommentItem> CommentItems => Set<CommentItem>();
}