using System.Data;

namespace MarketOps.Common.Pg.ConnectionFactory;

/// <summary>
/// Factory interface for Pg connections creation.
/// </summary>
public interface IPgConnectionFactory
{
    public IDbConnection Create();
}
