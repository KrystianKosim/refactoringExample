namespace LegacyApp;

public interface ClientRepository
{
    /// <summary>
    /// Simulating fetching a client from remote database
    /// </summary>
    /// <returns>Returning client object</returns>
    public Client GetById(int clientId);
}