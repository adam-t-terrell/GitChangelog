using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GitChangelog
{
    public class GitChangelog
    {
        public static void Main()
        {
            var configFilePath = "appsettings.json";

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("appsettings.json not found. Please provide configuration options:");

                var newChangelogConfig = GetChangelogConfigFromConsole();

                // Save the provided configuration to appsettings.json
                SaveChangelogConfigToFile(newChangelogConfig, configFilePath);
            }

            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var changelogConfig = configuration.Get<ChangelogConfig>();

            if (changelogConfig != null)
            {
                Console.WriteLine($"---- Processing Changelog for all projects in {changelogConfig.RepositoriesPath} ----");
                string changelog = GitCommandProcessor.GetChangelogForAllProjects(changelogConfig);
                OutputChanges(changelog, changelogConfig.ChangelogPath, changelogConfig.OutputDestination);
                Console.WriteLine($"---- Done processing ----{Environment.NewLine}");

                Console.WriteLine($"---- Processing projects with pending changes in {changelogConfig.RepositoriesPath} ----");
                string pendingchanges = GitCommandProcessor.GetProjectsWithPendingChanges(changelogConfig);
                OutputChanges(pendingchanges, changelogConfig.PendingChangesPath, changelogConfig.OutputDestination);
                Console.WriteLine($"---- Done processing ----{Environment.NewLine}");

                Console.WriteLine($"---- Processing Recent Branches for all projects in {changelogConfig.RepositoriesPath} ----"); 
                string branchesInfo = GitCommandProcessor.GetBranchesUpdatedWithinDateRange(changelogConfig);
                OutputChanges(branchesInfo, changelogConfig.RecentBranchesPath, changelogConfig.OutputDestination);
                Console.WriteLine($"---- Done processing ----{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine("Error: ChangelogConfig is null. Please check your configuration.");
            }

            WaitForExit();
        }

        private static ChangelogConfig GetChangelogConfigFromConsole()
        {
            var changelogConfig = new ChangelogConfig
            {
                Username = ReadConsoleInput("Enter your username: ", input => !string.IsNullOrWhiteSpace(input)),
                ChangelogPath = ReadConsoleInput("Enter the output path for the changelog: ", input => !string.IsNullOrWhiteSpace(input)),
                PendingChangesPath = ReadConsoleInput("Enter the output path for any pending changes in projects: ", input => !string.IsNullOrWhiteSpace(input)),
                RepositoriesPath = ReadConsoleInput("Enter the path to your repositories: ", input => !string.IsNullOrWhiteSpace(input)),

                // Prompt for date range
                FromDate = ReadConsoleDate("Enter the start date for the changelog (yyyy-MM-dd): "),
                ToDate = ReadConsoleDate("Enter the end date for the changelog (yyyy-MM-dd): "),

                OutputDestination = Enum.TryParse<OutputDestination>(ReadConsoleInput("Choose the output destination (1: Console, 2: File, 3: ConsoleAndFile): ",
                    input => int.TryParse(input, out int choice) && Enum.IsDefined(typeof(OutputDestination), choice)), out var outputDestination)
                ? outputDestination
                : OutputDestination.Console
            };

            return changelogConfig;
        }

        private static string ReadConsoleInput(string prompt, Func<string, bool> validator)
        {
            string userInput;

            do
            {
                Console.Write(prompt);
                userInput = Console.ReadLine() ?? string.Empty;

                if (userInput != null && !validator(userInput))
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }

            } while (userInput != null && !validator(userInput));

            return userInput ?? string.Empty; // Return an empty string if userInput is null
        }

        private static DateTime ReadConsoleDate(string prompt)
        {
            do
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (DateTime.TryParse(input, out DateTime date))
                {
                    return date;
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please enter a valid date (yyyy-MM-dd).");
                }

            } while (true);
        }

        private static void SaveChangelogConfigToFile(ChangelogConfig changelogConfig, string filePath)
        {
            string json = JsonConvert.SerializeObject(changelogConfig, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Configuration saved to {filePath}");
        }

        private static void OutputChanges(string changes, string outputPath, OutputDestination outputDestination)
        {
            if (outputDestination == OutputDestination.Console || outputDestination == OutputDestination.ConsoleAndFile)
            {
                Console.WriteLine(changes);
            }

            if (outputDestination == OutputDestination.File || outputDestination == OutputDestination.ConsoleAndFile)
            {
                // Save the changelog to a file
                File.WriteAllText(outputPath, changes);
                Console.WriteLine($"Changelog saved to {outputPath}");
            }
        }

        public static void WaitForExit(bool showPressAnyKey = true)
        {
            if (showPressAnyKey)
            {
                Console.WriteLine("Press any key to close the app");
            }

            Console.ReadKey();
        }
    }
}
