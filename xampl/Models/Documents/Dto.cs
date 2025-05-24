namespace xampl.Models.Documents
{
    // DO NOT USE IT AS WRAPPER FOR ADDITIONAL FUNCTIONALITY,
    // MOST LIKELY YOU ARE LOOKING FOR VIEW MODELS.
    //
    // THE SOLE INTENTION OF THIS CLASSES IS UPDATING DTO OBJECT
    // NAMES SINCE EF CORE POWERTOOLS CAN NAME THEM ONLY
    // EXACTLY AS THE NAME OF THE TABLES.

    public class DocumentDto : Document;

    public class DocumentListDto : DocumentList;

    public class DocumentListItemDto : DocumentListItem;

    public class DocumentNoteDto : DocumentNote;

    public class UserDto : User;
}
