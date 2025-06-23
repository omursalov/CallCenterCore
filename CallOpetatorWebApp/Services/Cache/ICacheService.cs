namespace CallOpetatorWebApp.Services.Cache
{
    public interface ICacheService
    {
        T Execute<T>(string key, Func<T> func);
    }
}