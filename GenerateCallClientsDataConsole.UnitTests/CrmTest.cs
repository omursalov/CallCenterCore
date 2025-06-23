namespace GenerateCallClientsDataConsole.UnitTests
{
    public class CrmTest
    {
        [Fact]
        public void ConnectionTest()
        {
            var crmClient = CrmConnector.Create(
                "https://192.168.0.21:443/D365CE/XRMServices/2011/Organization.svc",
                "MURSALOV\\Администратор",
                "X0baLQEZ", 
                out Guid callerId, out Exception ex);
            Assert.NotNull(crmClient);
            Assert.Null(ex);
            Assert.True(callerId != default);
        }
    }
}