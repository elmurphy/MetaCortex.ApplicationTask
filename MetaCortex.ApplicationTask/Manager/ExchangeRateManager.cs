using MetaCortex.ApplicationTask.Manager.Interfaces;
using MetaCortex.ApplicationTask.Models;
using MetaCortex.ApplicationTask.Process.Interfaces;
using MetaCortex.ApplicationTask.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Manager
{
    public class ExchangeRateManager : IExchangeRateManager
    {
        public readonly ICacheManager _cacheManager;
        public readonly IXmlRequestProcess _xmlRequestProcess;

        public ExchangeRateManager(ICacheManager cacheManager, IXmlRequestProcess xmlRequestProcess)
        {
            this._cacheManager = cacheManager;
            this._xmlRequestProcess = xmlRequestProcess;
        }

        public async Task<List<ExchangeRateResponse>> Handle(string Code, DateTime startDate, DateTime endDate)
        {
            List<ExchangeRateResponse> response = new List<ExchangeRateResponse>();

            ControlStartTimeNEndTime(startDate, endDate); //Control start date and end date

            List<DateTime> requestedDays = getRequestedDays(startDate, endDate); //Get days between start date and end date

            List<ExchangeRateResponse> cacheData = await _cacheManager.Get(requestedDays); //Get data from cache

            List<DateTime> notExistsDays = cacheData.getNotExistsDays(startDate, endDate); //Get doesn't exist days in cache

            List<ExchangeRateResponse> requestedData = await getFromRequest(notExistsDays); //Get not exists data from request

            response.AddRange(cacheData);
            response.AddRange(requestedData);

            response.ForEach(x => x.ExchangeRates = x.ExchangeRates.Where(y => y.Code == Code || string.IsNullOrEmpty(Code)).ToList()); //Get requested currency

            return response;
        }


        private void ControlStartTimeNEndTime(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new Exception("Start date is greater than end date.");
            if (endDate > DateTime.Now) throw new Exception("End date is greater than today.");
        }

        private List<DateTime> getRequestedDays(DateTime startDate, DateTime endDate)
        {
            List<DateTime> result = new List<DateTime>();

            int dateDifference = endDate.getDateDifference(startDate);

            for (int i = 0; i < dateDifference; i++)
            {
                result.Add(startDate.AddDays(i));
            }

            return result;
        }

        private async Task<List<ExchangeRateResponse>> getFromRequest(List<DateTime> notExistsDays)
        {
            var result = new List<ExchangeRateResponse>();

            foreach (DateTime notExistsDay in notExistsDays)
            {
                ExchangeRateResponse dayFromRequest = await _xmlRequestProcess.Handle(notExistsDay);

                if (dayFromRequest == null)
                {
                    dayFromRequest = new ExchangeRateResponse()
                    {
                        ExchangeRates = new List<ExchangeRate>(),
                        Date = notExistsDay
                    };
                }

                _cacheManager.Add(dayFromRequest);
                result.Add(dayFromRequest);
            }

            return result;
        }

    }
}
