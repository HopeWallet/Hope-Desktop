using System;

/// <summary>
/// Class which represents the json object for a token transfer.
/// </summary>
[Serializable]
public class TokenTransactionJson
{

    public string[] topics;
    public string address;
    public string data;
    public string blockNumber;
    public string timeStamp;
    public string gasPrice;
    public string gasUsed;
    public string logIndex;
    public string transactionHash;
    public string transactionIndex;

}