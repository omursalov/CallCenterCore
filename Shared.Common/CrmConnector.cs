using Data8.PowerPlatform.Dataverse.Client;
using Microsoft.Crm.Sdk.Messages;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GenerateCallClientsDataConsole
{
    public static class CrmConnector
    {
        public static OnPremiseClient Create(out System.Guid callerId, out System.Exception ex) 
        {
            ex = null;
            callerId = default;

            // В будущем нужен норм SSL сертификат, пока так (для теста)
            ServicePointManager.ServerCertificateValidationCallback =
               delegate (object sender, X509Certificate certificate, X509Chain chain,
                   SslPolicyErrors sslPolicyErrors) { return true; };

            var crmClient = new OnPremiseClient(
                ConfigurationManager.AppSettings["CrmOrgServiceUrl"],
                ConfigurationManager.AppSettings["CrmLogin"],
                ConfigurationManager.AppSettings["CrmPass"]);

            try
            {
                var response = (WhoAmIResponse)crmClient.Execute(new WhoAmIRequest());
                callerId = response.UserId;
            }
            catch (System.Exception exception)
            {
                ex = exception;
            }

            return crmClient;
        }

        public static OnPremiseClient Create(
            string crmOrgServiceUrl, string crmLogin, string crmPass,
            out System.Guid callerId, out System.Exception ex)
        {
            ex = null;
            callerId = default;

            // В будущем нужен норм SSL сертификат, пока так (для теста)
            ServicePointManager.ServerCertificateValidationCallback =
               delegate (object sender, X509Certificate certificate, X509Chain chain,
                   SslPolicyErrors sslPolicyErrors) { return true; };

            var crmClient = new OnPremiseClient(
                crmOrgServiceUrl,
                crmLogin,
                crmPass);

            try
            {
                var response = (WhoAmIResponse)crmClient.Execute(new WhoAmIRequest());
                callerId = response.UserId;
            }
            catch (System.Exception exception)
            {
                ex = exception;
            }

            return crmClient;
        }
    }
}