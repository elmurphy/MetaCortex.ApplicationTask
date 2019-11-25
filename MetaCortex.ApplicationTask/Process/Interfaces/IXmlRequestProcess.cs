using MetaCortex.ApplicationTask.Models;
using System;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Process.Interfaces
{
    public interface IXmlRequestProcess
    {
        Task<ExchangeRateResponse> Handle(DateTime currentDate);
    }
}