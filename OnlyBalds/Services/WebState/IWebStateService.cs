namespace OnlyBalds.Services.WebState;

public interface IWebStateService
{
    /// <summary>
    /// Retrieves the current web state.
    /// </summary>
    /// <returns></returns>
    int GetActiveUsers();

    /// <summary>
    /// Adds a user to the active users list.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IEnumerable<string> AddActiveUser(string user);

    /// <summary>
    /// Removes a user from the active users list.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IEnumerable<string> RemoveActiveUser(string user);
}
