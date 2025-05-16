using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp;
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>(sp => {
            var client = new HttpClient { };
            return client;
        });

        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IUserService, ApiUserService>();
        services.AddSingleton<IAuthenticationService, ApiAuthenticationService>();
        services.AddSingleton<ITaskService, ApiTaskService>();
        services.AddSingleton<ISignalRService, SignalRService>();

        services.AddSingleton<INavigationService>(sp =>
            new NavigationService(type => (ViewModelBase)sp.GetRequiredService(type))
        );

        // ***** ĐĂNG KÝ TOASTNOTIFICATIONVIEWMODEL *****
        services.AddSingleton<ToastNotificationViewModel>();
        // ***** KẾT THÚC ĐĂNG KÝ *****

        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<StaffManagementViewModel>();
        services.AddTransient<TaskManagementViewModel>();
        services.AddTransient<AddEditUserViewModel>();
        services.AddTransient<AddEditTaskViewModel>();

        services.AddSingleton<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }
}