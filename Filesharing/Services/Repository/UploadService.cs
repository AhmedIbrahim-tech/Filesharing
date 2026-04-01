using Filesharing.Models;
using Filesharing.Services.Interface;

namespace Filesharing.Services.Repository
{

    #region Contractor

    public class UploadService : IUploadService
    {
        private readonly ApplicationDBContext db;

        public UploadService(ApplicationDBContext db)
        {
            this.db = db;
        }

        #endregion

        #region Get All Uploads
        public IQueryable<UploadViewModel> GetAllUploads()
        {
            var result = db.Uploads
                    .OrderByDescending(u => u.DownloadCount)
                    .Select(u => new UploadViewModel
                    {
                        ID = u.ID,
                        FileName = u.FileName,
                        Size = u.Size,
                        ContentType = u.ContentType,
                        OriginalFileName = u.OriginalFileName,
                        DownloadCount = u.DownloadCount,
                        UploadDate = u.UploadDate
                    });
            return result;
        }

        #endregion

        #region Get All Uploads by UserID

        public IQueryable<UploadViewModel> GetAllUploadsbyUserID(string userId)
        {
            var result = db.Uploads.Where(x => x.UserId == userId).OrderByDescending(a => a.UploadDate)
                .Select(u => new UploadViewModel
                {
                    ID = u.ID,
                    FileName = u.FileName,
                    Size = u.Size,
                    ContentType = u.ContentType,
                    OriginalFileName = u.OriginalFileName,
                    DownloadCount = u.DownloadCount,
                    UploadDate = u.UploadDate
                });
            return result;
        }

        #endregion

        #region Search

        public IEnumerable<UploadViewModel> Search(string term)
        {
            var Model = db.Uploads
                          .Where(
                            U => U.OriginalFileName.Contains(term) || 
                            U.FileName.Contains(term) || 
                            U.ContentType.Contains(term)
                            )
                          .OrderByDescending(x => x.DownloadCount)
                          .Select(u => new UploadViewModel
                          {
                              ID = u.ID,
                              FileName = u.FileName,
                              Size = u.Size,
                              ContentType = u.ContentType,
                              OriginalFileName = u.OriginalFileName,
                              DownloadCount = u.DownloadCount,
                              UploadDate = u.UploadDate
                          });
            return Model;
        }

        #endregion

        #region Create

        public async Task CreateAsync(InputUpload model)
        {
            var uploadObject = new Upload
            {
                OriginalFileName = model.OriginalFileName,
                FileName = model.FileName,
                ContentType = model.ContentType,
                Size = model.Size,
                UserId = model.UserId,
                UploadDate = DateTime.Now
            };

            await db.Uploads.AddAsync(uploadObject);
            await db.SaveChangesAsync();
        }

        #endregion

        #region Delete

        public async Task DeleteAsync(int id, string userid)
        {
            try
            {
                var SelectedItem = await db.Uploads.FirstOrDefaultAsync(x => x.ID == id && x.UserId == userid);
                if (SelectedItem != null)
                {
                    db.Uploads.Remove(SelectedItem);
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {

                throw new(ex.Message);
            }
        }

        #endregion

        #region Find By (Id , user-id)

        public async Task<UploadViewModel?> FindAsync(int id, string userid)
        {
            // This's Code Be., Not AnyOne Delete Item From Another User 
            var SelectedItem = await db.Uploads.FirstOrDefaultAsync(x => x.ID == id && x.UserId == userid);
            if (SelectedItem != null)
            {
                return new UploadViewModel
                {
                    ID = SelectedItem.ID,
                    ContentType = SelectedItem.ContentType,
                    FileName = SelectedItem.FileName,
                    OriginalFileName = SelectedItem.OriginalFileName,
                    Size = SelectedItem.Size,
                    UploadDate = SelectedItem.UploadDate,
                    DownloadCount = SelectedItem.DownloadCount
                };
            }
            return null;
        }



        public async Task<UploadViewModel?> FindAsync(string id)
        {
            // This's Code Be., Not AnyOne Delete Item From Another User 
            var SelectedItem = await db.Uploads.FirstOrDefaultAsync(x=>x.FileName == id);
            if (SelectedItem != null)
            {
                return new UploadViewModel
                {
                    ID = SelectedItem.ID,
                    ContentType = SelectedItem.ContentType,
                    FileName = SelectedItem.FileName,
                    OriginalFileName = SelectedItem.OriginalFileName,
                    Size = SelectedItem.Size,
                    UploadDate = SelectedItem.UploadDate,
                    DownloadCount = SelectedItem.DownloadCount
                };
            }
            return null;
        }

        #endregion

        #region Increment Download Count

        public async Task IncreamentDownloadCountAsync(string id)
        {
            var SelectedItem = await db.Uploads.FirstOrDefaultAsync(x => x.FileName == id);
            if (SelectedItem != null)
            {
                // Counter for Download 
                SelectedItem.DownloadCount++;

                db.Update(SelectedItem);
                await db.SaveChangesAsync();
            }
        }

        #endregion

        #region GetUploadCount

        public async Task<int> GetUploadCountAsync()
        {
            return await db.Uploads.CountAsync();
        }

        #endregion

        #region

    }
}

#endregion