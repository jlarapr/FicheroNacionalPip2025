using System.Windows.Controls;
using System.Windows;

namespace FicheroNacionalPip.Presentation.Views.Controls;

public class CustomDataGrid : DataGrid {
    static CustomDataGrid() {
        // Se establece la clave de estilo por defecto para asegurar que se busque en Themes/Generic.xaml
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDataGrid),
            new FrameworkPropertyMetadata(typeof(CustomDataGrid)));
    }

    // Constructor por defecto, requerido para instanciación vía XAML.
    public CustomDataGrid() : this(null) {
        this.AutoGenerateColumns = true;

    }

    // Constructor adicional que permite pasar columnas iniciales
    public CustomDataGrid(IEnumerable<DataGridColumn>? columns) {
        // Configuración predeterminada
        this.AutoGenerateColumns = false;
        this.CanUserAddRows = false;
        this.CanUserDeleteRows = false;
        this.IsReadOnly = true;
        this.RowHeight = 40;
        this.ColumnHeaderHeight = 50;

        // Aplicar estilo de Material Design, si el recurso está definido
        if (Application.Current.Resources.Contains("MaterialDesignDataGrid")) {
            this.Style = (Style)Application.Current.Resources["MaterialDesignDataGrid"];
        }

        // Agregar columnas si se proporcionan
        if (columns == null) return;
        foreach (DataGridColumn column in columns) {
            this.Columns.Add(column);
        }
    }

    // Agrega aquí propiedades, métodos o eventos personalizados según sean requeridos
}