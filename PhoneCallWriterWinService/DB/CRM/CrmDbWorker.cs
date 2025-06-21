using Dapper;
using PhoneCallWriterWinService.DB.CRM.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PhoneCallWriterWinService.DB.CRM
{
    public class CrmDbWorker
    {
        private readonly string _connectionString;

        public CrmDbWorker()
        {
            _connectionString = ConfigurationManager.AppSettings["CrmDbConnStr"];
        }

        public IList<CallClientsEntity> GetActiveCallClientsEntities()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<CallClientsEntity>("SELECT * FROM Users").ToList();
            }
        }
    }
}