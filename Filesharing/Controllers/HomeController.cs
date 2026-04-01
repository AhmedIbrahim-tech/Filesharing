using Microsoft.AspNetCore.Localization;
using System.Diagnostics;
using System.Text;

namespace Filesharing.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ApplicationDBContext db, IMailServices mailServices) : Controller
    {
        public IActionResult Index()
        {
            var popularUploads = db.Uploads
                .OrderByDescending(u => u.DownloadCount)
                .Take(3)
                .Select(u => new UploadViewModel
                {
                    ID = u.ID,
                    FileName = u.FileName,
                    Size = u.Size,
                    ContentType = u.ContentType,
                    OriginalFileName = u.OriginalFileName,
                    DownloadCount = u.DownloadCount,
                });

            ViewBag.Popular = popularUploads;
            return View();
        }

        public IActionResult Privacy() => View();
        public IActionResult Info() => View();
        public IActionResult About() => View();

        [HttpGet]
        public IActionResult Contact() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var contactEntry = new Contact
            {
                Email = model.Email,
                Name = model.Name,
                Message = model.Message,
                Subject = model.Subject,
                UserId = CurrentUserId
            };

            await db.Contacts.AddAsync(contactEntry);
            await db.SaveChangesAsync();

            Response.Cookies.Append("fs_msg", "Your message has been sent successfully! We will get back to you soon.", new CookieOptions { Path = "/" });
            Response.Cookies.Append("fs_type", "success", new CookieOptions { Path = "/" });

            // Prepare and send notification email
            var emailBody = new StringBuilder();
            emailBody.AppendLine("<h1>FileSharing - New Contact Message</h1>");
            emailBody.AppendFormat("<p><strong>Name:</strong> {0}</p>", model.Name);
            emailBody.AppendFormat("<p><strong>Email:</strong> {0}</p>", model.Email);
            emailBody.AppendFormat("<p><strong>Subject:</strong> {0}</p>", model.Subject);
            emailBody.AppendFormat("<p><strong>Message:</strong></p><p>{0}</p>", model.Message);

            await mailServices.SendMailAsync(new EmailBody
            {
                Subject = $"New Message: {model.Subject}",
                Email = "ebrahema89859@gmail.com",
                Body = emailBody.ToString()
            });

            return RedirectToAction(nameof(Contact));
        }

        [HttpGet]
        public IActionResult SetLang(string lang, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}