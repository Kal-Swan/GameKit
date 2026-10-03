using System.Windows;

namespace GameKit.UiElementHelpers;

public interface IDialogService
{
    void MessageBox(string message);
    
    MessageBoxResult MessageBox(string message, string caption, MessageBoxButton button, MessageBoxImage image);
}