using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace HomingProgram
{
    class Program
    {
        static void Main(string[] args)
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
                    if (!TryGetObjectNumbers(doc, out var objectNumbers) 
                        || !TryGetObjectCount(doc, out var objectCount))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    // Istället för att öppna explorer om det finns en count, läs upp <lägenhetshistoriken> och avgör om det finns en lägenhet sökt idag med samma objektsnummer redan.

                    if (int.TryParse(objectCount, out var count) && count > 0)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + "- Startar hemsida. Antal sidor: " + count);
                        Process.Start("explorer.exe", url);
                        Thread.Sleep(1000 * 60 * 30); // vänta 30 minuter
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

        private static bool TryGetObjectCount(HtmlDocument doc, out string count)
        {
            try
            {
                count = doc.DocumentNode
                        .Descendants()
                        .Single(node => node.HasClass("ojects-term-list"))
                        .Descendants("a")
                        .Single(node => node.Attributes.Any(attribute => attribute.Name == "href" && attribute.Value == url))
                        .Descendants()
                        .Single(node => node.HasClass("total-article"))
                        .InnerText;

                return true;
            }
            catch
            {
                count = null;
                return false;
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
