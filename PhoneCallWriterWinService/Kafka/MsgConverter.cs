using Newtonsoft.Json;
using PhoneCallWriterWinService.DB.CRM.Models;

namespace PhoneCallWriterWinService.Kafka
{
    public static class MsgConverter
    {
        public static string Execute(CallClientsEntity callClientsEntity, PhoneCall phoneCall)
            => JsonConvert.SerializeObject(new
            {
                CallClientsEntityId = callClientsEntity.Id,
                Welcome = callClientsEntity.Welcome,
                Questions = callClientsEntity.Questions,
                PhoneCallId = phoneCall.Id,
                ContactId = phoneCall.To,
                ContactFIO = phoneCall.FIO,
                ContactMobilePhone = phoneCall.MobilePhone,
            });
    }
}