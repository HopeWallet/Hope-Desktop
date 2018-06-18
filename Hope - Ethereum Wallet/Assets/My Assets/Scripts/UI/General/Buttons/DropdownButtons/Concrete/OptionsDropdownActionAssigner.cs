using UnityEngine;

/// <summary>
/// Class which assigns the actions for each dropdown button in the options menu.
/// </summary>
public class OptionsDropdownActionAssigner
{

    /// <summary>
    /// Adds all the actions for the options dropdowns.
    /// </summary>
    /// <param name="uiSettings"> The ui settings which contains the dropdown info. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    public OptionsDropdownActionAssigner(UIManager.Settings uiSettings, PopupManager popupManager)
    {
        uiSettings.generalSettings.dropdowns.extraOptionsDropdowns[0].onClickAction = () => popupManager.GetPopup<PRPSHodlPopup>();
    }
}