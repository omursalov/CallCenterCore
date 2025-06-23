using Data8.PowerPlatform.Dataverse.Client;
using GenerateCallClientsDataConsole;

namespace CallOpetatorWebApp.Services.Crm
{
    public class CrmService : ICrmService
    {
        private IConfiguration _configuration;

        public OnPremiseClient Client => CrmConnector.Create(
            _configuration["Crm:OrgUrl"], _configuration["Crm:Login"], _configuration["Crm:Pass"],
            out Guid callerId, out Exception ex);
    }
}