using Tuna.Alan;
using Tuna.Uygulama;

namespace Tuna.Testler;

public sealed class ModulKataloguTestleri
{
    [Fact]
    public void Katalog_gerekli_hedef_semalari_icerir()
    {
        var schemas = new ModulKatalogu().GetModules().Select(module => module.Schema).ToHashSet();

        Assert.Contains("core", schemas);
        Assert.Contains("catalog", schemas);
        Assert.Contains("inventory", schemas);
        Assert.Contains("sales", schemas);
        Assert.Contains("purchase", schemas);
        Assert.Contains("finance", schemas);
        Assert.Contains("einvoice", schemas);
        Assert.Contains("tracktrace", schemas);
        Assert.Contains("audit", schemas);
        Assert.Contains("reporting", schemas);
        Assert.Contains("staging_foxpro", schemas);
    }

    [Fact]
    public void Izleme_modulu_yuksek_hacimli_eski_kaynaklari_gorunur_tutar()
    {
        var module = new ModulKatalogu().GetModules().Single(item => item.Module == UygulamaModulu.Izleme);

        Assert.Contains("PTS*", module.LegacySources);
        Assert.Contains("YKKS*", module.LegacySources);
        Assert.Contains("UTSS*", module.LegacySources);
    }
}
