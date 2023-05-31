using AutoMapper;

namespace Filesharing.Helper.AutoMapper
{
	public class UploadProfile : Profile
	{
		public UploadProfile()
		{
			CreateMap<InputUpload, Upload>().ForMember(u=>u.UploadDate , op => op.Ignore());
			CreateMap<UploadViewModel , Upload>().ReverseMap();
		}
	}
}
