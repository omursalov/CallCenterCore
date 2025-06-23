using CallOpetatorWebApp.ViewModels;

namespace CallOpetatorWebApp.Services.Kafka
{
    /// <summary>
    /// SINGLETON (пусть пока будет так, в будущем можно увеличить число consumers)
    /// </summary>
    public interface IKafkaCallsReader
    {
        /// <summary>
        /// Получить следующий звонок
        /// </summary>
        OutCall Next();
    }
}