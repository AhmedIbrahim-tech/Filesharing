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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InputFile model)
    {
        if (!ModelState.IsValid || model.File == null)
            return Json(new { success = false, message = "Please select a valid file to upload." });

        var result = await uploadService.UploadFileAsync(model.File, UserId);
        
        if (result == null)
            return Json(new { success = false, message = "File upload failed. Ensure the file is under 5MB." });

        // For AJAX, we return the partial view of the newly created card
        return PartialView("_UploadCard", result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await uploadService.DeleteAsync(id, UserId);
        if (!success) return Json(new { success = false, message = "Failed to delete file." });

        return Json(new { success = true, message = "File deleted successfully!" });
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Search(string term) => View(uploadService.Search(term));
}
