using AutoMapper;
using xampl.Models.Documents;
using xampl.ViewModels;

namespace xampl.Services.Mapper
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {
            CreateMap<User, UserVM>().ReverseMap();
            CreateMap<Document, DocumentVM>().ReverseMap();
            CreateMap<DocumentNote, DocumentNoteVM>().ReverseMap();
            CreateMap<DocumentList, DocumentListVM>().ReverseMap();
            CreateMap<DocumentListItem, DocumentListItemVM>().ReverseMap();
        }
    }
}
