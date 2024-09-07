using Microsoft.EntityFrameworkCore;

namespace OnlyBalds.Api.Data;

/// <summary>
/// Represents the data context for the posts.
/// </summary>
/// <remarks>
/// This class represents the data context for the posts.
/// </remarks>
public class PostDataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostDataContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="PostDataContext"/> class using the specified options.
    /// </remarks>
    public PostDataContext(DbContextOptions<PostDataContext> options)
        : base(options) { }

    /// <summary>
    /// Gets the set of all <see cref="PostItems"/> entities in the context.
    /// </summary>
    public DbSet<PostItem> PostItems => Set<PostItem>();
}
