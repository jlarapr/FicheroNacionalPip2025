namespace FicheroNacionalPip.Presentation.Models;

public class Elector {
    public string EstatusElectoral { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string ApellidoMaterno { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string NombrePadre { get; set; } = string.Empty;
    public string NombreMadre { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; } = DateTime.MinValue;
    public string Sexo { get; set; } = string.Empty;

    public string ColorOjos { get; set; } = string.Empty;
    public string Estatura { get; set; } = string.Empty;
    public bool Gemelo { get; set; }
    public string LugarNacimiento { get; set; } = string.Empty;
    public string Precinto { get; set; } = string.Empty;
    public string UnidadElectoral { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string DireccionResidencial { get; set; } = string.Empty;
    public string Barrio { get; set; } = string.Empty;
    public string DireccionLinea1 { get; set; } = string.Empty;

    public string DireccionPostal1 { get; set; } = string.Empty;
    public string DireccionPostal2 { get; set; } = string.Empty;
    public string ZonaPostal { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string NumeroSeguroSocial { get; set; } = string.Empty;
    public string LicenciaConducir { get; set; } = string.Empty;
    public string NumeroCuentaAee { get; set; } = string.Empty;
    public string TipoImpedimento { get; set; } = string.Empty;
    public string FacilidadRequerida { get; set; } = string.Empty;
    public string DocumentoApoyo { get; set; } = string.Empty;
    public bool FuncionarioDeColegio { get; set; }

    public string NumeroElectoral { get; set; } = string.Empty;

}