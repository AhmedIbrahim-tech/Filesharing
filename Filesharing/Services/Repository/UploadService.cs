#region Fields
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Filesharing.Models;
using Filesharing.Services.Interface;
using Microsoft.AspNetCore.Hosting;

namespace Filesharing.Services.Repository
{

    #endregion

    #region Contractor

    public class UploadService : IUploadService
    {
        private readonly ApplicationDBContext db;
        private readonly IMapper _mapper;

        public UploadService(ApplicationDBContext db, IMapper mapper)
        {
            this.db = db;
            _mapper = mapper;
        }

        #endregion

        #region Get All Uploads
        public IEnumerable<UploadViewModel> GetAllUploadsAsync()
        {
            var Model = db.Uploads
                    .OrderByDescending(u => u.DownloadCount).ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider);
            //.Select(u => new UploadViewModel
            //{
            //	FileName = u.FileName,
            //	Size = u.Size,
            //	ContentType = u.ContantType,
            //	OriginalFileName = u.OriginalFileName,
            //	DownloadCount = u.DownloadCount,
            //}).ToListAsync();
            return Model;
        }

        #endregion

        #region Get All Uploads by UserID

        public IEnumerable<UploadViewModel> GetAllUploadsbyUserIDAsync(string userId)
        {
            var result = db.Uploads.Where(x => x.UserId == userId).OrderByDescending(a => a.UploadDate)
                .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider);
            return result;
        }

        #endregion

        #region Search

        public IEnumerable<UploadViewModel> Search(string term)
        {
            var Model = db.Uploads
                          .Where(U => U.OriginalFileName.Contains(term)
                            || U.FileName.Contains(term) || U.ContentType.Contains(term))
                          .OrderByDescending(x => x.DownloadCount)
                          .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider);
            return Model;
        }

        #endregion

        #region Create

        public async Task Create(InputUpload model)
        {
            var MapperObj = _mapper.Map<Upload>(model);
            //var inputupload = new Upload()
            //{
            //	OriginalFileName = model.FileName,
            //	FileName = model.FileName,
            //	ContantType = model.ContentType,
            //	Size = model.Size,
            //	UserId = model.UserId,
            //	UploadDate = DateTime.Now
            //};

            await db.Uploads.AddAsync(MapperObj);
            await db.SaveChangesAsync();
        }

        #endregion

        #region Delete

        public async Task Delete(int id, string userid)
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

        public async Task<UploadViewModel> Find(int id, string userid)
        {
            // This's Code Be., Not AnyOne Delete Item From Another User 
            var SelectedItem = await db.Uploads.FirstOrDefaultAsync(x => x.ID == id && x.UserId == userid);
            if (SelectedItem != null)
            {
                var obj = _mapper.Map<Upload , UploadViewModel>(SelectedItem);
                return obj;
                //var dto = new UploadViewModel
                //{
                //	ID = SelectedItem.ID,
                //	ContentType = SelectedItem.ContantType,
                //	FileName = SelectedItem.FileName,
                //	OriginalFileName = SelectedItem.OriginalFileName,
                //	Size = SelectedItem.Size,
                //	UploadDate = SelectedItem.UploadDate,
                //	DownloadCount = SelectedItem.DownloadCount

                //};
            }
            return null;
        }



        public async Task<UploadViewModel> Find(string id)
        {
            // This's Code Be., Not AnyOne Delete Item From Another User 
            var SelectedItem = await db.Uploads.FirstOrDefaultAsync(x=>x.FileName == id);
            if (SelectedItem != null)
            {
                return _mapper.Map<UploadViewModel>(SelectedItem);
                //var dto = new UploadViewModel
                //{
                //	ID = SelectedItem.ID,
                //	ContentType = SelectedItem.ContantType,
                //	FileName = SelectedItem.FileName,
                //	OriginalFileName = SelectedItem.OriginalFileName,
                //	Size = SelectedItem.Size,
                //	UploadDate = SelectedItem.UploadDate,
                //	DownloadCount = SelectedItem.DownloadCount

                //};
                //return dto;
            }
            return null;
        }

        #endregion

        #region Increment Download Count

        public async Task IncreamentDownloadCount(string id)
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

        public async Task<int> GetUploadCount()
        {
            return await db.Uploads.CountAsync();
        }

        #endregion


        #region

    }
}

#endregion