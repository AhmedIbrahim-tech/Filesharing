using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Diagnostics;
using System.Text;

namespace Filesharing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext db;
        private readonly IMailServices mailServices;

        public HomeController(ILogger<HomeController> logger , ApplicationDBContext db , IMailServices mailServices)
        {
            _logger = logger;
            this.db = db;
            this.mailServices = mailServices;
        }

        private string userid
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
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
                    ContentType = u.ContentType,
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

        public IActionResult Info()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModal modal)
        {
            if (ModelState.IsValid)
            {
                var Contact = new Contact
                {
                    Email = modal.Email,
                    Name = modal.Name,
                    Message = modal.Message,
                    Subject = modal.Subject,
                    UserId = userid
                };

                db.Contacts.Add(Contact) ;
                await db.SaveChangesAsync();
                TempData["Message"] = "Message has been Successful";

                #region Send_Email
                // build Body To Email
                StringBuilder Sb = new StringBuilder();
                Sb.AppendLine("<h1> File Sharing - Unread Message </h1>");
                Sb.AppendFormat("Name : {0}", modal.Name);
                Sb.AppendFormat("Email : {0} ", modal.Email);
                Sb.AppendLine();
                Sb.AppendFormat("Subject : {0} ", modal.Subject);
                Sb.AppendFormat("Message : {0} ", modal.Message);

                // Send Email
                mailServices.SendMail(new InputEmailMessage
                {
                    Subject = "You have Unread Message",
                    Email = "ebrahema89859@gmail.com",
                    Body = Sb.ToString()
                });

                #endregion

                return RedirectToAction("Contact");
            }
            return View(modal);
        }

        [HttpGet]
        public ActionResult SetLang(string Lang , string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(Lang))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Lang)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1)}
                );
            }


			if (!string.IsNullOrEmpty(returnUrl))
			{
				return LocalRedirect(returnUrl);
			}

			return RedirectToAction("Index");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}