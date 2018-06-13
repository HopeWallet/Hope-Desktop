using System;
using UnityEngine;

/// <summary>
/// Class which manages exceptions which are possibly produced from ethereum transactions.
/// </summary>
public class ExceptionManager
{

    /// <summary>
    /// Displays an ethereum transaction exception.
    /// For now, prints the message. Later on it will likely display a gui popup containing the error message.
    /// </summary>
    /// <param name="exception"> The exception to display. </param>
    /// <param name="popupManager"> The PopupManager to use to close the active popups. </param>
    public static void DisplayException(Exception exception, PopupManager popupManager = null)
    {
        Debug.Log(exception.Message);
        popupManager?.CloseActivePopup();
    }
}
