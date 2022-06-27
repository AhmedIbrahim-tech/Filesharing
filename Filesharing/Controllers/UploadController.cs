namespace Filesharing.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly ApplicationDBContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public UploadController(ApplicationDBContext db, IWebHostEnvironment WebHostEnvironment)
        {
            this.db = db;
            webHostEnvironment = WebHostEnvironment;
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

            var result = db.Uploads.Where(u => u.UserId == userid)
                // AutoMapper Be., I User Upload And In View Use UploadViewModel
                .Select(u => new UploadViewModel
                {
                    ID = u.ID,
                    FileName = u.FileName,
                    Size = u.Size,
                    ContentType = u.ContantType,
                    OriginalFileName = u.OriginalFileName,
                    DownloadCount = u.DownloadCount,
                    UploadDate = u.UploadDate
                });
            return View(result);

            #endregion


        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Browse()
        {
            var Model = await db.Uploads
                .OrderByDescending(u => u.DownloadCount)
                .Select(u => new UploadViewModel
                {
                    FileName = u.FileName,
                    Size = u.Size,
                    ContentType = u.ContantType,
                    OriginalFileName = u.OriginalFileName,
                    DownloadCount = u.DownloadCount,
                }).ToListAsync();
            return View(Model);
        }


        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            var selectedFile = await db.Uploads.FirstOrDefaultAsync(u => u.FileName == id);
            if (selectedFile == null)
            {
                return NotFound();
            }

            selectedFile.DownloadCount++;

            db.Update(selectedFile);
            await db.SaveChangesAsync();

            var path = "~/Uploads/" + selectedFile.FileName;

            Response.Headers.Add("Expires", DateTime.Now.AddDays(-3).ToLongDateString());
            Response.Headers.Add("Cache-Control", "no-cache");

            return File(path, selectedFile.ContantType, selectedFile.OriginalFileName);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InputUpolad model)
        {
            if (ModelState.IsValid)
            {
                var NewFile = Guid.NewGuid().ToString();
                var extention = Path.GetExtension(model.File.FileName);
                var fileName = string.Concat(NewFile, extention);
                var root = webHostEnvironment.WebRootPath;
                var path = Path.Combine(root, "Uploads", fileName);

                using (var fs = System.IO.File.Create(path))
                {
                    await model.File.CopyToAsync(fs);
                }


                var inputupload = new Upload()
                {
                    OriginalFileName = model.File.FileName,
                    FileName = fileName,
                    ContantType = model.File.ContentType,
                    Size = model.File.Length,
                    UserId = userid,
                    UploadDate =  DateTime.Now
                };

                await db.Uploads.AddAsync(inputupload);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var selecteditem = await db.Uploads.FindAsync(id);
            if (selecteditem == null)
            {
                return NotFound();
            }
            // This's Code Be., Not AnyOne Delete Item From Another User 
            if (selecteditem.UserId != userid)
            {
                return NotFound();
            }
            return View(selecteditem);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var selecteditem = await db.Uploads.FindAsync(id);
            if (selecteditem == null)
            {
                return NotFound();
            }
            if (selecteditem.UserId != userid)
            {
                return NotFound();
            }

            db.Uploads.Remove(selecteditem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string term)
        {
            var Model = await db.Uploads
                .Where(U => U.OriginalFileName.Contains(term))
                .Select(u => new UploadViewModel
                {
                    FileName = u.FileName,
                    Size = u.Size,
                    ContentType = u.ContantType,
                    OriginalFileName = u.OriginalFileName
                }).ToListAsync();
            return View(Model);
        }
    }
}