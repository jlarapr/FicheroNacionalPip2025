namespace FicheroNacionalPip.Business;

public class Version : FicheroNacionalPip.Data.Version {

    public override string GetVersion()
    {
        System.Version? version = typeof(Version).Assembly.GetName().Version;
        return $"Business: {version?.Major}.{version?.Minor}.{version?.Build}";
    }

    public override string ToString() {
        return  $"{GetVersion()} | {GetDataVersion()}";
    }

    private string GetDataVersion() {
        return base.GetVersion();
    }

}