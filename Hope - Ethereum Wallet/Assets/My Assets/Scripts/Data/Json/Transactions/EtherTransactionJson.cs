using System;

/// <summary>
/// Class which represents the json object which contains all info about one ethereum transaction.
/// </summary>
[Serializable]
public class EtherTransactionJson
{

    public string value;
    public string gasPrice;
    public string hash;
    public string blockHash;
    public string from;
    public string to;
    public string input;
    public string contractAddress;

    public int blockNumber;
    public int timeStamp;
    public int nonce;
    public int transactionIndex;
    public int gas;
    public int isError;
    public int txreceipt_status;
    public int cumulativeGasUsed;
    public int gasUsed;
    public int confirmations;

}
