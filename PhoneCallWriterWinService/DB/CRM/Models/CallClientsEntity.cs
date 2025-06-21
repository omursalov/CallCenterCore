using System;
using System.Collections.Generic;

namespace PhoneCallWriterWinService.DB.CRM.Models
{
    /// <summary>
    /// Активный обзвон
    /// </summary>
    public class CallClientsEntity
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public string Welcome { get; set; }
        public string Questions { get; set; }

        /// <summary>
        /// Связанные активные звонки, которые необходимо обработать
        /// </summary>
        public IList<PhoneCall> PhoneCalls { get; set; }

        public CallClientsEntity() => PhoneCalls = new List<PhoneCall>();
    }
}