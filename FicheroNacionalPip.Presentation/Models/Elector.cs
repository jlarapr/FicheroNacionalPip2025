namespace FicheroNacionalPip.Presentation.Models;

/// <summary>
/// Representa los datos de un elector en el sistema.
/// </summary>
public record class Elector
{
    /// <summary>
    /// Estado actual del elector en el sistema electoral.
    /// </summary>
    public string EstatusElectoral { get; init; } = string.Empty;

    /// <summary>
    /// Primer apellido del elector.
    /// </summary>
    public string ApellidoPaterno { get; init; } = string.Empty;

    /// <summary>
    /// Segundo apellido del elector.
    /// </summary>
    public string ApellidoMaterno { get; init; } = string.Empty;

    /// <summary>
    /// Nombre(s) del elector.
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Nombre completo del padre.
    /// </summary>
    public string NombrePadre { get; init; } = string.Empty;

    /// <summary>
    /// Nombre completo de la madre.
    /// </summary>
    public string NombreMadre { get; init; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento del elector.
    /// </summary>
    public DateTime FechaNacimiento { get; init; } = DateTime.MinValue;

    /// <summary>
    /// Sexo del elector.
    /// </summary>
    public string Sexo { get; init; } = string.Empty;

    /// <summary>
    /// Color de ojos del elector.
    /// </summary>
    public string ColorOjos { get; init; } = string.Empty;

    /// <summary>
    /// Estatura del elector.
    /// </summary>
    public string Estatura { get; init; } = string.Empty;

    /// <summary>
    /// Indica si el elector es gemelo.
    /// </summary>
    public bool Gemelo { get; init; }

    /// <summary>
    /// Lugar de nacimiento del elector.
    /// </summary>
    public string LugarNacimiento { get; init; } = string.Empty;

    /// <summary>
    /// Precinto electoral asignado.
    /// </summary>
    public string Precinto { get; init; } = string.Empty;

    /// <summary>
    /// Unidad electoral asignada.
    /// </summary>
    public string UnidadElectoral { get; init; } = string.Empty;

    /// <summary>
    /// Sector donde reside el elector.
    /// </summary>
    public string Sector { get; init; } = string.Empty;

    /// <summary>
    /// Dirección física de residencia.
    /// </summary>
    public string DireccionResidencial { get; init; } = string.Empty;

    /// <summary>
    /// Barrio donde reside el elector.
    /// </summary>
    public string Barrio { get; init; } = string.Empty;

    /// <summary>
    /// Primera línea de la dirección.
    /// </summary>
    public string DireccionLinea1 { get; init; } = string.Empty;

    /// <summary>
    /// Primera línea de la dirección postal.
    /// </summary>
    public string DireccionPostal1 { get; init; } = string.Empty;

    /// <summary>
    /// Segunda línea de la dirección postal.
    /// </summary>
    public string DireccionPostal2 { get; init; } = string.Empty;

    /// <summary>
    /// Zona postal.
    /// </summary>
    public string ZonaPostal { get; init; } = string.Empty;

    /// <summary>
    /// Código postal completo.
    /// </summary>
    public string CodigoPostal { get; init; } = string.Empty;

    /// <summary>
    /// Número de teléfono del elector.
    /// </summary>
    public string Telefono { get; init; } = string.Empty;

    /// <summary>
    /// Número de Seguro Social del elector.
    /// </summary>
    public string NumeroSeguroSocial { get; init; } = string.Empty;

    /// <summary>
    /// Número de licencia de conducir.
    /// </summary>
    public string LicenciaConducir { get; init; } = string.Empty;

    /// <summary>
    /// Número de cuenta de AEE.
    /// </summary>
    public string NumeroCuentaAee { get; init; } = string.Empty;

    /// <summary>
    /// Tipo de impedimento si aplica.
    /// </summary>
    public string TipoImpedimento { get; init; } = string.Empty;

    /// <summary>
    /// Facilidad requerida para el elector.
    /// </summary>
    public string FacilidadRequerida { get; init; } = string.Empty;

    /// <summary>
    /// Documento de apoyo presentado.
    /// </summary>
    public string DocumentoApoyo { get; init; } = string.Empty;

    /// <summary>
    /// Indica si el elector es funcionario de colegio.
    /// </summary>
    public bool FuncionarioDeColegio { get; init; }

    /// <summary>
    /// Número electoral único del elector.
    /// </summary>
    public string NumeroElectoral { get; init; } = string.Empty;
}