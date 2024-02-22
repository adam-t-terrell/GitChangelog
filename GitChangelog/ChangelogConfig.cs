using Newtonsoft.Json;
using System;

namespace GitChangelog
{
    public class ChangelogConfig
    {
        [JsonProperty(nameof(Username))]
        public string Username { get; set; } = string.Empty;

        [JsonProperty(nameof(ChangelogPath))]
        public string ChangelogPath { get; set; } = string.Empty;

        [JsonProperty(nameof(PendingChangesPath))]
        public string PendingChangesPath { get; set; } = string.Empty;

        [JsonProperty(nameof(RecentBranchesPath))]
        public string RecentBranchesPath { get; set; } = string.Empty;

        [JsonProperty(nameof(RepositoriesPath))]
        public string RepositoriesPath { get; set; } = string.Empty;

        [JsonProperty(nameof(FromDate))]
        public DateTime FromDate { get; set; } = DateTime.MinValue;

        [JsonProperty(nameof(ToDate))]
        public DateTime ToDate { get; set; } = DateTime.MaxValue;

        [JsonProperty(nameof(OutputDestination))]
        public OutputDestination OutputDestination { get; set; } = OutputDestination.Console;
    }

    public enum OutputDestination
    {
        Console,
        File,
        ConsoleAndFile
    }
}
