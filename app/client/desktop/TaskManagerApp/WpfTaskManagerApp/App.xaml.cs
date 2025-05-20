using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels;
using WpfTaskManagerApp.ViewModels.Common; // For ViewModelBase

namespace WpfTaskManagerApp;

// Main application class.
public partial class App : Application
{
    // Service provider for dependency injection.
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    // Constructor.
    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection); // Setup DI container.
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    // Configures services for dependency injection.
    private void ConfigureServices(IServiceCollection services)
    {
        // HTTP client (singleton).
        services.AddSingleton<HttpClient>(); // Simplified registration.

        // Core services (singletons).
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IUserService, ApiUserService>();
        services.AddSingleton<IAuthenticationService, ApiAuthenticationService>();
        services.AddSingleton<ITaskService, ApiTaskService>();
        services.AddSingleton<ISignalRService, SignalRService>();

        // Navigation service (singleton).
        services.AddSingleton<INavigationService>(sp =>
            new NavigationService(type => (ViewModelBase)sp.GetRequiredService(type)) // Factory for view models.
        );

        // Toast notification ViewModel (singleton).
        services.AddSingleton<ToastNotificationViewModel>();

        // Main and other ViewModels.
        services.AddSingleton<MainViewModel>(); // Main orchestrator.
        services.AddTransient<LoginViewModel>(); // Transient: new instance each time.
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<StaffManagementViewModel>();
        services.AddTransient<TaskManagementViewModel>();
        services.AddTransient<AddEditUserViewModel>(); // For dialogs.
        services.AddTransient<AddEditTaskViewModel>(); // For dialogs.

        // Main window (singleton).
        services.AddSingleton<MainWindow>();
    }

    // Application startup logic.
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Resolve and show the main window.
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>(); // Set DataContext.
        mainWindow.Show();
    }
}