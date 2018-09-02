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
        public Material blurMaterial;
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
}