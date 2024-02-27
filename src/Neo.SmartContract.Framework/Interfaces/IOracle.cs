using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of oracle callback method
/// </summary>
public interface IOracle
{
    /// <summary>
    /// This method is called after the Oracle receives response from requested URL
    /// </summary>
    /// <param name="requestedUrl">The url of the data source</param>
    /// <param name="userData">Extra oracle request data specified by user</param>
    /// <param name="oracleResponse">Oracle response code <see cref="OracleResponseCode"/> from data source</param>
    /// <param name="jsonString">The oracle response data in format of json string</param>
    ///
    /// <example>
    /// public static void OnOracleResponse(string url, object userData, OracleResponseCode code, string result)
    /// {
    ///     // This check ensures that this method can only be called by the native oracle contract
    ///     // where <see cref="Runtime.CallingScriptHash"/> returns the hash of the native oracle contract here
    ///     if (Runtime.CallingScriptHash != Oracle.Hash) throw new Exception("Unauthorized!");
    ///     Storage.Put(Storage.CurrentContext, PreData, result);
    /// }
    /// </example>
    public void OnOracleResponse(
        string requestedUrl,
        object userData,
        OracleResponseCode oracleResponse,
        string jsonString);
}
