# C# Result

A C# Result Type to facilitate Railway Oriented Programming.

Latest Version: 0.2.0
# Getting Started

Install via Nuget
```bash
$ dotnet add package CSharp-Result --version 0.1.0
```

Install via Package Manager
```bash
Install-Package CSharp-Result -Version 0.1.0
```

# Usage
This library contains the Result Type, supporting extension methods, as well as integrations with Async and Collection based workflows.
It can be included in a project by using the relevant assembly:
```c#
using CSharp_Result;
```

More details about usage can be found in the [documentation](DOCUMENTATION.MD) and a deeper discussion of Railway Oriented Programming can be found at [FSharpForFunAndProfit](https://fsharpforfunandprofit.com/rop/).

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.MD) for details on our code of conduct, and the process for submitting pull requests to us.


## Versioning 
We use [SemVer](https://semver.org/) for versioning. For the versions available, see the tags on this repository.

## Changelog
| Version | Changes                                                                        |
|---|--------------------------------------------------------------------------------|
|0.2.0| Remove all nullable generic signature, allowing for notnullable enforcement |
|0.1.0| Rewrote Result using a struct, changed If to Assert, added new If expressions. |
|0.0.4| Override .Equals and .GetHashCode to compare wrapped types.                    |
|0.0.3| Add Get function to all Result Types.                                          |
|0.0.2| Added Documentation to Nuget Package.                                          |
|0.0.1| Initial Release.                                                               |

## Authors
* [Archeri2000](mailto:cheri.yuhann@outlook.com) 

## License
This project is licensed under MIT - see the [LICENSE.md](LICENSE.MD) file for details