namespace Server.Shared
{
    public interface ICachingService
    {
        void UpdateAsync<T>(string key, T value);
        Task<T?> GetAsync<T>(string key);
    
    }
}