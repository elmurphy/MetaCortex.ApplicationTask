using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetaCortex.ApplicationTask.Models;

namespace MetaCortex.ApplicationTask.Manager.Interfaces
{
    public interface IExchangeRateManager
    {
        Task<List<ExchangeRateResponse>> Handle(string Code, DateTime startDate, DateTime endDate);
    }
}