# GitChangelog

GitChangelog is a simple command-line tool that generates a changelog based on the commit history of multiple Git repositories within a specified date range. Additionally, it provides information about projects with uncommitted changes and details about branches updated within the date range.

## Features

- Generate a consolidated changelog (from local commits only) for multiple Git repositories.
- Identify projects with uncommitted changes.
- View information about branches (local and remote) updated within a specified date range, or the most recent branch.

## Getting Started

1. Clone the repository to your local machine.
2. Configure your settings in `appsettings.json` or enter them interactively if appsettings.json does not exist.
3. Run `GitChangelog.exe` to generate the changelog and view project information.

## Configuration

Modify the `appsettings.json` file to customize your configuration options:

```json
{
  "Username": "<your username>",
  "ChangelogPath": "c:\\Path\\To\\Your\\Desired\\Output\\changelog.txt",
  "PendingChangesPath": "c:\\Path\\To\\Your\\Desired\\Output\\pending_changes.txt",
  "RecentBranchesPath": "c:\\Path\\To\\Your\\Desired\\Output\\recent_branches.txt",
  "RepositoriesPath": "c:\\Path\\To\\Your\\Repositores\\",
  "FromDate": "<date in yyyy-MM-dd format>",
  "ToDate": "<date in yyyy-MM-dd format>",
  "OutputDestination": "<One of 'Console', 'File', or 'ConsoleAndFile'>'"
}
```

- **Username**: Your Git username for filtering commit history.
- **ChangelogPath**: Output path for the consolidated changelog.
- **PendingChangesPath**: Output path for projects with uncommitted changes.
- **RepositoriesPath**: Path to the directory containing your Git repositories.
- **FromDate**: Start date for the changelog (local) and branches (loacl and remote) (format: yyyy-MM-dd).
- **ToDate**: End date for the changelog (local) and branches (loacl and remote) (format: yyyy-MM-dd).
- **OutputDestination**: Choose the output destination (Console, File, ConsoleAndFile).

## Usage

Run `GitChangelog.exe` from the command line. Follow the prompts to provide configuration options if `appsettings.json` is not found.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.