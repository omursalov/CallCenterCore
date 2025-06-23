using CallOpetatorWebApp.Models;

namespace CallOpetatorWebApp.ViewModels
{
    public class OutCallModel
    {
        /// <summary>
        /// Звонок из Kafka (инфа)
        /// </summary>
        public KafkaOutCall? OutCall { get; set; }
        
        /// <summary>
        /// Сколько говорим? (скажем, в минутах)
        /// </summary>
        public int? Timer { get; set; }

        /// <summary>
        /// Доменное имя оператор
        /// </summary>
        public string? OperatorName {  get; set; }

        /// <summary>
        /// CRM ID оператор
        /// </summary>
        public Guid? OperatorId { get; set; }
    }
}