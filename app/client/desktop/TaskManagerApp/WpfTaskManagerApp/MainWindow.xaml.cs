using System.ComponentModel;
using System.Windows;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.ViewModels;

namespace WpfTaskManagerApp;

public partial class MainWindow : Window
{
    private NotifyIcon? _notifyIcon;
    private bool _isExplicitClose = false; // Flag for real close
    private TaskItem? _taskForTrayNotification;

    public MainWindow()
    {
        InitializeComponent();
        InitializeNotifyIcon(); // Initialize tray icon
    }

    private void InitializeNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("Resources/to_do.ico"),
            Visible = false, // Show only when window hidden
            Text = "Task Manager Pro"
        };

        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick; // Double click to open window
        _notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked; // Click balloon tip

        var contextMenu = new ContextMenuStrip();
        var showMenuItem = new ToolStripMenuItem("Show Task Manager Pro");
        showMenuItem.Click += ShowMenuItem_Click; // Show window from menu
        contextMenu.Items.Add(showMenuItem);

        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += ExitMenuItem_Click; // Exit app from menu
        contextMenu.Items.Add(exitMenuItem);

        _notifyIcon.ContextMenuStrip = contextMenu;
        _notifyIcon.Visible = true; // Display tray icon
    }

    public void ShowWindowAndActivate()
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();

        if (this.Topmost == false) // Ensure window is on top
        {
            this.Topmost = true;
            this.Topmost = false;
        }
    }

    private void ShowWindow()
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        ShowWindow(); // Open window on tray icon double click
    }

    private void ShowMenuItem_Click(object? sender, EventArgs e)
    {
        ShowWindow(); // Open window from context menu
    }

    public void ShowTrayNotification(string title, string message, TaskItem? task = null)
    {
        _taskForTrayNotification = task; // Store task for notification click
        _notifyIcon?.ShowBalloonTip(5000, title, message, ToolTipIcon.Info); // Show balloon tip
    }

    private void NotifyIcon_BalloonTipClicked(object? sender, EventArgs e)
    {
        ShowWindowAndActivate(); // Show window when notification clicked

        if (_taskForTrayNotification != null && DataContext is MainViewModel mainVm)
        {
            mainVm.HandleTrayNotificationActivation(_taskForTrayNotification); // Handle notification action
            _taskForTrayNotification = null; // Reset stored task
        }
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        _isExplicitClose = true; // Mark as real close
        System.Windows.Application.Current.Shutdown(); // Shutdown app
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_isExplicitClose) // If not real close
        {
            e.Cancel = true; // Cancel closing
            this.Hide(); // Hide window instead
        }
        else // Real close
        {
            _notifyIcon?.Dispose(); // Dispose tray icon
        }
        base.OnClosing(e);
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            // Optionally hide window when minimized
        }
        base.OnStateChanged(e);
    }
}
