namespace CallOpetatorWebApp.Models
{
    public class KafkaOutCall
    {
        public Guid CallClientsEntityId { get; set; }
        public string Welcome { get; set; }
        public string Questions { get; set; }
        public Guid PhoneCallId { get; set; }
        public Guid ContactId { get; set; }
        public string ContactFIO { get; set; }
        public string ContactMobilePhone { get; set; }
    }
}