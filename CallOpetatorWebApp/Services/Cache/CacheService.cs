namespace CallOpetatorWebApp.Services.Cache
{
    /// <summary>
    /// Singleton
    /// </summary>
    public class CacheService : ICacheService
    {
        private IDictionary<string, object> _data;
        private object _lockObj = new object();

        public CacheService()
        {
            _data = new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Тут с lock'ами в будущем можно получше подумать,
        /// чтобы не стопать потоки, если много кто лезет в _data,
        /// и много где.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T Execute<T>(string key, Func<T> func)
        {
            lock (_lockObj)
            {
                if (!_data.ContainsKey(key))
                {
                    var result = func();
                    _data.Add(key, result);
                    return result;
                }
                else
                    return (T)_data[key];
            }
        }

        public void ForceClean()
        {
            // На будущее.. Вероятно стоит чистить что-то в data или все,
            // каждые, скажем, 15 минут.
        }
    }
}
