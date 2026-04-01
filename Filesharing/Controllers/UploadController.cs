namespace Filesharing.Controllers;

[Authorize]
public class UploadController(IUploadService uploadService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public IActionResult Index() => View(uploadService.GetAllUploadsByUserId(UserId));

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Browse() => View(uploadService.GetAllUploads());

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Download(string id) // id is the fileName
    {
        var selectedFile = await uploadService.FindAsync(id);
        if (selectedFile == null) return NotFound();

        await uploadService.IncrementDownloadCountAsync(id);

        var path = Path.Combine("Uploads", selectedFile.FileName);
        
        // Prevent caching for secure downloads
        Response.Headers.Append("Expires", DateTime.Now.AddDays(-3).ToString("R"));
        Response.Headers.Append("Cache-Control", "no-cache");

        return File($"~/{path}", selectedFile.ContentType, selectedFile.OriginalFileName);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InputFile model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await uploadService.UploadFileAsync(model.File, UserId);
        
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "File upload failed.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var selectedItem = await uploadService.FindAsync(id, UserId);
        if (selectedItem == null) return NotFound();
        return View(selectedItem);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        await uploadService.DeleteAsync(id, UserId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Search(string term) => View(uploadService.Search(term));
}
