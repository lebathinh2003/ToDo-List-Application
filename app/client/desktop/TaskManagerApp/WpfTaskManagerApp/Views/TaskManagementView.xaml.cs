using System.Windows.Controls;
using System.Windows.Input;
using WpfTaskManagerApp.ViewModels;

namespace WpfTaskManagerApp.Views;

public partial class TaskManagementView : UserControl
{
    public TaskManagementView()
    {
        InitializeComponent();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is TaskManagementViewModel viewModel && viewModel.SearchCommand.CanExecute(null))
            {
                viewModel.SearchCommand.Execute(null);
            }
        }
    }
}
