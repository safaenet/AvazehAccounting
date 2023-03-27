using System.Windows;
using System.Windows.Controls;

namespace AvazehWpf;

public class TextBoxThatDoesntResizeWithText : TextBox
{
    protected override Size MeasureOverride(Size constraint)
    {
        return new Size(0, 0);
    }
}