# Backend C# Style Guide

- Target: `net8.0`
- `Nullable` enabled
- `ImplicitUsings` enabled
- Test project uses `LangVersion latest`

Style conventions:

- PascalCase for types, methods, properties, enums
- camelCase for locals and parameters
- Expression-bodied members are preferred for simple methods/properties

Notes:

- No `.editorconfig`, `Directory.Build.props`, or style settings were found in `dotnet/`
- Backend currently uses default SDK/IDE formatting
- Tests use NUnit and FluentAssertions
