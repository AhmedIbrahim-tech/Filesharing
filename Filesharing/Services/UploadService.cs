using System.Linq.Expressions;

namespace Filesharing.Services;

public interface IUploadService
{
    IQueryable<UploadViewModel> GetAllUploads();
    IQueryable<UploadViewModel> GetAllUploadsByUserId(string userId);
    IEnumerable<UploadViewModel> Search(string searchTerm);
    Task<UploadViewModel?> UploadFileAsync(IFormFile file, string userId);
    Task<UploadViewModel?> FindAsync(int id, string userId);
    Task<UploadViewModel?> FindAsync(string fileName);
    Task<bool> DeleteAsync(int id, string userId);
    Task IncrementDownloadCountAsync(string fileName);
    Task<int> GetUploadCountAsync();
}

public class UploadService(ApplicationDBContext db, IWebHostEnvironment webHostEnvironment) : IUploadService
{
    private readonly string _uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");

    public IQueryable<UploadViewModel> GetAllUploads()
    {
        return db.Uploads
            .OrderByDescending(u => u.DownloadCount)
            .Select(UploadViewModelProjection);
    }

    public IQueryable<UploadViewModel> GetAllUploadsByUserId(string userId)
    {
        return db.Uploads
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.UploadDate)
            .Select(UploadViewModelProjection);
    }

    public IEnumerable<UploadViewModel> Search(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<UploadViewModel>();

        return db.Uploads
            .Where(u => u.OriginalFileName.Contains(searchTerm) ||
                        u.FileName.Contains(searchTerm) ||
                        u.ContentType.Contains(searchTerm))
            .OrderByDescending(u => u.DownloadCount)
            .Select(UploadViewModelProjection);
    }

    public async Task<UploadViewModel?> UploadFileAsync(IFormFile file, string userId)
    {
        if (file == null || file.Length == 0) return null;

        if (!Directory.Exists(_uploadsFolder))
            Directory.CreateDirectory(_uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(_uploadsFolder, uniqueFileName);

        using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var upload = new Upload
        {
            OriginalFileName = file.FileName,
            FileName = uniqueFileName,
            ContentType = file.ContentType,
            Size = file.Length,
            UserId = userId,
            UploadDate = DateTime.Now
        };

        await db.Uploads.AddAsync(upload);
        await db.SaveChangesAsync();

        return await FindAsync(upload.ID, userId);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var selectedItem = await db.Uploads.FirstOrDefaultAsync(u => u.ID == id && u.UserId == userId);
        if (selectedItem != null)
        {
            var filePath = Path.Combine(_uploadsFolder, selectedItem.FileName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            db.Uploads.Remove(selectedItem);
            return await db.SaveChangesAsync() > 0;
        }
        return false;
    }

    public async Task<UploadViewModel?> FindAsync(int id, string userId)
    {
        return await db.Uploads
            .Where(u => u.ID == id && u.UserId == userId)
            .Select(UploadViewModelProjection)
            .FirstOrDefaultAsync();
    }

    public async Task<UploadViewModel?> FindAsync(string fileName)
    {
        return await db.Uploads
            .Where(u => u.FileName == fileName)
            .Select(UploadViewModelProjection)
            .FirstOrDefaultAsync();
    }

    public async Task IncrementDownloadCountAsync(string fileName)
    {
        await db.Uploads
            .Where(u => u.FileName == fileName)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.DownloadCount, u => u.DownloadCount + 1));
    }

    public async Task<int> GetUploadCountAsync()
    {
        return await db.Uploads.CountAsync();
    }

    #region Helper Projection

    private static readonly Expression<Func<Upload, UploadViewModel>> UploadViewModelProjection = u => new UploadViewModel
    {
        ID = u.ID,
        FileName = u.FileName,
        Size = u.Size,
        ContentType = u.ContentType,
        OriginalFileName = u.OriginalFileName,
        DownloadCount = u.DownloadCount,
        UploadDate = u.UploadDate
    };

    #endregion
}
