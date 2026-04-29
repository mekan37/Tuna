using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IModulKatalogu
{
    IReadOnlyList<ModulTanimi> GetModules();
}
