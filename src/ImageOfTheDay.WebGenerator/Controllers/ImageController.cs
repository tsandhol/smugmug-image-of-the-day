using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ImageOfTheDay.Core;

namespace ImageOfTheDay.WebGenerator.Controllers
{
    public class ImageController : Controller
    {
        // GET: Image
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Publish(string consumerKey, IEnumerable<string> rootNodes, string targetMakerUrl)
        {
            var updater = new ImageUpdater(consumerKey);
            await updater.UpdateImageOfTheDay(rootNodes, targetMakerUrl);
            return Json(true);
        }
    }
}