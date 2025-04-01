<div align="center">

# Contribution Guidelines

Thank you for taking the time to contribute to this project👏

</div>

The followings are the types of contributions we expect and the guidelines to follow.

## Reporting Bugs

- Make sure that you are using the latest version
- Go to [issues](https://github.com/mdgrs-mei/WinUIShell/issues) page and confirm that there is not already an existing bug report.
- File an issue and follow the `Bug Report` issue form.

## Suggesting Enhancements

If you are thinking about requesting new features or minor improvements to existing functionality, please follow the steps below:

- Read the [documentation](https://github.com/mdgrs-mei/WinUIShell/blob/main/README.md) carefully and confirm that the functionality is not covered already.
- Go to [issues](https://github.com/mdgrs-mei/WinUIShell/issues) page and confirm that there is not already an existing enhancement request.
- File an issue and follow the `Feature Request` issue form.

## Making Pull Requests

When you make a code contribution, you must agree that the content you submit is provided under the project [license](https://github.com/mdgrs-mei/WinUIShell/blob/main/LICENSE). To make a PR, please follow the steps below:

- File an issue by following the above guidelines before making a PR
- In the issue, suggest that you are willing to make a PR
- Fork this repository
- Create a branch in your forked repository with a meaningful name (e.g. add-file-manager-entry)
- Make the changes in the branch and push the commits
- Create a PR in the original repository

### Build and Test

Before building the project, you need .NET SDK installed. Follow the steps in the [Microsoft documentation](https://learn.microsoft.com/en-us/dotnet/core/install/) and install the latest SDK. After getting the access to the `donet` command, you can build the project by calling the build script:

```powershell
& .\Build.ps1
```

After the build, basic functionalities that are not dependent to the interactive part can be tested using Pester:

```powershell
& .\tests\RunPesterTests.ps1
```

For interactive testing on the console, we recommend that you install [RestartableSession](https://github.com/mdgrs-mei/RestartableSession) module. Once installed, you can set up a testing console by running this script:

```powershell
& .\tests\RestartableSession.ps1
```

This script builds the project and imports the built module. When you make a code modification, you can just call `restart` command to do the same process (build and import). The Pester test can be run by `pester` command.

### Coding

When you make a code modification, please follow the style of the existing code.
