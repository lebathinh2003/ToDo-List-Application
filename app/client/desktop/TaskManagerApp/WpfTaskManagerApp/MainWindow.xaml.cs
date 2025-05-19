using System.ComponentModel;
using System.Windows;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.ViewModels; // Cần thêm tham chiếu System.Windows.Forms.dll
// using Application = System.Windows.Application; // Nếu có xung đột tên

namespace WpfTaskManagerApp;

public partial class MainWindow : Window
{
    private NotifyIcon? _notifyIcon;
    private bool _isExplicitClose = false; // Cờ để biết có phải là đóng thực sự không
    private TaskItem? _taskForTrayNotification;

    public MainWindow()
    {
        InitializeComponent();
        InitializeNotifyIcon();
    }

    private void InitializeNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("Resources/to_do.ico"),
            Visible = false, // Chỉ hiển thị khi cửa sổ bị ẩn
            Text = "Task Manager Pro"
        };

        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        _notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;

        var contextMenu = new ContextMenuStrip();
        var showMenuItem = new ToolStripMenuItem("Show Task Manager Pro");
        showMenuItem.Click += ShowMenuItem_Click;
        contextMenu.Items.Add(showMenuItem);

        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += ExitMenuItem_Click;
        contextMenu.Items.Add(exitMenuItem);

        _notifyIcon.ContextMenuStrip = contextMenu;
        _notifyIcon.Visible = true;
    }

    public void ShowWindowAndActivate()
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
        if (this.Topmost == false) // Đảm bảo cửa sổ lên trên cùng nếu nó không phải là Topmost
        {
            this.Topmost = true;
            this.Topmost = false;
        }
        //if (_notifyIcon != null) _notifyIcon.Visible = true;
    }

    private void ShowWindow()
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
        //if (_notifyIcon != null) _notifyIcon.Visible = false;
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    private void ShowMenuItem_Click(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    public void ShowTrayNotification(string title, string message, TaskItem? task = null)
    {
        _taskForTrayNotification = task; // Lưu task lại để xử lý khi click
        _notifyIcon?.ShowBalloonTip(5000, title, message, ToolTipIcon.Info);
    }

    private void NotifyIcon_BalloonTipClicked(object? sender, EventArgs e)
    {
        ShowWindowAndActivate();
        if (_taskForTrayNotification != null && DataContext is MainViewModel mainVm)
        {
            mainVm.HandleTrayNotificationActivation(_taskForTrayNotification);
            _taskForTrayNotification = null; // Reset sau khi xử lý
        }
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        _isExplicitClose = true; // Đánh dấu là người dùng muốn đóng thực sự
        System.Windows.Application.Current.Shutdown(); // Sử dụng System.Windows.Application để tránh nhầm lẫn
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_isExplicitClose) // Nếu không phải là đóng thực sự từ menu "Exit"
        {
            e.Cancel = true; // Hủy việc đóng cửa sổ
            this.Hide();     // Ẩn cửa sổ đi
            //if (_notifyIcon != null)
            //{
            //    _notifyIcon.Visible = true; // Hiển thị icon ở khay hệ thống
            //}
        }
        else // Nếu là đóng thực sự
        {
            _notifyIcon?.Dispose(); // Dọn dẹp NotifyIcon
        }
        base.OnClosing(e);
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            // Tùy chọn: bạn có thể ẩn cửa sổ và hiện NotifyIcon khi minimize
            // this.Hide();
            // if (_notifyIcon != null) _notifyIcon.Visible = true;
        }
        base.OnStateChanged(e);
    }
}
