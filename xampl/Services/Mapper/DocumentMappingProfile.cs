using AutoMapper;
using xampl.Models.Documents;
using xampl.ViewModels;

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
