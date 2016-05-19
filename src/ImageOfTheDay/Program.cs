using System.Configuration;
using System.Threading.Tasks;
using ImageOfTheDay.Core;

namespace ImageOfTheDay
{
    class Program
    {
        static void Main(string[] args)
        {

            var t = UpdateImageOfTheDay();
            t.Wait();
        }

        private static async Task UpdateImageOfTheDay()
        {

            var updater = new ImageUpdater(ConfigurationManager.AppSettings["consumerKey"]);
            string rootNode = ConfigurationManager.AppSettings["rootNode"];
            await
                updater.UpdateImageOfTheDay(new[] {rootNode}, ConfigurationManager.AppSettings["makerIFTTTUrl"]);
        }
    }
}
