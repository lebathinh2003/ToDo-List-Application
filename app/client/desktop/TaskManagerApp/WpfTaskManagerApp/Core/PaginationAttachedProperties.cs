using System.Windows;
using System.Windows.Input;
namespace WpfTaskManagerApp.Core;

public static class PaginationAttachedProperties
{
    // TotalItemsText (String, ví dụ: "Total Tasks: {0}" sẽ được format trong ViewModel hoặc khi binding)
    public static readonly DependencyProperty TotalItemsTextProperty =
        DependencyProperty.RegisterAttached("TotalItemsText", typeof(string), typeof(PaginationAttachedProperties), new PropertyMetadata("Total Items: 0"));

    public static string GetTotalItemsText(DependencyObject obj) => (string)obj.GetValue(TotalItemsTextProperty);
    public static void SetTotalItemsText(DependencyObject obj, string value) => obj.SetValue(TotalItemsTextProperty, value);

    // CurrentPageDisplay (để kiểu object để chấp nhận string hoặc int từ ViewModel)
    public static readonly DependencyProperty CurrentPageDisplayProperty =
        DependencyProperty.RegisterAttached("CurrentPageDisplay", typeof(object), typeof(PaginationAttachedProperties), new PropertyMetadata("1"));

    public static object GetCurrentPageDisplay(DependencyObject obj) => obj.GetValue(CurrentPageDisplayProperty);
    public static void SetCurrentPageDisplay(DependencyObject obj, object value) => obj.SetValue(CurrentPageDisplayProperty, value);

    // TotalPagesDisplay (để kiểu object)
    public static readonly DependencyProperty TotalPagesDisplayProperty =
        DependencyProperty.RegisterAttached("TotalPagesDisplay", typeof(object), typeof(PaginationAttachedProperties), new PropertyMetadata("1"));

    public static object GetTotalPagesDisplay(DependencyObject obj) => obj.GetValue(TotalPagesDisplayProperty);
    public static void SetTotalPagesDisplay(DependencyObject obj, object value) => obj.SetValue(TotalPagesDisplayProperty, value);

    // FirstPageCommand
    public static readonly DependencyProperty FirstPageCommandProperty =
        DependencyProperty.RegisterAttached("FirstPageCommand", typeof(ICommand), typeof(PaginationAttachedProperties), new PropertyMetadata(null));

    public static ICommand GetFirstPageCommand(DependencyObject obj) => (ICommand)obj.GetValue(FirstPageCommandProperty);
    public static void SetFirstPageCommand(DependencyObject obj, ICommand value) => obj.SetValue(FirstPageCommandProperty, value);

    // PreviousPageCommand
    public static readonly DependencyProperty PreviousPageCommandProperty =
        DependencyProperty.RegisterAttached("PreviousPageCommand", typeof(ICommand), typeof(PaginationAttachedProperties), new PropertyMetadata(null));

    public static ICommand GetPreviousPageCommand(DependencyObject obj) => (ICommand)obj.GetValue(PreviousPageCommandProperty);
    public static void SetPreviousPageCommand(DependencyObject obj, ICommand value) => obj.SetValue(PreviousPageCommandProperty, value);

    // NextPageCommand
    public static readonly DependencyProperty NextPageCommandProperty =
        DependencyProperty.RegisterAttached("NextPageCommand", typeof(ICommand), typeof(PaginationAttachedProperties), new PropertyMetadata(null));

    public static ICommand GetNextPageCommand(DependencyObject obj) => (ICommand)obj.GetValue(NextPageCommandProperty);
    public static void SetNextPageCommand(DependencyObject obj, ICommand value) => obj.SetValue(NextPageCommandProperty, value);

    // LastPageCommand
    public static readonly DependencyProperty LastPageCommandProperty =
        DependencyProperty.RegisterAttached("LastPageCommand", typeof(ICommand), typeof(PaginationAttachedProperties), new PropertyMetadata(null));

    public static ICommand GetLastPageCommand(DependencyObject obj) => (ICommand)obj.GetValue(LastPageCommandProperty);
    public static void SetLastPageCommand(DependencyObject obj, ICommand value) => obj.SetValue(LastPageCommandProperty, value);

    // CanGoToPreviousPage
    public static readonly DependencyProperty CanGoToPreviousPageProperty =
        DependencyProperty.RegisterAttached("CanGoToPreviousPage", typeof(bool), typeof(PaginationAttachedProperties), new PropertyMetadata(false));

    public static bool GetCanGoToPreviousPage(DependencyObject obj) => (bool)obj.GetValue(CanGoToPreviousPageProperty);
    public static void SetCanGoToPreviousPage(DependencyObject obj, bool value) => obj.SetValue(CanGoToPreviousPageProperty, value);

    // CanGoToNextPage
    public static readonly DependencyProperty CanGoToNextPageProperty =
        DependencyProperty.RegisterAttached("CanGoToNextPage", typeof(bool), typeof(PaginationAttachedProperties), new PropertyMetadata(false));

    public static bool GetCanGoToNextPage(DependencyObject obj) => (bool)obj.GetValue(CanGoToNextPageProperty);
    public static void SetCanGoToNextPage(DependencyObject obj, bool value) => obj.SetValue(CanGoToNextPageProperty, value);
}