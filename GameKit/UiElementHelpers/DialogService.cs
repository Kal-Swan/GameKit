using System.Windows;

namespace GameKit.UiElementHelpers;

public class DialogService : IDialogService
{
    public void MessageBox(string message)
    {
        System.Windows.MessageBox.Show(message);
    }

    public MessageBoxResult MessageBox(string message, string caption, MessageBoxButton button, MessageBoxImage image)
    {
        return System.Windows.MessageBox.Show(message, caption, button, image);
    }
}