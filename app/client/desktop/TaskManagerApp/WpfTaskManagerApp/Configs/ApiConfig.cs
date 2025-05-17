namespace WpfTaskManagerApp.Configs; public static class ApiConfig
{
    // URL cơ sở của API backend
    public const string BaseUrl = "http://localhost:5000"; // Hoặc URL API thực tế của bạn

    // Endpoints cho Authentication
    public const string AuthEndPoint = "auth"; // Ví dụ: /auth/login, /auth/change-password

    // Endpoints cho User operations
    public const string UserEndPoint = "users"; // Ví dụ: /users, /users/{id}, /users/id/{id}
    public const string UserProfileEndPoint = "users"; // Endpoint cho user tự cập nhật profile, ví dụ: /user

    // Endpoints cho Task operations
    public const string TaskEndPoint = "tasks"; // Ví dụ: /tasks, /tasks/{id}
}