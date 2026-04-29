using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class AktarimPlaniTestleri
{
    [Fact]
    public void Aktarim_plani_salt_okunur_kopya_ile_baslar_ve_canli_gecis_ile_biter()
    {
        var plan = new AktarimPlaniSaglayici().GetPlan();

        Assert.Equal(AktarimAsamaTuru.SaltOkunurAnlikKopya, plan.First().Kind);
        Assert.Equal(AktarimAsamaTuru.CanliGecis, plan.Last().Kind);
    }

    [Fact]
    public void Aktarim_plani_paralel_calismadan_once_mutabakat_icerir()
    {
        var plan = new AktarimPlaniSaglayici().GetPlan();
        var reconciliation = plan.Single(stage => stage.Kind == AktarimAsamaTuru.Mutabakat);
        var parallelRun = plan.Single(stage => stage.Kind == AktarimAsamaTuru.ParalelCalisma);

        Assert.True(reconciliation.Order < parallelRun.Order);
    }
}
