using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using SimpleWebScraper.Data;
using SimpleWebScraper.Builders;
using SimpleWebScraper.Workers;

namespace SimpleWebScraper
{
    class Program
    {

        private const string Method = "search";

        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("Please enter the city you would like to scrape information from: ");
                var craigslistCity = Console.ReadLine() ?? string.Empty;


                Console.WriteLine("Please enter the CraigList category that you would like to scrap: ");
                var craigslistCategoryName = Console.ReadLine() ?? string.Empty;


                using (WebClient client = new WebClient())
                {
                    string contents = client.DownloadString($"http://{craigslistCity.Replace(" ", string.Empty)}" +
                        $".craigslist.org/{Method}/{craigslistCategoryName}");

                    ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                        .WithData(contents)
                        .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                        .WithRegentOption(RegexOptions.ExplicitCapture)
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@">(.*?)</a>")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build())
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@"href=\""(.*?)\""")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build())
                        .Build();

                    Scraper scraper = new Scraper();

                    var scrapedElements = scraper.Scrape(scrapeCriteria);

                    if (scrapedElements.Any())
                    {
                        foreach (var scrapedElement in scrapedElements)
                        {
                            Console.WriteLine(scrapedElement);
                        }
                    }
                    else
                    {
                        Console.WriteLine("There were no matches for specified scraped criteria");
                    }
                }

            
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
