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
    /// Class which contains all prefabs needed to be dynamically created.
    /// </summary>
    [Serializable]
    public class UIPrefabs
    {
        public Buttons buttons;
        public Popups popups;
        public Menus menus;
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

    /// <summary>
    /// Class which holds buttons used and instantiated in the ui.
    /// </summary>
    [Serializable]
    public class Buttons
    {
        public GameObject assetButton;
        public GameObject transactionButton;
    }

    /// <summary>
    /// Class which contains the different popup prefabs.
    /// </summary>
    [Serializable]
    public class Popups
    {
        public GameObject addTokenPopup;
        public GameObject confirmHideAssetPopup;
        public GameObject hideAssetPopup;
        public GameObject loadingPopup;
        public GameObject receiveAssetPopup;
        public GameObject sendAssetPopup;
        public GameObject sendAssetConfirmationPopup;
        public GameObject transactionInfoPopup;
    }

    /// <summary>
    /// Class which contains the different menus for the ui.
    /// </summary>
    [Serializable]
    public class Menus
    {
        public GameObject importOrCreateMenu;
        public GameObject createPasswordMenu;
        public GameObject walletCreateMenu;
        public GameObject walletImportMenu;
        public GameObject walletUnlockMenu;
        public GameObject openedWalletMenu;
    }

}