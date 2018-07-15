using System;
using UnityEngine;

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
    /// Class which contains the different menus to use in the UI.
    /// </summary>
    [Serializable]
    public class MenuSettings
    {
        public GameObject[] factoryButtons;
        public GameObject[] menus;
        public GameObject[] popups;
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