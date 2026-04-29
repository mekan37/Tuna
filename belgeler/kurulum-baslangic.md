# Tuna Kurulum Baslangici

Bu iskelet, referans setindeki hedef mimariye gore olusturulan ilk .NET uygulama temelidir.

## Teknik Secim

- Uygulama adi: Tuna.
- Calisan SDK: .NET 10. Projeler `net10.0` hedefler.
- Mimari: moduler monolit.
- Veri hedefi: PostgreSQL sema ayrimi, staging migration, audit ve outbox.

Referanslar:
- `04-hedef-mimari-veri-modeli.md > 8. Hedef .NET Mimari`
- `04-hedef-mimari-veri-modeli.md > 9. PostgreSQL Hedef Veri Modeli`
- `04-hedef-mimari-veri-modeli.md > 12. Aktarim Stratejisi`
- `09-sonuc.md > 22. Sonuc`

## Projeler

| Proje | Gorev |
|---|---|
| `Tuna.Servis` | Minimal servis, OpenAPI, saglik ve operasyon endpointleri |
| `Tuna.Uygulama` | Kullanim senaryosu servisleri ve uygulama sozlesmeleri |
| `Tuna.Alan` | Modul, kaynak kanit ve aktarim kavramlari |
| `Tuna.Altyapi` | Kuyruk ve PostgreSQL altyapi sozlesmeleri |
| `Tuna.Isci` | Entegrasyon, bildirim, rapor ve aktarim arka plan isleri |
| `Tuna.Raporlama` | Rapor metadata katalogu |
| `Tuna.Aktarim` | DBF staging ve mutabakat CLI baslangici |

## Komutlar

```powershell
dotnet restore Tuna.sln
dotnet build Tuna.sln
dotnet test Tuna.sln
dotnet run --project kaynak\Tuna.Servis
dotnet run --project kaynak\Tuna.Isci
dotnet run --project kaynak\Tuna.Aktarim
```

## PostgreSQL

Ilk sema taslagi `database/postgresql/001_initial_schemas.sql` dosyasindadir. DBF dosyalari dogrudan hedef tablolara alinmayacak; once `staging_foxpro` altina ham veri, sonra normalize hedef tablolar, en sonda mutabakat raporu calisacaktir.
