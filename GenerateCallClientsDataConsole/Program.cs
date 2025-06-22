using Data8.PowerPlatform.Dataverse.Client;
using Data8.PowerPlatform.Dataverse.Client.Wsdl;
using Faker;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

// В будущем нужен норм SSL сертификат, пока так (для теста)
ServicePointManager.ServerCertificateValidationCallback =
   delegate (object sender, X509Certificate certificate, X509Chain chain,
       SslPolicyErrors sslPolicyErrors) { return true; };

var crmClient = new OnPremiseClient(
    ConfigurationManager.AppSettings["CrmOrgServiceUrl"],
    ConfigurationManager.AppSettings["CrmLogin"],
    ConfigurationManager.AppSettings["CrmPass"]);

var response = (WhoAmIResponse)crmClient.Execute(new WhoAmIRequest());
var callerId = response.UserId;

for (var i = 0; i < 5; i++)
{
    // Создадим обзвоны
    var subject = Company.Name();
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

    Console.WriteLine($"Создан обзвон {callingContactsEntityId}");

    // Создадим контактов
    for (var j = 0; j < 500; j++)
    {
        var fioParts = Name.FullName().Split(' ');
        var contactId = crmClient.Create(new Entity
        {
            LogicalName = "contact",
            Attributes =
            {
                { "firstname", fioParts[0] },
                { "lastname", fioParts[1] },
                { "middlename", fioParts.Length > 2 ? fioParts[2] : "TEST" },
                { "mobilephone", Phone.Number().Split(' ')[0] }
            }
        });

        Console.WriteLine($"Создан контакт {contactId}");

        var party1 = new[]
        {
            new Entity
            {
                LogicalName = "activityparty",
                Attributes =
                {
                    { "partyid", new EntityReference("systemuser", callerId) }
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
        var phoneCallId = crmClient.Create(new Entity
        {
            LogicalName = "phonecall",
            Attributes =
            {
                { "subject", subject },
                { "from", party1 },
                { "to", party2 },
                { "new_calling_contacts_entityid", new EntityReference("new_calling_contacts_entity", callingContactsEntityId) }
            }
        });

        Console.WriteLine($"Создан звонок {phoneCallId}");
    }
}