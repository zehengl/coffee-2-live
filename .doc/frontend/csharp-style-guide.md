# Frontend C# Style Guide (Blazor)

- Project SDK: `Microsoft.NET.Sdk.BlazorWebAssembly`
- Target: `net8.0`
- `Nullable` enabled
- `ImplicitUsings` enabled

Style conventions:

- PascalCase for types, methods, properties, enums
- camelCase for locals and parameters
- File-scoped namespaces are used
- Constructor injection syntax is used for services
- Expression-bodied members are used for simple members

Notes:

- No `.editorconfig` or shared style config found in `blazor/`
- Frontend currently uses default SDK/IDE formatting and analyzer defaults
