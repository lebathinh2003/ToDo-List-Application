using System.Windows.Controls;
using System.Windows.Input;
using WpfTaskManagerApp.ViewModels;

namespace WpfTaskManagerApp.Views;

public partial class StaffManagementView : UserControl
{
    public StaffManagementView()
    {
        InitializeComponent();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is StaffManagementViewModel viewModel && viewModel.SearchCommand.CanExecute(null))
            {
                viewModel.SearchCommand.Execute(null);
            }
        }
    }

}
