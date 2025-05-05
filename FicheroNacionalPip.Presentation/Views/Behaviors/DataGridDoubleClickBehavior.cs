using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace FicheroNacionalPip.Presentation.Views.Behaviors;

public class DataGridDoubleClickBehavior : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseDoubleClick += OnMouseDoubleClick;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseDoubleClick -= OnMouseDoubleClick;
    }

    private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AssociatedObject.SelectedItem is not null &&
            AssociatedObject.DataContext is INotifyPropertyChanged viewModel)
        {
            var commandProperty = viewModel.GetType().GetProperty("OpenDialogCommand");
            var command = commandProperty?.GetValue(viewModel) as ICommand;
            if (command != null && command.CanExecute(null))
                command.Execute(null);
        }
    }
}