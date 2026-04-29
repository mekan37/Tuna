using Tuna.Alan;

namespace Tuna.Uygulama;

public interface IAktarimPlaniSaglayici
{
    IReadOnlyList<AktarimAsamasi> GetPlan();
}
