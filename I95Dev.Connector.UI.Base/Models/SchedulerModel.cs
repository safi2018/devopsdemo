using System;

namespace I95Dev.Connector.UI.Base.Models
{
    public class SchedulerModel
    {
        public string Author { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public string Path { get; set; }

        public string WorkingDirectory { get; set; }

        public short DaysInterval { get; set; }

        public string TaskFolder { get; set; }

        public TimeSpan RepetitionInterval { get; set; }

        public string Arguments { get; set; }

        public string Password { get; set; }
    }
}