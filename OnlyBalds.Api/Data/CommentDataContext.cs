using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Data;

/// <summary>
/// Represents the data context for the comments.
/// </summary>
/// <remarks>
/// This class represents the data context for the comments.
/// </remarks>
/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
public class CommentDataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDataContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="CommentDataContext"/> class using the specified options.
    /// </remarks>
    public CommentDataContext(DbContextOptions<CommentDataContext> options)
        : base(options) { }

    /// <summary>
    /// Gets the set of all <see cref="CommentItem"/> entities in the context.
    /// </summary>
    public DbSet<CommentItem> CommentItems => Set<CommentItem>();

}