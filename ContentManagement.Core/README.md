# ContentManager

Interface implementation and tests
Packages used:
##Main
### Microsoft.NET.Test.Sdk(17.3.1);
### xunit(2.4.2);
### xunit.runner.visualstudio(2.4.5);
##Additional
### System.IO.Abstractions(17.2.1);
### System.IO.Abstractions.TagHelpers(17.2.1);

I. Cole logic in ContentManagement.Core

Interface and implementation of main business logic;
Helper classes - streamInfo, ConfigurationOptions;

II. Unit tests - ContentManagement.UnitTests

Build using xUnit, VS;
UnitTestsFixture - Sample dependencies for the main class (ContentManager) created here;
## If mocked filesystem is passed to constructor, the filesystem is mocked as described in the same file. If new FileSystem() is pased,
## then the file system used, is the real one;
File system is abstracted using System.IO.Abstractions package, for isolating it, and better testing;
ContentManagerUnitTests - unit tests for each of IContentManager's methods following the scheme:
### Method_Should_Return_X_If_Y;
Tests for user rights are added, though they are not yet passing;

III. Integration tests- ContentManagement.IntegrationTests

Build using xUnit, VS;
IntegrationTestsFixture - Sample dependencies for the main class (ContentManager) created here;
## If mocked filesystem is passed to constructor, the filesystem is mocked as described in the same file. If new FileSystem() is pased,
## then the file system used, is the real one;
File system is abstracted using System.IO.Abstractions package, for isolating it, and better testing;
ContentManagerIntegrationTests - integration tests for the following scenarios:
### I. User with rights 
[
	[checks] + [gets] X [file, bytes, hash],
	[checks] + [updates, stores] X [bytes] + [gets] X [file, bytes],
	[checks] + [deletes] X [bytes]
];
### II. User with rights
[
	[gets] X [file, bytes, hash] + [updates, stores, deletes] X [bytes] + [checks],
	[deletes] X [bytes] + [checks],
	[updates, stores] X [bytes] + [deletes] X [bytes] + [checks],
	[updates, stores] X [bytes] + [stores, updates] X [bytes] + [checks]
];
### III. User with rights
[
	[gets] X [file, bytes, hash] + [deletes] X [bytes] + [gets] X [file, bytes, hash],
	[updates, stores] X [bytes] + [gets] X [file, bytes, hash]
];
### IV. User with rights
[
	[deletes] X [bytes] + [updates, stores] X [bytes]
];
### V. User with rights
[
	[stores] X [bytes] + [get] X [bytes, file, hash] - checks for equality between return of different methods
];

Implemented are tests for matrixes I IV and V. II and III are in progress.

Intergration tests for rights currently are not developed. After unit tests for the same are resolved, this will be continued.