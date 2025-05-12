# 🗂️ Task Management System - Microservice Application (.NET 8)

## 📌 Description

A system that allows managing, assigning, and tracking the progress of tasks in real-time. Assignees receive instant notifications when a task is assigned to them, even if the application is running in the background.

---

## 🏗️ Architecture Overview

### Microservices:

| Service            | Description                                                              |
| ------------------ | ------------------------------------------------------------------------ |
| **AuthService**    | Handles user authentication and issues JWT tokens (using IdentityServer) |
| **UserService**    | Manages user profile information (e.g., name, email, role)               |
| **JobService**     | Manages tasks, statuses, and assigns responsible users                   |
| **SignalRService** | Sends real-time notifications via SignalR                                |

---

## 💻 Frontend

| Platform         | Description                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------ |
| **WPF (.NET 8)** | Desktop application using MVVM architecture, supports real-time updates and user-friendly UI/UX. |

---

## 🔐 User Roles

| Role        | Permissions                                            |
| ----------- | ------------------------------------------------------ |
| **Admin**   | Full access: manage users, assign tasks, view reports  |
| **Manager** | Assign tasks and monitor progress (no user management) |
| **Staff**   | View and update their assigned tasks only              |

---

## 🔧 Technologies Used

- **.NET 8** (ASP.NET Core, EF Core)
- **MySQL / SQL Server**
- **SignalR** for real-time communication
- **Duende IdentityServer** for authentication
- **WPF (MVVM)** for frontend

---

## 🚀 How to Run

1. Clone this repository.
2. Set up the databases for each service.
3. Configure connection strings in each project.
4. Run each service independently.
5. Launch the WPF client.

---

## 📬 Contact

For questions or support, please contact: [your-email@example.com]
