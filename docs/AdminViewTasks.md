# Admin View Tasks

## Overview
This document outlines the tasks and changes related to the `AdminViewModel` and `AdminWindow` in the `FicheroNacionalPip.Presentation` project. These tasks are aimed at enhancing the functionality and user experience of the Admin view.

## Tasks

### 1. ViewModel Enhancements
- [ ] **File**: `AdminViewModel.cs`
- [ ] **Location**: `FicheroNacionalPip.Presentation\ViewModels\RightMenu`
- [ ] **Description**: The `AdminViewModel` is responsible for managing the state and logic of the Admin view. It currently includes a property `MyTitle` initialized to "Admin".
- [ ] **Future Enhancements**:
  - [ ] Add properties for managing user data, such as `CbxUsers`, `TxtVisiblePasswordbox`, and other bindings used in the view.
  - [ ] Implement commands for buttons like `BtnAdd`, `BtnEdit`, `BtnDelete`, etc.

### 2. View Design and Bindings
- [ ] **File**: `AdminWindow.xaml`
- [ ] **Location**: `FicheroNacionalPip.Presentation\Views\RightMenu`
- [ ] **Description**: The `AdminWindow` defines the UI for the Admin view. It includes various controls such as `ComboBox`, `PasswordBox`, `TextBox`, `CheckBox`, and `Button`.
- [ ] **Future Enhancements**:
  - [ ] Ensure all bindings are correctly implemented in the `AdminViewModel`.
  - [ ] Add validation and error handling for user inputs.
  - [ ] Improve the UI design for better usability.

### 3. Data Context and Integration
- [ ] **Description**: The `AdminWindow` uses `AdminViewModel` as its DataContext. Ensure seamless integration between the View and ViewModel.
- [ ] **Future Enhancements**:
  - [ ] Connect the ViewModel to backend services for fetching and updating user data.
  - [ ] Implement dependency injection for better testability.

### 4. Commands and Interactions
- [ ] **Description**: The buttons in the `AdminWindow` (e.g., `Add`, `Edit`, `Delete`, `Save`, `Cancel`) need to be wired to commands in the `AdminViewModel`.
- [ ] **Future Enhancements**:
  - [ ] Define and implement commands in the ViewModel.
  - [ ] Add logic for handling button actions, such as adding a new user or saving changes.

### 5. Validation and Security
- [ ] **Description**: The view includes fields for passwords and user information. Ensure proper validation and security measures are in place.
- [ ] **Future Enhancements**:
  - [ ] Add password strength validation.
  - [ ] Mask sensitive information in the UI.
  - [ ] Implement role-based access control for different sections of the view.

### Principles to Follow
- **SOLID Principles**: Ensure the code adheres to SOLID principles to guarantee a robust and flexible architecture.
- **Clean Code**: Maintain clean, readable, and maintainable code.

### Architecture
- **Project Layers**:
  - **Data**: Responsible for database interaction and data persistence.
  - **Business**: Contains business logic and application rules.
  - **Presentation**: Handles the user interface and user interaction.

## Notes
- The `AdminViewModel` and `AdminWindow` are part of the `RightMenu` module in the `FicheroNacionalPip.Presentation` project.
- Follow MVVM best practices for implementing bindings and commands.
- Ensure all changes are tested thoroughly before deployment.

## References
- [MVVM Pattern Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/mvvm-overview?view=netframeworkdesktop-4.8)
- [Material Design in XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
