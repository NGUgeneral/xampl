using AutoMapper;
using xampl.Models.DTO;
using xampl.Models.ViewModels;

namespace xampl.Services.Mapper
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {
            CreateMap<UserDto, UserVM>().ReverseMap();
            CreateMap<DocumentDto, DocumentVM>().ReverseMap();
            CreateMap<DocumentNoteDto, DocumentNoteVM>().ReverseMap();
            CreateMap<DocumentListDto, DocumentListVM>().ReverseMap();
            CreateMap<DocumentListItemDto, DocumentListItemVM>().ReverseMap();
        }
    }
}
