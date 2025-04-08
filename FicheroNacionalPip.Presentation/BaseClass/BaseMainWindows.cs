using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using FicheroNacionalPip.Presentation.Interfaces;

namespace FicheroNacionalPip.Presentation.BaseClass;

public abstract class BaseMainWindows : ObservableObject, IClosingWindows, IMouseDown, IKeyPreview, IToggleMenu  {
    public Action? OnClosing { get; set; } = null;
    public Action<bool>? ToggleMenuEvent { get; set; }

    public virtual bool CanClose() {
        MessageBoxResult response = MessageBox.Show("!!!Do you really want to exit?", "Exiting...", MessageBoxButton.YesNo,
            MessageBoxImage.Exclamation);

        return response == MessageBoxResult.Yes;
    }

    public virtual void MouseDown(object sender, MouseButtonEventArgs e) {
        try {
            if (e.ChangedButton != MouseButton.Left) return;
            if (sender is Window windows)
                windows.DragMove();
        }
        catch {
            // ignored
        }
    }

    public virtual void KeyPreviewKeyDown(object sender, KeyEventArgs e) {
        try {
            if (e.Key != Key.Escape) return;
            if (sender is Window windows)
                windows.Close();
        }
        catch {
            // ignored
        }
    }
}