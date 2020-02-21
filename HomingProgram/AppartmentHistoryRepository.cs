using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomingProgram
{
    public class AppartmentHistoryRepository
    {
        private const string appartmentHistoryPath = "appartmenthistory.json";

        public async Task<AppartmentHistory> GetAppartmentHistory()
        {
            if (!File.Exists(appartmentHistoryPath))
            {
                return new AppartmentHistory();
            }

            using (var stream = new FileStream(appartmentHistoryPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                return await JsonSerializer.DeserializeAsync<AppartmentHistory>(stream);
            }
        }

        public async Task SaveAppartment(List<string> objectNumbers)
        {
            var currentHistory = await GetAppartmentHistory();

            foreach (var objectNumber in objectNumbers)
            {
                var existingObject = currentHistory.AppartmentStatistics.SingleOrDefault(x => x.ObjectNumber == objectNumber);
                if (existingObject == null)
                {
                    var statistic = new AppartmentStatistics
                    {
                        ObjectNumber = objectNumber,
                    };
                    statistic.DatesShown.Add(DateTime.UtcNow.Date);
                    currentHistory.AppartmentStatistics.Add(statistic);
                }
                else
                {
                    existingObject.DatesShown.Add(DateTime.UtcNow.Date);
                }
            }

            var json = JsonSerializer.Serialize(currentHistory);

            await File.WriteAllTextAsync(appartmentHistoryPath, json, Encoding.UTF8);
        }
    }
}
