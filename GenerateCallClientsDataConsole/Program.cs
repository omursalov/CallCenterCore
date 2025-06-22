using Data8.PowerPlatform.Dataverse.Client;
using GenerateCallClientsDataConsole;
using Microsoft.Xrm.Sdk;
using System.Configuration;

var crmClient = new OnPremiseClient(
    ConfigurationManager.AppSettings["CrmOrgServiceUrl"],
    ConfigurationManager.AppSettings["CrmLogin"],
    ConfigurationManager.AppSettings["CrmPass"]);

for (var i = 0; i < 5; i++)
{
    // Создадим обзвоны
    var subject = $"Обзвон {i}";
    var callingContactsEntityId = crmClient.Create(new Entity
    {
        LogicalName = "new_calling_contacts_entity",
        Attributes =
        {
            { "new_name", subject },
            { "new_welcome", "Привет! Готовы предложить Вам.." },
            { "new_questions", "Вопрос 1\nВопрос 2\nВопрос 3.." }
        }
    });

    // Создадим контактов
    var contactIds = new List<Guid>();
    for (var j = 0; j < 500; j++)
    {
        contactIds.Add(crmClient.Create(new Entity
        {
            LogicalName = "contact",
            Attributes =
            {
                { "firstname", "" },
                { "lastname", "" },
                { "middlename", "" },
                { "mobilephone", $"+7{CommonHelper.RandomDigits(10)}" }
            }
        }));
    }

    foreach (var contactId in contactIds)
    {
        var party1 = new[] 
        { 
            new Entity 
            { 
                LogicalName = "activityparty",
                Attributes =
                {
                    { "partyid", new EntityReference("systemuserid", crmClient.CallerId) }
                }
            } 
        };

        var party2 = new[]
        {
            new Entity
            {
                LogicalName = "activityparty",
                Attributes =
                {
                    { "partyid", new EntityReference("contact", contactId) }
                }
            }
        };

        // Создадим звонок, в лукапах обзвон и контакт
        contactIds.Add(crmClient.Create(new Entity
        {
            LogicalName = "phonecall",
            Attributes =
            {
                { "subject", subject },
                { "from", party1 },
                { "to", party2 },
                { "new_calling_contacts_entityid", new EntityReference("new_calling_contacts_entity", callingContactsEntityId) }
            }
        }));
    }
}