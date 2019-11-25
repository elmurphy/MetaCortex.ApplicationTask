using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Models
{
    public class RequestFromAPI
    {
        public string CurrencyCode { get; set; }
        public int Unit { get; set; }
        public string CurrencyName { get; set; }
        public string BanknoteBuying { get; set; }
        public string ForexBuying { get; set; }
    }
}
