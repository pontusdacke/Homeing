using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                var doc = web.Load(url);
                try
                {
                    var numberOfAppartments = doc.DocumentNode
                        .Descendants()
                        .Single(node => node.HasClass("ojects-term-list"))
                        .Descendants("a")
                        .Single(node => node.Attributes.Any(attribute => attribute.Name == "href" && attribute.Value == url))
                        .Descendants()
                        .Single(node => node.HasClass("total-article"))
                        .InnerText;

                    if (int.TryParse(numberOfAppartments, out var count) && count > 0)
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
    }
}
