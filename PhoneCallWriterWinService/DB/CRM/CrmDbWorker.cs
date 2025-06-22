using Dapper;
using PhoneCallWriterWinService.DB.CRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PhoneCallWriterWinService.DB.CRM
{
    /// <summary>
    /// На счет IDbConnection.. Может, его стоит создать один раз..
    /// Тут тоже подумать, подзабыл, как лучше. Обертку какую-нить.
    /// </summary>
    public class CrmDbWorker
    {
        private readonly string _connectionString;

        public CrmDbWorker()
        {
            _connectionString = ConfigurationManager.AppSettings["CrmDbConnStr"];
        }

        /// <summary>
        /// ONLY FOR TEST
        /// </summary>
        public void Ping()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                if (int.Parse(connection.ExecuteScalar("select 1").ToString()) != 1)
                    throw new System.Exception("Ошибка взаимодействия с SQL");
            }
        }

        public IList<CallClientsEntity> GetActiveCallClientsEntities()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var items = connection.Query<object>(
                    @"select 
                         cc.new_calling_contacts_entityId,
                         cc.new_name,
                         cc.new_welcome,
                         cc.new_questions,
                         p.activityId as 'phone_call_id',
                         c.contactId,
                         c.fullName,
                         c.mobilePhone
                    from new_calling_contacts_entityBase (nolock) cc
                    join PhoneCallBase (nolock) p
                        on cc.new_calling_contacts_entityId = p.new_calling_contacts_entityid
                    join ActivityPointerBase (nolock) act
                        on p.ActivityId = act.ActivityId
                    join ActivityPartyBase (nolock) ap
                        on act.ActivityId = ap.ActivityId
                            and ap.PartyObjectTypeCode = 2 -- contact
                    join ContactBase (nolock) c
                        on c.ContactId = ap.PartyId
                    where cc.statecode = 0
                        and cc.statuscode = 1 -- начало").Select(x => (IDictionary<string, object>)x).ToList();

                var result = new List<CallClientsEntity>();

                // Да, если будет миллион строк, тут кривой поиск..
                // Можно в будущем через словарь сделать.
                foreach (var item in items)
                {
                    var id = Guid.Parse(item["new_calling_contacts_entityId"].ToString());
                    
                    var callingContactsEntity = result.FirstOrDefault(x => x.Id == id);
                    
                    if (callingContactsEntity == null)
                    {
                        callingContactsEntity = new CallClientsEntity
                        {
                            Id = id,
                            Name = item["new_name"].ToString(),
                            Welcome = item["new_welcome"].ToString(),
                            Questions = item["new_questions"].ToString()
                        };
                        result.Add(callingContactsEntity);
                    }

                    callingContactsEntity.PhoneCalls.Add(new PhoneCall
                    {
                        Id = Guid.Parse(item["phone_call_id"].ToString()),
                        To = Guid.Parse(item["contactId"].ToString()),
                        FIO = item["fullName"].ToString(),
                        MobilePhone = item["mobilePhone"].ToString()
                    });
                }

                return result;
            }
        }
    }
}