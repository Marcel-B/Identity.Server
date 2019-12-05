namespace com.b_velop.Identity.Server.Infrastructure
{
    public interface ISecretProvider
    {
        string GetSecret(string key);
    }
}