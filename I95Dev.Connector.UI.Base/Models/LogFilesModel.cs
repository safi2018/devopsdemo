using System;

namespace I95Dev.Connector.UI.Base.Models
{
    public class LogFilesModel : BaseListModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public double FileSize { get; set; }
        public string FileType { get; set; }
    }
}