namespace Filesharing.Services.Interface
{
	public interface IUploadService
	{
		//IEnumerable<UploadViewModel> GetAllUploadsAsync();
		//IEnumerable<UploadViewModel> GetAllUploadsbyUserIDAsync(string userId);
		//IQueryable<UploadViewModel> Search(string term);
		IEnumerable<UploadViewModel> Search(string term);
		IQueryable<UploadViewModel> GetAllUploads();
		IQueryable<UploadViewModel> GetAllUploadsbyUserID(string userId);


		Task CreateAsync(InputUpload model);
		Task<UploadViewModel> FindAsync(int id, string userid);
		Task<UploadViewModel> FindAsync(string id);
		Task DeleteAsync(int id, string userid);
		Task IncreamentDownloadCountAsync(string id);
		Task<int> GetUploadCountAsync();
	}
}
