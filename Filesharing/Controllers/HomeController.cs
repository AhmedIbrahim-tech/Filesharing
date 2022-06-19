using System.Diagnostics;

namespace Filesharing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext db;

        public HomeController(ILogger<HomeController> logger , ApplicationDBContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {
            #region To_Get_Upload_By_UserID

            var highDownloads = db.Uploads.OrderByDescending( u => u.DownloadCount).Take(3)
                .Select(u => new UploadViewModel
                {
                    ID = u.ID,
                    FileName = u.FileName,
                    Size = u.Size,
                    ContentType = u.ContantType,
                    OriginalFileName = u.OriginalFileName,
                    DownloadCount = u.DownloadCount,
                });
            ViewBag.popular = highDownloads;
            return View();

            #endregion


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}