using CallOpetatorWebApp.Models;

namespace CallOpetatorWebApp.Services.Kafka
{
    /// <summary>
    /// SINGLETON (пусть пока будет так, в будущем можно увеличить число consumers)
    /// </summary>
    public interface IKafkaCallsReader : IDisposable
    {
        /// <summary>
        /// Получить следующий звонок
        /// </summary>
        KafkaOutCall Next();
    }
}