using System;
using System.Collections.Generic;

namespace MetaCortex.ApplicationTask.Models
{
    public class ExchangeRateResponse
    {
        public DateTime Date { get; set; }
        public List<ExchangeRate> ExchangeRates { get; set; }
    }
}