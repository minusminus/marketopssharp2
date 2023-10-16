using System.Data;
using Dapper;

namespace MarketOps.Common.Pg.Operations;

/// <summary>
/// Extensions supporting parameters usage in queries.
/// </summary>
public static class DynamicParametersExtensions
{
    #region Inputs
    public static DynamicParameters AddInParamString(this DynamicParameters dynamicParameters, string parameterName, string parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue ?? string.Empty, DbType.String, ParameterDirection.Input, parameterValue?.Length ?? 0);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamStrings(this DynamicParameters dynamicParameters, string parameterName, IEnumerable<string> parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue.Select(p => p ?? string.Empty), direction: ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamVarcharMax(this DynamicParameters dynamicParameters, string parameterName, string parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue ?? string.Empty, DbType.String, ParameterDirection.Input, -1);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamByte(this DynamicParameters dynamicParameters, string parameterName, byte? parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue, DbType.Byte, ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamInt(this DynamicParameters dynamicParameters, string parameterName, int? parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue, DbType.Int32, ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamInts(this DynamicParameters dynamicParameters, string parameterName, IEnumerable<int> parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue, direction: ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamLong(this DynamicParameters dynamicParameters, string parameterName, long? parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue, DbType.Int64, ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamBit(this DynamicParameters dynamicParameters, string parameterName, bool parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue, DbType.Boolean, ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamDate(this DynamicParameters dynamicParameters, string parameterName, DateTime parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue.Date, DbType.Date, ParameterDirection.Input);
        return dynamicParameters;
    }

    public static DynamicParameters AddInParamDateTime(this DynamicParameters dynamicParameters, string parameterName, DateTime parameterValue)
    {
        dynamicParameters.Add(parameterName, parameterValue, DbType.DateTime, ParameterDirection.Input);
        return dynamicParameters;
    }
    #endregion

    #region Outputs
    public static DynamicParameters AddOutParamString(this DynamicParameters dynamicParameters, string parameterName, int size)
    {
        dynamicParameters.Add(parameterName, dbType: DbType.String, direction: ParameterDirection.Output, size: size);
        return dynamicParameters;
    }

    public static DynamicParameters AddOutParamInt(this DynamicParameters dynamicParameters, string parameterName)
    {
        dynamicParameters.Add(parameterName, dbType: DbType.Int32, direction: ParameterDirection.Output);
        return dynamicParameters;
    }

    public static DynamicParameters AddOutParamBit(this DynamicParameters dynamicParameters, string parameterName)
    {
        dynamicParameters.Add(parameterName, dbType: DbType.Boolean, direction: ParameterDirection.Output, size: 1);
        return dynamicParameters;
    }
    #endregion

    #region ReturnValues
    public static DynamicParameters AddRetParamInt(this DynamicParameters dynamicParameters, string parameterName)
    {
        dynamicParameters.Add(parameterName, dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        return dynamicParameters;
    }
    #endregion
}