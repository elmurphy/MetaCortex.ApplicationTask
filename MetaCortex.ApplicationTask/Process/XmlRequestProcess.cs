using MetaCortex.ApplicationTask.Models;
using MetaCortex.ApplicationTask.Process.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace MetaCortex.ApplicationTask.Process
{
    public class XmlRequestProcess : IXmlRequestProcess
    {
        private readonly IConfiguration _configuration;
        public XmlRequestProcess(IConfiguration configuration)
        {

            this._configuration = configuration;
        }
        public async Task<ExchangeRateResponse> Handle(DateTime currentDate)
        {
            var result = new ExchangeRateResponse();
            XmlDocument document = await HandleRequest(currentDate);

            if (document != null)
            {
                result.ExchangeRates = HandleDocument(document);
                result.Date = currentDate;
                return result;
            }
            return null;
        }

        private List<ExchangeRate> HandleDocument(XmlDocument document)
        {
            var result = new List<ExchangeRate>();

            var documentData = JObject.Parse(JsonConvert.SerializeXmlNode(document));
            var Tarih_Date = documentData.SelectToken("Tarih_Date");
            foreach (var item in Tarih_Date.Last.First)
            {
                var temp = JsonConvert.DeserializeObject<RequestFromAPI>(item.ToString());
                temp.CurrencyCode = item["@CurrencyCode"].ToString();
                if (temp != null)
                    result.Add(new ExchangeRate()
                    {
                        Code = temp.CurrencyCode.Trim(),
                        Name = temp.CurrencyName.Trim(),
                        Price = !string.IsNullOrEmpty(temp.BanknoteBuying) ? Convert.ToDouble(temp.BanknoteBuying.Trim().Replace(".", ",")) : Convert.ToDouble(temp.ForexBuying.Trim().Replace(".", ",")),// Çevrimde sorun var.
                        Unit = temp.Unit
                    });
            }

            return result;
        }

        private async Task<XmlDocument> HandleRequest(DateTime currentDate)
        {
            var requestUrl = reorderRequestUrl(_configuration.GetSection("GlobalVariables").GetSection("RequestUrl").Value, currentDate);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(await response.Content.ReadAsStringAsync());
                return document;
            }
            return null;
        }
        private string reorderRequestUrl(string requestUrl, DateTime currentDate) => requestUrl.Replace("DD", currentDate.Day.ToString()).Replace("MM", currentDate.Month.ToString()).Replace("YYYY", currentDate.Year.ToString());
    }
}
