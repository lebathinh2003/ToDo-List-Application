using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WpfTaskManagerApp.Helpers;

public static class PasswordHelper
{
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
        // Debug.WriteLine($"PasswordHelper.SetPassword (Attached DP): Setting Password DP to '{(string.IsNullOrEmpty(value) ? "<empty>" : value)}' on {dp.GetType().Name}"); 
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
        // Debug.WriteLine($"PasswordHelper.SetIsUpdating: Setting IsUpdating DP to '{value}' on {dp.GetType().Name}"); 
        dp.SetValue(IsUpdatingProperty, value);
    }

    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        // Debug.WriteLine($"PasswordHelper.OnPasswordPropertyChanged (Attached DP changed): Sender={sender.GetType().Name}, OldValue='{e.OldValue}', NewValue='{e.NewValue}'"); 
        if (sender is PasswordBox passwordBox)
        {
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            if (!GetIsUpdating(passwordBox) && passwordBox.Password != (string)e.NewValue)
            {
                // Debug.WriteLine($"PasswordHelper.OnPasswordPropertyChanged: Updating PasswordBox.Password UI to '{e.NewValue}'"); 
                passwordBox.Password = (string)e.NewValue;
            }
            // else
            // {
            //    Debug.WriteLine($"PasswordHelper.OnPasswordPropertyChanged: IsUpdating is true, not updating PasswordBox.Password."); 
            // }
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            // Debug.WriteLine($"PasswordHelper.PasswordBox_PasswordChanged (UI Event): PasswordBox content changed. Current PasswordBox.Password = '{(string.IsNullOrEmpty(passwordBox.Password) ? "<empty>" : passwordBox.Password)}'");

            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
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
            // Debug.WriteLine($"PasswordHelper.OnAttachBehaviorChanged: AttachBehavior changed to {attach} for {pb.Name}");

            if (attach)
            {
                pb.PasswordChanged -= PasswordBox_PasswordChanged;
                pb.PasswordChanged += PasswordBox_PasswordChanged;
                // Debug.WriteLine($"PasswordHelper.OnAttachBehaviorChanged: Hooked PasswordChanged event for {pb.Name}");

                string currentViewModelPassword = GetPassword(pb);
                if (pb.Password != currentViewModelPassword)
                {
                    if (!string.IsNullOrEmpty(pb.Password) && string.IsNullOrEmpty(currentViewModelPassword))
                    {
                        // Debug.WriteLine($"PasswordHelper.OnAttachBehaviorChanged: Initial sync from PasswordBox ('{pb.Password}') to ViewModel (was empty).");
                        PasswordBox_PasswordChanged(pb, new RoutedEventArgs());
                    }
                }
            }
            else
            {
                pb.PasswordChanged -= PasswordBox_PasswordChanged;
                // Debug.WriteLine($"PasswordHelper.OnAttachBehaviorChanged: Unhooked PasswordChanged event for {pb.Name}");
            }
        }
    }
}

