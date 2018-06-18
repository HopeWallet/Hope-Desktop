using System;

namespace UISettings
{

    /// <summary>
    /// Class which contains general UI settings.
    /// </summary>
    [Serializable]
    public class GeneralSettings
    {
        public Dropdowns dropdowns;
    }

    /// <summary>
    /// Class which contains all dropdowns.
    /// </summary>
    [Serializable]
    public class Dropdowns
    {
        public DropdownButtonInfo[] optionsDropdowns;
        public AssetDropdownButtonInfo[] extraOptionsDropdowns;
    }
}