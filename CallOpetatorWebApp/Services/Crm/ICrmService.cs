using Data8.PowerPlatform.Dataverse.Client;

namespace CallOpetatorWebApp.Services.Crm
{
    public interface ICrmService
    {
        public OnPremiseClient Client { get; }
    }
}