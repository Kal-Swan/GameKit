using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameKit.UiElementHelpers.Adorners;

public class DragAdorner : Adorner
{
    private readonly Border _ghost;
    private Point _position;
    
    public DragAdorner(UIElement adornedElement, string text) : base(adornedElement)
    {
        IsHitTestVisible = false;

        _ghost = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(230, 60, 52, 137)),
            CornerRadius = new CornerRadius(3),
            Padding = new Thickness(8, 3, 8, 3),
            Child = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold
            }
        };
    }

    public void SetPosition(Point position)
    {
        _position = position;
        
        (Parent as AdornerLayer)?.Update(AdornedElement);
    }
    
    protected override int VisualChildrenCount => 1;
    protected override Visual? GetVisualChild(int index)
    {
        return _ghost;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _ghost.Measure(constraint);
        return _ghost.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _ghost.Arrange(new Rect(finalSize));
        return finalSize;
    }

    public override GeneralTransform? GetDesiredTransform(GeneralTransform transform)
    {
        var group = new GeneralTransformGroup();
        
        group.Children.Add(base.GetDesiredTransform(transform));
        
        group.Children.Add(new TranslateTransform(_position.X + 12, _position.Y + 12));

        return group;
    }
}