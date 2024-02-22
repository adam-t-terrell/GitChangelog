using System.Diagnostics;

namespace GitChangelog
{
    public class GitCommandProcessor
    {
        public static string GetChangelogForAllProjects(ChangelogConfig config)
        {
            var changelog = new System.Text.StringBuilder();

            // Get the list of repositories in the specified path
            var repositories = Directory.GetDirectories(config.RepositoriesPath);

            foreach (var repository in repositories)
            {
                string repositoryName = new DirectoryInfo(repository).Name;

                // Run git log command to get the commit history within the specified date range
                string gitLogCommand = $"git log --all --author={config.Username} --pretty=format:\"%h - %s (%ad)\" --date=local " +
                       $"--since='{config.FromDate:yyyy-MM-dd}' --until='{config.ToDate:yyyy-MM-dd}'";

                // Read the output of the git log command
                string output = RunGitCommand(gitLogCommand, repository);

                // Skip if no commits are found
                if (string.IsNullOrWhiteSpace(output))
                {
                    continue;
                }

                // Append the changelog for the current repository
                changelog.AppendLine($"# {repositoryName}");
                changelog.AppendLine(output);
                changelog.AppendLine();
            }

            return changelog.ToString();
        }

        public static string GetProjectsWithPendingChanges(ChangelogConfig config)
        {
            var pendingchanges = new System.Text.StringBuilder();

            // Get the list of repositories in the specified path
            var repositories = Directory.GetDirectories(config.RepositoriesPath);

            pendingchanges.AppendLine("Projects with uncommitted changes:");

            foreach (var repository in repositories)
            {
                string repositoryName = new DirectoryInfo(repository).Name;

                // Check for uncommitted changes using git status
                string gitStatusCommand = "git status --short";
                
                // Read the output of the git status command
                string statusOutput = RunGitCommand(gitStatusCommand, repository);

                // Display the project name if uncommitted changes are found
                if (!string.IsNullOrWhiteSpace(statusOutput))
                {
                    pendingchanges.AppendLine($"- {repositoryName}");
                }
            }

            return pendingchanges.ToString();
        }

        public static string GetBranchesUpdatedWithinDateRange(ChangelogConfig config)
        {
            var branchesInfo = new System.Text.StringBuilder();

            // Get the list of repositories in the specified path
            var repositories = Directory.GetDirectories(config.RepositoriesPath);

            branchesInfo.AppendLine($"Branches updated within the date range {config.FromDate:yyyy-MM-dd} - {config.ToDate:yyyy-MM-dd}:");

            foreach (var repository in repositories)
            {
                string repositoryName = new DirectoryInfo(repository).Name;

                // Run git for-each-ref command to get information about branches within the specified date range
                string gitBranchCommand = $"git for-each-ref --sort=-committerdate --since='{config.FromDate:yyyy-MM-dd}' --until='{config.ToDate:yyyy-MM-dd}' refs/heads/ refs/remotes/ --format=\"%(HEAD) %(refname:short) - %(committerdate:relative) (%(committerdate:local))\"";
                string output = RunGitCommand(gitBranchCommand, repository);

                if (string.IsNullOrWhiteSpace(output))
                {
                    // If there are no branches that have been updated within the date range, show the most recent branch
                    gitBranchCommand = $"git for-each-ref --sort=-committerdate --count=1 refs/heads/ refs/remotes/ --format=\"%(HEAD) %(refname:short) - %(committerdate:relative) (%(committerdate:local))\"";
                    output = RunGitCommand(gitBranchCommand, repository);
                }

                // Skip if no branches are found
                if (string.IsNullOrWhiteSpace(output))
                {
                    continue;
                }

                // Append the branch information for the current repository
                branchesInfo.AppendLine($"# {repositoryName}");
                branchesInfo.AppendLine(output);
                branchesInfo.AppendLine();
            }

            return branchesInfo.ToString();
        }

        private static string RunGitCommand(string gitCommandText, string repoDirectory)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = repoDirectory,
                Arguments = $"/c {gitCommandText}"
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            // Read the output of the git command
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }
}
