# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade H.Infrastructure\H.Infrastructure.csproj to .NET 10.0
4. Upgrade H.Content\H.Content.csproj to .NET 10.0
5. Upgrade H.Core\H.Core.csproj to .NET 10.0
6. Upgrade H.CLI\H.CLI.csproj to .NET 10.0
7. Upgrade H.Core.Test\H.Core.Test.csproj to .NET 10.0
8. Upgrade H.Infrastructure.Test\H.Infrastructure.Test.csproj to .NET 10.0
9. Upgrade H.CLI.Test\H.CLI.Test.csproj to .NET 10.0
10. Upgrade H.Integration\H.Integration.csproj to .NET 10.0
11. Run unit tests to validate upgrade in the projects listed below:
    - H.CLI.Test\H.CLI.Test.csproj
    - H.Core.Test\H.Core.Test.csproj
    - H.Infrastructure.Test\H.Infrastructure.Test.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                            | Current Version | New Version | Description                                   |
|:----------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| NETStandard.Library                     |     2.0.3       |             | Package functionality included with new framework reference |
| Newtonsoft.Json                         |     13.0.3      |   13.0.4    | Recommended for .NET 10.0                    |
| System.Configuration.ConfigurationManager | 8.0.0         |   10.0.1    | Recommended for .NET 10.0                    |
| System.Runtime.Caching                 |     9.0.4       |   10.0.1    | Recommended for .NET 10.0                    |
| System.Runtime.CompilerServices.Unsafe |     6.0.0       |   6.1.2     | Recommended for .NET 10.0                    |
| System.Threading.Tasks.Extensions      |     4.5.4       |             | Package functionality included with new framework reference |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### H.Infrastructure modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0-windows`

NuGet packages changes:
- NETStandard.Library should be removed (*functionality included with new framework reference*)

#### H.Content modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0`

NuGet packages changes:
- NETStandard.Library should be removed (*functionality included with new framework reference*)

#### H.Core modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0-windows`

NuGet packages changes:
- Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (*recommended for .NET 10.0*)
- System.Configuration.ConfigurationManager should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
- System.Runtime.Caching should be updated from `9.0.4` to `10.0.1` (*recommended for .NET 10.0*)
- System.Runtime.CompilerServices.Unsafe should be updated from `6.0.0` to `6.1.2` (*recommended for .NET 10.0*)
- NETStandard.Library should be removed (*functionality included with new framework reference*)

#### H.CLI modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0`

NuGet packages changes:
- System.Runtime.CompilerServices.Unsafe should be updated from `6.0.0` to `6.1.2` (*recommended for .NET 10.0*)
- System.Threading.Tasks.Extensions should be removed (*functionality included with new framework reference*)

#### H.Core.Test modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0`

NuGet packages changes:
- System.Runtime.CompilerServices.Unsafe should be updated from `6.0.0` to `6.1.2` (*recommended for .NET 10.0*)
- System.Threading.Tasks.Extensions should be removed (*functionality included with new framework reference*)

#### H.Infrastructure.Test modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0`

#### H.CLI.Test modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0`

NuGet packages changes:
- System.Runtime.CompilerServices.Unsafe should be updated from `6.0.0` to `6.1.2` (*recommended for .NET 10.0*)
- System.Threading.Tasks.Extensions should be removed (*functionality included with new framework reference*)

#### H.Integration modifications

Project properties changes:
- Convert project to SDK-style format
- Target framework should be changed from `.NET Framework 4.8` to `net10.0`

NuGet packages changes:
- System.Runtime.CompilerServices.Unsafe should be updated from `6.0.0` to `6.1.2` (*recommended for .NET 10.0*)
- System.Threading.Tasks.Extensions should be removed (*functionality included with new framework reference*)