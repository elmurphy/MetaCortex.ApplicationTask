using MetaCortex.ApplicationTask.Manager.Interfaces;
using MetaCortex.ApplicationTask.Models;
using MetaCortex.ApplicationTask.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Manager
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _cache;
        public CacheManager(IDistributedCache cache)
        {
            this._cache = cache;
        }
        public void Add(ExchangeRateResponse exchangeRateResponse) => _cache.Set(exchangeRateResponse.Date.cacheDateTimeFormat(), exchangeRateResponse.ExchangeRates.dataObjectToCache());

        public async Task<List<ExchangeRateResponse>> Get(List<DateTime> dateTimes)
        {
            var response = new List<ExchangeRateResponse>();

            foreach (DateTime dateTime in dateTimes)
            {
                string currentDayDate = dateTime.cacheDateTimeFormat();
                byte[] currentDayResult = await _cache.GetAsync(currentDayDate);

                if (currentDayResult != null) response.Add(new ExchangeRateResponse() { Date = dateTime, ExchangeRates = currentDayResult.cacheDataToObject<List<ExchangeRate>>() });
            }

            return response;
        }
    }
}
