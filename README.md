# Task Manager Pro

## 🌟 Project Introduction

Task Manager Pro is a powerful task management application designed to help individuals and teams organize work, track progress, and enhance productivity. The application consists of a backend built with a modern microservice architecture and a WPF desktop client application for user interaction.

## 🛠️ Backend Architecture and Technology

The backend of Task Manager Pro is built with scalability, maintainability, and high performance in mind.

### General Architecture:
* **Microservices**: The system is divided into small, independent services, each responsible for a specific business domain.
* **Clean Architecture**: Adheres to Clean Architecture principles to separate concerns, making the code understandable, testable, and maintainable.
* **CQRS (Command Query Responsibility Segregation)**: Separates data read (Queries) and write (Commands) logic to optimize performance and scalability for each type of operation.

### Main Microservices:
1.  **IdentityService**: Manages user authentication, role (login, registration, tokens).
2.  **UserService**: Manages user (staff) information and user-related operations.
3.  **TaskService**: Manages tasks, including creation, updates, deletion, assignment, and status tracking.
4.  **ApiGateway**: A single entry point for all client requests, routing them to the appropriate microservices.
5.  **SignalRService**: Responsible for real-time communication between the server and clients (e.g., new task notifications).

### Inter-Service Communication:
* **gRPC**: Used for synchronous, high-performance communication requiring quick responses between internal services.
* **MassTransit integrated with RabbitMQ**: Utilized for asynchronous communication that doesn't require immediate feedback (e.g., event processing, background notifications), ensuring reliability and resilience.

### Backend Technologies:
* **.NET 8**: The primary development platform.
* **SQL Server**: Database management system, hosted as a Docker container for easy deployment and management.
* **Docker**: Used for containerizing SQL Server and RabbitMQ.
* **SignalR**: For real-time features.

## 🖥️ Frontend Application (WPF Desktop)

The client application is built using WPF, following the MVVM (Model-View-ViewModel) architecture and Object-Oriented Programming (OOP) principles to ensure well-structured, testable, and maintainable code.

### Core Frontend Principles:

* **Object-Oriented Programming (OOP)**:
    * The frontend leverages OOP by organizing code into classes that represent real-world entities (Models like `User`, `TaskItem`) or application components (ViewModels, Services).
    * **Encapsulation** is used to bundle data and behavior.
    * **Inheritance** is seen with `ViewModelBase` providing common functionality (like `INotifyPropertyChanged`, `INotifyDataErrorInfo`) to specific ViewModels.
    * **Abstraction** is achieved through interfaces for services (e.g., `IUserService`).

* **Model-View-ViewModel (MVVM)**: This pattern separates the UI (View) from the application logic and data (ViewModel and Model).
    * **Model**: Represents data (e.g., `User`, `TaskItem`) and business logic (often encapsulated in services).
    * **View**: The XAML files (`LoginView.xaml`, `ProfileView.xaml`, etc.) define the UI structure and appearance. It binds to ViewModel properties and commands.
    * **ViewModel**: (`LoginViewModel.cs`, `ProfileViewModel.cs`, etc.) acts as a bridge. It prepares data from the Model for the View and handles user interactions via Commands. It implements `INotifyPropertyChanged` (via `ViewModelBase`) for UI updates and `INotifyDataErrorInfo` for validation.
        * **Data Binding**: Connects View elements to ViewModel properties.
        * **Commands**: (`RelayCommand`) Handle user actions (e.g., button clicks) in the ViewModel.

### Key Features:
* **Modern User Interface**:
    * Login page designed with a 2-column layout (Welcome/Intro and Login Form), utilizing colors and gradients.
    * Redesigned Navbar with updated colors, shadow effects, and a clear active tab indicator.
    * Profile page features a 2-column layout (Account Information and Security) to optimize space utilization.
* **Authentication and Profile Management**:
    * Secure login.
    * Users can view and update their personal information (FullName, Email, Address).
    * Users can change their password.
    * Real-time validation for input fields.
* **Staff Management (For Admins)**:
    * Displays a list of staff members in a `DataGrid` with columns: Username, Full Name, Email, Role, Status (Active/Inactive).
    * **Does not display the Staff ID column.**
    * Search functionality by keyword (API handles searching across relevant fields).
    * Sorting functionality by attributes (FullName, Username, Email, Role) in ascending (asc) or descending (desc) order.
    * Pagination with a `skip` parameter (0-indexed).
    * **Actions for each row**:
        * **Edit**: Admins can edit staff information (Username, Email, Address, FullName, Password - optional, IsActive). **Role cannot be updated via this dialog.**
        * **Delete**: Admins can "soft delete" staff (set `IsActive = false`). The button is only visible when the staff member is active.
        * **Restore**: Admins can restore staff (set `IsActive = true`). The button is only visible when the staff member is inactive.
        * **Assign Task**: Admins can click this button to open the "Add New Task" dialog and pre-assign the staff member from that row as the Assignee. This button is disabled if the staff member is inactive.
* **Task Management**:
    * Displays a list of tasks in a `DataGrid` with a UI consistent with the Staff Management page.
    * Columns: Title, Description (with tooltip for full content), Assignee, Due Date, Status.
    * **Does not display the Task ID column.**
    * Search, sort, and pagination functionalities similar to Staff Management.
    * Filter tasks by status (ToDo, InProgress, Done, Cancelled, All).
    * **Actions for each row (Admin)**:
        * **Edit**: Admins can edit all fields of a task.
        * **Delete**: Admins can delete tasks (currently hard delete, soft delete can be considered).
    * **Staff Permissions**:
        * Staff can only see tasks assigned to them.
        * Staff can click the "Edit" button for their assigned tasks to **only update the "Status" field**. Other fields will be read-only in the dialog.
* **Add/Edit Task Dialog**:
    * **Assignee is a required field.**
    * The Assignee `ComboBox` supports search/filter functionality for a large number of staff.
    * Validation for required fields.
* **Real-time Notifications (SignalR)**:
    * Hub URL: `http://localhost:5000/hub-server`.
    * When a staff member is assigned a new task by an admin, they receive a notification.
    * When a task's status is updated, relevant users (assignee or admin) receive a notification.
    * Notifications appear as **tray notifications (balloon tips)**.
    * Clicking on a task-related notification opens the **edit task dialog** for the respective task.
* **Background Operation**:
    * When the user clicks the close (X) button on the main window, the application minimizes to the system tray instead of shutting down completely.
    * The application can be fully closed from the context menu of the taskbar icon.
* **Toast Notifications**: Used for quick in-app messages (e.g., save success, errors).

## 🚀 How to Run the Project

### Requirements:
* .NET 8 SDK
* Docker Desktop

### Steps:
1.  **Clone the repository**
    * Repo URL: `https://github.com/lebathinh2003/ToDo-List-Application.git`.
2.  **Open Backend**:
    * Open a terminal or command prompt in the root directory of the backend.
    * Run the script file: `./runScript.sh` (on Linux/macOS) or `runScript.sh` (on Windows with Git Bash or WSL).
    * **Option 1: `setup-backend`**:
        * Select this option for the script to automatically pull the necessary Docker images (SQL Server, RabbitMQ) and create the corresponding containers.
        * Wait for the setup process to complete.
    * **Option 2: `run all service`**:
        * After a successful backend setup, select this option to run all microservices locally.
        * Ensure all services have started without errors.
3.  **Open Frontend (WPF Desktop App)**:
    * Open the WPF project solution (`WpfTaskManagerApp.sln`) using Visual Studio.
    * Build the project.
    * Run the application (Press F5 or the Start button).

## 🔮 Future Development (Optional)
* Implement a DialogService for centralized dialog management.
* Add soft delete functionality for Tasks.
* Enhance UI/UX for other screens.
* Write unit and integration tests.

---
