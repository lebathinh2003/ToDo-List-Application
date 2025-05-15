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

        // ***** ĐĂNG KÝ ITokenProvider *****
        services.AddSingleton<ITokenProvider, TokenProvider>();

        // Đăng ký các service khác
        // IUserService không còn phụ thuộc trực tiếp vào IAuthenticationService nữa
        services.AddSingleton<IUserService, ApiUserService>();

        // ApiAuthenticationService giờ sẽ inject IUserService và ITokenProvider
        services.AddSingleton<IAuthenticationService, ApiAuthenticationService>();

        // ApiTaskService giờ sẽ inject ITokenProvider
        services.AddSingleton<ITaskService, ApiTaskService>();

        services.AddSingleton<ISignalRService, SignalRService>();

        services.AddSingleton<INavigationService>(sp =>
            new NavigationService(type => (ViewModelBase)sp.GetRequiredService(type))
        );

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