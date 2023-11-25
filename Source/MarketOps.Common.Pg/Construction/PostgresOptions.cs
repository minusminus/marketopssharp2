namespace MarketOps.Common.Pg.Construction;

/// <summary>
/// Pg connection configuration options.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public sealed class PostgresOptions
{
    public string ConnectionString { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
