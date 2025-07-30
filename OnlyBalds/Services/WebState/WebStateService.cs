namespace OnlyBalds.Services.WebState;

public class WebStateService : IWebStateService
{
    private IList<string> _activeUsers = new List<string>();

    /// <inheritdoc/>
    public int GetActiveUsers()
    {
        return _activeUsers.Count;
    }

    /// <inheritdoc/>
    public IEnumerable<string> AddActiveUser(string user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(user) || _activeUsers.Contains(user))
        {
            return Enumerable.Empty<string>();
        }

        _activeUsers.Add(user);
        return _activeUsers.ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<string> RemoveActiveUser(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            return Enumerable.Empty<string>();
        }

        _activeUsers.Remove(user);
        return _activeUsers.ToList();
    }
}
