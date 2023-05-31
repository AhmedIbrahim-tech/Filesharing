namespace Filesharing.Services.Interface
{
	public interface IUploadService
	{
		IEnumerable<UploadViewModel> GetAllUploadsAsync();

		IEnumerable<UploadViewModel> GetAllUploadsbyUserIDAsync(string userId);

		IEnumerable<UploadViewModel> Search(string term);
		Task Create(InputUpload model);
		Task<UploadViewModel> Find(int id, string userid);
		Task<UploadViewModel> Find(string id);
		Task Delete(int id, string userid);
		Task IncreamentDownloadCount(string id);
		Task<int> GetUploadCount();
	}
}
