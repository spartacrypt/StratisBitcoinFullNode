| Windows | Linux | OS X
| :---- | :------ | :---- |
[![Windows build status][1]][2] | [![Linux build status][3]][4] | [![OS X build status][5]][6] | 

[1]: https://ci.appveyor.com/api/projects/status/451tv98n7xvxm5ol/branch/master?svg=true
[2]: https://ci.appveyor.com/project/stratis/stratisbitcoinfullnode
[3]: https://travis-ci.org/stratisproject/StratisBitcoinFullNode.svg?branch=master
[4]: https://travis-ci.org/stratisproject/StratisBitcoinFullNode
[5]: https://travis-ci.org/stratisproject/StratisBitcoinFullNode.svg?branch=master
[6]: https://travis-ci.org/stratisproject/StratisBitcoinFullNode


Redstone Core
===============

https://redstoneplatform.com

Redstone is an implementation of the Bitcoin protocol in C# on the [.NET Core](https://dotnet.github.io/) platform.    
Redstone is a Stratis fork based on the [NBitcoin](https://github.com/MetacoSA/NBitcoin) project.  

For Proof of Stake support on the Redstone token the node is using [NStratis](https://github.com/stratisproject/NStratis) which is a POS implementation of NBitcoin.  

[.NET Core](https://dotnet.github.io/) is an open source cross platform framework and enables the development of applications and services on Windows, macOS and Linux.  

Join our community on [discord](https://discord.gg/BCSX854) or [telegram](https://t.me/redstoneplatform).  

Development
-----------
Up for some blockchain development?

Check this guides for more info:
* [Contributing Guide](Documentation/contributing.md)
* [Coding Style](Documentation/coding-style.md)

There is a lot to do and we welcome contributers developers and testers who want to get some Blockchain experience.
You can find tasks at the issues/projects or visit the channel on [discord](https://discord.gg/BCSX854)

Testing
-------
* [Testing Guidelines](Documentation/testing-guidelines.md)

CI build
-----------

We use [AppVeyor](https://www.appveyor.com/) for our CI build and to create nuget packages.
Every time someone pushes to the master branch or create a pull request on it, a build is triggered and new nuget packages are created.

To skip a build, for example if you've made very minor changes, include the text **[skip ci]** or **[ci skip]** in your commits' comment (with the squared brackets).
