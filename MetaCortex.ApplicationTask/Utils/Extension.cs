using MetaCortex.ApplicationTask.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Utils
{
    public static class Extension
    {
        public static string cacheDateTimeFormat(this DateTime dateTime) => dateTime.ToString("dd.MM.yyyy");
        public static T cacheDataToObject<T>(this byte[] data) => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        public static byte[] dataObjectToCache<T>(this T data) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        public static List<DateTime> getNotExistsDays(this List<ExchangeRateResponse> existDays, DateTime startDate, DateTime endDate)
        {
            List<DateTime> result = new List<DateTime>();

            int dateDifference = endDate.getDateDifference(startDate);

            for (int i = 0; i < dateDifference; i++)
            {
                if (existDays.FirstOrDefault(x => x.Date == startDate.AddDays(i)) == null) result.Add(startDate.AddDays(i));
            }

            return result;
        }
        public static int getDateDifference(this DateTime endDate, DateTime startDate)
        {
            return Convert.ToInt32(endDate.Subtract(startDate).TotalDays + 1);
        }
    }
}
