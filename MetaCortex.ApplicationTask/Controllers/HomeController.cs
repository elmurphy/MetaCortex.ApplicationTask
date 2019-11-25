using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MetaCortex.ApplicationTask.Models;
using Microsoft.Extensions.Caching.Distributed;
using MetaCortex.ApplicationTask.Manager.Interfaces;

namespace MetaCortex.ApplicationTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExchangeRateManager _exchangeRateManager;
        public HomeController(ILogger<HomeController> logger, IExchangeRateManager exchangeRateManager)
        {
            //this._logger = logger;
            this._exchangeRateManager = exchangeRateManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ResponseModal<List<ExchangeRateResponse>>> getData(string Code, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(Code)) throw new Exception("Code is cannot be null");
            return await getDataPriv(Code, startDate, endDate);
        }
        private async Task<ResponseModal<List<ExchangeRateResponse>>> getDataPriv(string Code, DateTime startDate, DateTime endDate)
        {
            var response = new ResponseModal<List<ExchangeRateResponse>>();

            try
            {

                var data = await _exchangeRateManager.Handle(Code, startDate, endDate);
                response.Data = data;

            }
            catch (Exception ex)
            {
                response.Exception = ex;
                //_logger.LogError(default(EventId), ex, "getData() => Error");
            }

            return response;
        }

        public async Task<ResponseModal<List<ExchangeRateResponse>>> getExchangeRates()
        {
            var dataDay = DateTime.Now;
            var response = new ResponseModal<List<ExchangeRateResponse>>();

            int timeOutCounter = 0;
            while (response.Data == null || response.Data.Count == 0 || timeOutCounter > 10)
            {
                response = await getDataPriv(null, dataDay, dataDay);
                dataDay = dataDay.AddDays(-1);
                timeOutCounter++;
            }

            return response;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
