using AutoMapper;

namespace Filesharing.Helper.AutoMapper
{
    public class UploadProfile : Profile
    {
        public UploadProfile()
        {
            CreateMap<InputUpload, Upload>()
			.ForMember(u => u.UploadDate, op => op.Ignore())
			.ForMember(u => u.ID , op => op.Ignore())
			.ReverseMap();
            CreateMap<UploadViewModel, Upload>().ReverseMap();
        }
    }
}
