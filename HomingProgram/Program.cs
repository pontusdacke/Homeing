using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomingProgram
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://wahlinfastigheter.se/lediga-objekt/lagenheter/";
            var web = new HtmlWeb();
            while (true)
            {
#if RELEASE
                var doc = web.Load(url);
#else
                HtmlDocument doc = new HtmlDocument();
                doc.OptionOutputAsXml = true;
                var html = File.ReadAllText("wahlins/Lägenheter - Wahlin Fastigheter.html");
                doc.LoadHtml(html);
#endif

                try
                {
                    if (!TryGetObjectNumbers(doc, out var objectNumbers))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    var repository = new AppartmentHistoryRepository();
                    var history = await repository.GetAppartmentHistory();
                    var newObjects = objectNumbers.Where(on => !history.IsShownToday(on)).ToList();
                    if (newObjects.Any())
                    {
                        Console.WriteLine(DateTime.Now.ToString() + "- Startar hemsida. Antal nya objekt: " + newObjects.Count);

                        await repository.SaveAppartment(newObjects);

                        Process.Start("explorer.exe", url);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Kunde inte hämta antal. Tar en pausminut...");
                    Thread.Sleep(1000 * 60);
                }

                Thread.Sleep(1000);
            }
        }

        private static bool TryGetObjectNumbers(HtmlDocument doc, out List<string> objectNumbers)
        {
            try
            {
                objectNumbers = doc.DocumentNode
                    .SelectNodes("//div[@class='posts-wrapper-block']//div[@class='post-info']//strong")
                    .Where(node => node.InnerText.Contains("Objektsnummer"))
                    .Select(x => x.SelectSingleNode("span").InnerText.Trim())
                    .ToList();

                return true;
            }
            catch
            {
                objectNumbers = new List<string>();
                return false;
            }
        }
    }
}
