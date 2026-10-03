using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using GameKit.UiElementHelpers.Adorners;
using GameKit.ViewModels;

namespace GameKit.Views;

public partial class MainWindow : Window
{
    private MainViewModel _viewModel;
    private EntityViewModel _dragCandidate;
    private DragAdorner? _adorner;
    
    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _viewModel.Entities.CommitPendingEdits();
        
        if (!_viewModel.DiscardChangesIfAny())
        {
            e.Cancel = true;
        }
        base.OnClosing(e);
    }
    
    private void BeginDrag(object sender, MouseButtonEventArgs e)
    {
        var dragItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource)?.DataContext as EntityViewModel;

        if (dragItem is null)
        {
            return;
        }
        
        _dragCandidate = dragItem;
    }

    private void Drag(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }
        
        var item = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
        if (item?.DataContext is not EntityViewModel)
        {
            return;
        }

        if (FindAncestor<ToggleButton>((DependencyObject)e.OriginalSource) is not null)
        {
            return;
        }
        
        var tree = (TreeView)sender;
        var layer = AdornerLayer.GetAdornerLayer(tree);

        _adorner = new DragAdorner(tree, _dragCandidate.DisplayName);
        layer?.Add(_adorner);

        try
        {
            DragDrop.DoDragDrop(
                tree,
                new DataObject(typeof(EntityViewModel), _dragCandidate),
                DragDropEffects.Move);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            if (_adorner is not null)
            {
                layer.Remove(_adorner);
                _adorner = null;
            }
        }
    }

    private void EndDrag(object sender, DragEventArgs e)
    {
        // stops other events listening to this drag
        e.Handled = true;

        var target = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource)?.DataContext as EntityViewModel;

        if (target is null)
        {
            return;
        }
        
        _adorner?.SetPosition(e.GetPosition((TreeView)sender));
    }

    private void DropOver(object sender, DragEventArgs e)
    {
        // stops other events listening to this drag
        e.Handled = true;
        
        if (e.Data.GetData(typeof(EntityViewModel)) is not EntityViewModel dragged)
        {
            return;
        }
        
        var target = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource)?.DataContext as EntityViewModel;

        if (_viewModel.Entities.CanMove(dragged, target))
        {
            _viewModel.Entities.MoveTo(dragged, target);
        }
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }
            
            current = current is Visual ? VisualTreeHelper.GetParent(current) : LogicalTreeHelper.GetParent(current);
        }

        return null;
    }
    
    private void OnTreeGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        e.UseDefaultCursors = false;
        Mouse.SetCursor(e.Effects.HasFlag(DragDropEffects.Move) ? Cursors.Arrow : Cursors.No);
        e.Handled = true;
    }
}