namespace FicheroNacionalPip.Data;

public abstract class Version {
    public virtual string GetVersion()
    {
        System.Version? version = typeof(Version).Assembly.GetName().Version;
        return $"Data: {version?.Major}.{version?.Minor}.{version?.Build}";
    }
}