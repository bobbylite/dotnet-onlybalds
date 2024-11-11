namespace OnlyBalds.Api.Interfaces.Repositories;

public interface IHomeRepository
{
    /// <summary>
    /// Get the index.
    /// </summary>
    /// <returns>A <see cref="string"/> asynchronously.</returns>
    /// <remarks>
    /// This method is intende to serve browser requests to thie index.
    /// </remarks>
    Task<string> GetIndex();
}