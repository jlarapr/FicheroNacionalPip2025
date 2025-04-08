using System.Windows;
using System.Windows.Media.Animation;
using FicheroNacionalPip.Presentation.Interfaces;

namespace FicheroNacionalPip.Presentation.Helpers;

public class ToggleMenu {
    public static bool GetEnableToggleMenu(DependencyObject obj, bool value) {
        return (bool)obj.GetValue(EnableToggleMenuProperty);
    }
    public static void SetEnableToggleMenu(DependencyObject obj, bool value) {
        obj.SetValue(EnableToggleMenuProperty, value);
    }
    public static readonly DependencyProperty EnableToggleMenuProperty =
        DependencyProperty.RegisterAttached(
            "EnableToggleMenu",
            typeof(bool),
            typeof(ToggleMenu),
            new PropertyMetadata(false, OnEnableToggleMenuChanged)
        );

    private static void OnEnableToggleMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is Window window) {
            window.Loaded += (o, eventArgs) => {
                if (window.DataContext is IToggleMenu vm2) {
                    vm2.ToggleMenuEvent += stateClosed => {
                        Storyboard? sb;
                        if (stateClosed) {
                            sb = window.FindResource("CloseMenu") as Storyboard;
                        }
                        else {
                            sb = window.FindResource("OpenMenu") as Storyboard;
                        }
                        sb?.Begin();
                    };
                }
            };
        }
    }
}