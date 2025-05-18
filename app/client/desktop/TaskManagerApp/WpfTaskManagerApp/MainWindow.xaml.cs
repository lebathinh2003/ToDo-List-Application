using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms; // Cần thêm tham chiếu System.Windows.Forms.dll
// using Application = System.Windows.Application; // Nếu có xung đột tên

namespace WpfTaskManagerApp;

public partial class MainWindow : Window
{
    private NotifyIcon? _notifyIcon;
    private bool _isExplicitClose = false; // Cờ để biết có phải là đóng thực sự không

    public MainWindow()
    {
        InitializeComponent();
        InitializeNotifyIcon();
    }

    private void InitializeNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            // Bạn cần một file icon .ico trong project và set Build Action là "Resource" hoặc "Content"
            // Ví dụ: Icon = new System.Drawing.Icon("Assets/app_icon.ico"),
            // Hoặc dùng icon từ resource:
            // Icon = System.Drawing.Icon.FromHandle(Properties.Resources.YourAppIcon.GetHicon()),
            // Tạm thời dùng một icon mặc định nếu chưa có
            Icon = System.Drawing.SystemIcons.Application,
            Visible = false, // Chỉ hiển thị khi cửa sổ bị ẩn
            Text = "Task Manager Pro"
        };

        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

        var contextMenu = new ContextMenuStrip();
        var showMenuItem = new ToolStripMenuItem("Show Task Manager Pro");
        showMenuItem.Click += ShowMenuItem_Click;
        contextMenu.Items.Add(showMenuItem);

        var exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += ExitMenuItem_Click;
        contextMenu.Items.Add(exitMenuItem);

        _notifyIcon.ContextMenuStrip = contextMenu;
    }

    private void ShowWindow()
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
        if (_notifyIcon != null) _notifyIcon.Visible = false;
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    private void ShowMenuItem_Click(object? sender, EventArgs e)
    {
        ShowWindow();
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
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = true; // Hiển thị icon ở khay hệ thống
                _notifyIcon.ShowBalloonTip(1000, "Task Manager Pro", "Application is still running in the background.", ToolTipIcon.Info);
            }
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
