using System.Windows;
using GameKit.ViewModels;

namespace GameKit.Views;

public partial class MoveToDialog : Window
{
    public MoveToDialog()
    {
        InitializeComponent();
    }
    
    private void OnConfirmClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}