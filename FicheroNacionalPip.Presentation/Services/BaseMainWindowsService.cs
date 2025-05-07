using FicheroNacionalPip.Presentation.BaseClass;
using Microsoft.Extensions.Logging;

namespace FicheroNacionalPip.Presentation.Services;

public class BaseMainWindowsService : BaseMainWindows {
    protected BaseMainWindowsService(ILogger<BaseMainWindows> logger) : base(logger) { }
}