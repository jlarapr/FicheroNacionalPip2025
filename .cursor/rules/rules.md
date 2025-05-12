# Coding Rules for WPF Clean Architecture App

## Language
C#

## Project Type
- WPF Desktop Application (.NET 8+)
- MVVM Pattern using CommunityToolkit.Mvvm
- Material Design in XAML Toolkit
- SQLite for local configuration
- SQL Server for central configuration

## Architecture
- Use 3-layered architecture:
  - Presentation (WPF UI)
  - Business (Domain logic, DTOs, Services)
  - Data (Repositories, EF Core, SQLite/SQL Server)

- Follow SOLID principles:
  - SRP: Each class should have a single responsibility
  - OCP: Classes should be extendable without modification
  - LSP: Derived classes must be substitutable for base types
  - ISP: Prefer small, specific interfaces
  - DIP: High-level modules must depend on abstractions

- Views should NOT contain logic (no code-behind)
- Use ViewModels with Commands and Observable Properties
- All services and repositories should use dependency injection via interfaces
- Use Result pattern for error handling and operation outcomes

## Error Handling & Result Pattern
- Use `Result<TSuccess, TFailure>` for operation outcomes
- Prefer Result over exceptions for expected error cases
- Methods should return Result instead of throwing exceptions when possible
- Use Result's Map/Bind/Match methods for functional composition
- Log all errors appropriately using ILogger
- Include meaningful error messages in Result.Failure

## File Organization
- Views → `Presentation/Views/`
- ViewModels → `Presentation/ViewModels/`
- Models → `Business/Models/`
- Services → `Business/Services/` and `Interfaces/`
- Repositories → `Data/Repositories/` and `Interfaces/`
- Shared config → `Data/Configuration/`
- Rules → `Business/Rules/`

## Naming Conventions
- Classes: `PascalCase`
- Interfaces: `I` prefix, e.g., `IUserService`
- Private fields: `_camelCase`
- Properties: `PascalCase`
- Commands: `Verb + Command`, e.g., `SaveCommand`
- ViewModels: `EntityNameViewModel`, e.g., `UserViewModel`
- Result types: `EntityNameResult`, e.g., `UserResult`

## Formatting and Style
- Indentation: 4 spaces
- Braces: always use `{}` even for single-line blocks
- Open braces go on a new line
- Use `var` only when the type is obvious
- Prefer expression-bodied members where appropriate
- Use records for immutable data models

## MVVM Guidelines
- ViewModel properties must raise `PropertyChanged`
- All UI logic should be bound to ViewModel
- Use `RelayCommand` or `AsyncRelayCommand` for UI actions
- Use `DataTemplates` to bind ViewModels to Views
- Do not access `Dispatcher` directly from ViewModel
- Handle errors using Result pattern in commands

## UI/XAML
- Use MaterialDesign styles and themes consistently
- Apply `TextElement.FontSize`, `FontFamily`, and `Foreground` via resource dictionaries
- Avoid hard-coded layout values in XAML (use `DynamicResource`, `Grid`, `StackPanel`)
- Use `UserControl` for reusable UI components
- Commands and bindings must have fallback values to avoid runtime errors
- Implement proper keyboard navigation and accessibility

## Testing
- Write unit tests for Business layer logic
- Use xUnit for testing framework
- Mock dependencies using Moq
- Test Result outcomes for both success and failure cases
- Write integration tests for critical workflows
- Test UI components using Microsoft.UI.Xaml.Testing

## Documentation & Comments
- Public classes/methods must use XML doc comments (`///`)
- Comments should explain "why", not "what"
- Avoid redundant comments (trust clean naming)
- Document all possible Result failure cases
- Include examples in complex method documentation

## Logging
- Use structured logging with Serilog
- Log all significant business operations
- Include correlation IDs for tracking operations
- Log appropriate levels (Debug, Info, Warning, Error)
- Do not log sensitive information
- Log Result failures appropriately

## Git & Versioning
- One feature per branch
- Follow semantic versioning in assemblies
- Use GitHub issues/PR templates for consistent team collaboration
- Include relevant ticket numbers in commit messages
- Keep commits focused and atomic

## Performance Guidelines
- Use async/await for I/O operations
- Implement virtualization for large collections
- Cache frequently accessed data appropriately
- Use ObservableCollection sparingly
- Profile memory usage regularly

## Cursor AI Usage
- Follow these rules when generating, refactoring, or explaining code
- Avoid suggesting `code-behind` logic
- Respect existing architecture and style
- Use Result pattern for error handling
- Maintain consistent naming conventions
