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
        : base(options)
    {
            Database.EnsureDeleted();
            Database.EnsureCreated();
         }

    /// <summary>
    /// Gets the set of all <see cref="Account"/> entities in the context.
    /// </summary>
    public DbSet<Account> Accounts => Set<Account>();

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

    public DbSet<QuestionnaireItems> QuestionnaireItems => Set<QuestionnaireItems>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Questionnaire)
            .WithOne()
            .HasForeignKey<QuestionnaireItems>(q => q.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionnaireItems>()
            .Property(t => t.StartDate)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<QuestionnaireItems>()
            .HasOne(q => q.Data)
            .WithOne()
            .HasForeignKey<QuestionnaireData>(q => q.QuestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ThreadItem>()
            .Property(t => t.StartDate)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<PostItem>()
            .Property(t => t.PostedOn)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        
        modelBuilder.Entity<PostItem>()
            .HasMany(p => p.Favorites)
            .WithOne()
            .HasForeignKey(f => f.PostId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<PostItem>()
            .HasMany(p => p.Comments)
            .WithOne()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .Property(f => f.FavoritedOn)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<CommentItem>()
            .Property(t => t.PostedOn)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        base.OnModelCreating(modelBuilder);
    }
}