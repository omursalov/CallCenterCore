using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace CallOpetatorWebApp.Controllers
{
    public class BaseController : Controller
    {
        protected ICrmService _crmService;
        protected ICacheService _cacheService;
        protected IKafkaCallsReader _kafkaCallsReader;
        protected readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        public BaseController(ICrmService crmService, 
            ICacheService cacheService,
            IKafkaCallsReader kafkaCallsReader)
        {
            _crmService = crmService;
            _cacheService = cacheService;
            _kafkaCallsReader = kafkaCallsReader;
        }

        /// <summary>
        /// Логировать ошибки, для тестов сделал
        /// </summary>
        public IActionResult WrapperExecute(Func<IActionResult> method)
        {
            try
            {
                return method();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }
    }
}