using System;
using OnlyBalds.Api.Interfaces.Repositories;

namespace OnlyBalds.Api.Repositories;

public class HomeRepository : IHomeRepository
{
    private readonly string _htmlFilePath;

    public HomeRepository()
    {
        // Define the path where your static HTML file is located
        _htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
    }

    /// <inheritdoc/>
    public async Task<string> GetIndex()
    {
        if (File.Exists(_htmlFilePath))
        {
            return await File.ReadAllTextAsync(_htmlFilePath);
        }

        return "<html><body><h1>Home page not found</h1></body></html>";
    }
}
