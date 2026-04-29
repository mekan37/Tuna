namespace Tuna.Altyapi;

public sealed class PostgresAyarlari
{
    public const string SectionName = "Postgres";

    public string ConnectionString { get; init; } =
        "Host=localhost;Port=5432;Database=tuna;Username=tuna;Password=tuna";
}
