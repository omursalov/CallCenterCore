using System;

namespace PhoneCallWriterWinService.DB.CRM.Models
{
    /// <summary>
    /// Активный звонок (activity)
    /// </summary>
    public class PhoneCall
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Кому звоним (контакт)
        /// </summary>
        public Guid To { get; set; }

        /// <summary>
        /// ФИО контакта
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// Телефон контакта
        /// </summary>
        public string MobilePhone { get; set; }
    }
}