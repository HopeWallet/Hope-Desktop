using System;
/// <summary>
/// Class which contains the extra dropdown info for a given asset address.
/// </summary>
[Serializable]
public class AssetDropdownButtonInfo : DropdownButtonInfo
{
    public string[] assetAddresses;
}