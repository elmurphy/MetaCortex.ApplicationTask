using MetaCortex.ApplicationTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Manager.Interfaces
{
    public interface ICacheManager
    {
        void Add(ExchangeRateResponse exchangeRateResponse);
        Task<List<ExchangeRateResponse>> Get(List<DateTime> dateTimes);
    }
}