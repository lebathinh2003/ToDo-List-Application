using System.Windows;
using System.Windows.Controls;

namespace WpfTaskManagerApp.Helpers;

// Helper for PasswordBox data binding.
public static class PasswordHelper
{
    // Attached DP for binding PasswordBox.Password.
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

    // Gets the Password property.
    public static string GetPassword(DependencyObject dp)
    {
        return (string)dp.GetValue(PasswordProperty);
    }

    // Sets the Password property.
    public static void SetPassword(DependencyObject dp, string value)
    {
        dp.SetValue(PasswordProperty, value);
    }

    // Tracks if the password update is from UI.
    private static readonly DependencyProperty IsUpdatingProperty =
       DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false));

    // Gets IsUpdating.
    private static bool GetIsUpdating(DependencyObject dp)
    {
        return (bool)dp.GetValue(IsUpdatingProperty);
    }

    // Sets IsUpdating.
    private static void SetIsUpdating(DependencyObject dp, bool value)
    {
        dp.SetValue(IsUpdatingProperty, value);
    }

    // Handles Password DP changes.
    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            // Update PasswordBox if change isn't from UI.
            if (!GetIsUpdating(passwordBox) && passwordBox.Password != (string)e.NewValue)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged; // Avoid recursion.
                passwordBox.Password = (string)e.NewValue;
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged; // Re-hook.
            }
        }
    }

    // Handles PasswordBox.PasswordChanged event.
    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            SetIsUpdating(passwordBox, true); // UI is source.
            SetPassword(passwordBox, passwordBox.Password); // Update DP.
            SetIsUpdating(passwordBox, false); // Reset.
        }
    }

    // Attached DP to enable password binding behavior.
    public static readonly DependencyProperty AttachBehaviorProperty =
        DependencyProperty.RegisterAttached(
            "AttachBehavior",
            typeof(bool),
            typeof(PasswordHelper),
            new PropertyMetadata(false, OnAttachBehaviorChanged));

    // Gets AttachBehavior.
    public static bool GetAttachBehavior(DependencyObject dp)
    {
        return (bool)dp.GetValue(AttachBehaviorProperty);
    }

    // Sets AttachBehavior.
    public static void SetAttachBehavior(DependencyObject dp, bool value)
    {
        dp.SetValue(AttachBehaviorProperty, value);
    }

    // Attaches/detaches PasswordChanged event handler.
    private static void OnAttachBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox pb)
        {
            bool attach = (bool)e.NewValue;

            pb.PasswordChanged -= PasswordBox_PasswordChanged; // Ensure no duplicates.
            if (attach)
            {
                pb.PasswordChanged += PasswordBox_PasswordChanged;

                // Initial sync.
                string currentViewModelPassword = GetPassword(pb);
                if (!string.IsNullOrEmpty(pb.Password) && string.IsNullOrEmpty(currentViewModelPassword))
                {
                    PasswordBox_PasswordChanged(pb, new RoutedEventArgs()); // UI to ViewModel.
                }
                else if (string.IsNullOrEmpty(pb.Password) && !string.IsNullOrEmpty(currentViewModelPassword))
                {
                    pb.Password = currentViewModelPassword; // ViewModel to UI.
                }
            }
        }
    }
}