namespace CursorShops.Models
{
    public class ToolBarModel
    {
        public bool AttachFile { get; set; } = false;
        public bool CancelTask { get; set; } = false;
        public bool SaveTask { get; set; } = false;
        public bool CompleteTask { get; set; } = false;
        public bool ReturnTask { get; set; } = false;
        public bool QuestionTask { get; set; } = false;
        public bool PrintTask { get; set; } = false;
        public string TaskModuleName { get; set; }
        public string CurrentUserName {get; set;}
        public string Status { get; set; }

    }
}
