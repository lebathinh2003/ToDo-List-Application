using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WpfTaskManagerApp.Helpers;
public static class PasswordHelper
{
    // Static constructor to confirm class loading
    static PasswordHelper()
    {
        Debug.WriteLine("-----------------------------------------------------------");
        Debug.WriteLine("PasswordHelper: STATIC CONSTRUCTOR CALLED. Class is loaded.");
        Debug.WriteLine("-----------------------------------------------------------");
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

    public static string GetPassword(DependencyObject dp)
    {
        return (string)dp.GetValue(PasswordProperty);
    }

    public static void SetPassword(DependencyObject dp, string value)
    {
        dp.SetValue(PasswordProperty, value);
    }

    private static readonly DependencyProperty IsUpdatingProperty =
       DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false));

    private static bool GetIsUpdating(DependencyObject dp)
    {
        return (bool)dp.GetValue(IsUpdatingProperty);
    }

    private static void SetIsUpdating(DependencyObject dp, bool value)
    {
        // Debug.WriteLine($"PasswordHelper.SetIsUpdating: Setting IsUpdating DP to '{value}' on {dp.GetType().Name}"); // Can be noisy
        dp.SetValue(IsUpdatingProperty, value);
    }

    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            // This ensures that if the ViewModel updates the Password property,
            // the UI (PasswordBox.Password) is updated, unless IsUpdating is true (meaning the change came from the UI itself).
            if (!GetIsUpdating(passwordBox) && passwordBox.Password != (string)e.NewValue)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged; // Temporarily unhook to prevent recursion
                passwordBox.Password = (string)e.NewValue;
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged; // Re-hook
            }
            // else
            // {
            //    Debug.WriteLine($"PasswordHelper.OnPasswordPropertyChanged: IsUpdating is true or PasswordBox.Password already matches. Not updating PasswordBox UI from ViewModel."); 
            // }
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {

            SetIsUpdating(passwordBox, true); // Signal that the source of change is the UI
            SetPassword(passwordBox, passwordBox.Password); // Update the attached DP, which will update the ViewModel via TwoWay binding
            SetIsUpdating(passwordBox, false); // Reset the flag
        }
    }

    public static readonly DependencyProperty AttachBehaviorProperty =
        DependencyProperty.RegisterAttached(
            "AttachBehavior",
            typeof(bool),
            typeof(PasswordHelper),
            new PropertyMetadata(false, OnAttachBehaviorChanged));

    public static bool GetAttachBehavior(DependencyObject dp)
    {
        return (bool)dp.GetValue(AttachBehaviorProperty);
    }

    public static void SetAttachBehavior(DependencyObject dp, bool value)
    {
        dp.SetValue(AttachBehaviorProperty, value);
    }

    private static void OnAttachBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        if (d is PasswordBox pb)
        {
            bool attach = (bool)e.NewValue;

            // Always remove first to prevent multiple subscriptions if this callback were somehow invoked multiple times for the same instance.
            pb.PasswordChanged -= PasswordBox_PasswordChanged;
            if (attach)
            {
                pb.PasswordChanged += PasswordBox_PasswordChanged;

                // Initial sync: If PasswordBox has a value and ViewModel's bound property is empty, update ViewModel.
                string currentViewModelPassword = GetPassword(pb);
                if (!string.IsNullOrEmpty(pb.Password) && string.IsNullOrEmpty(currentViewModelPassword))
                {
                    PasswordBox_PasswordChanged(pb, new RoutedEventArgs());
                }
                // If ViewModel has a value and PasswordBox is empty, OnPasswordPropertyChanged (triggered by binding) should handle it.
                else if (string.IsNullOrEmpty(pb.Password) && !string.IsNullOrEmpty(currentViewModelPassword))
                {
                    pb.Password = currentViewModelPassword;
                }
            }
        }
        else
        {
        }
    }
}
