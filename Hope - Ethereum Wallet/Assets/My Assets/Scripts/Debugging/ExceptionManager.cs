using System;
using UnityEngine;

/// <summary>
/// Class which manages exceptions which are possibly produced from ethereum transactions.
/// </summary>
public class ExceptionManager
{
    private static PopupManager PopupManager;

    /// <summary>
    /// Initializes the ExceptionManager with the required PopupManager.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    public ExceptionManager(PopupManager popupManager)
    {
        PopupManager = popupManager;
    }

    /// <summary>
    /// Displays an ethereum transaction exception.
    /// For now, prints the message. Later on it will likely display a gui popup containing the error message.
    /// </summary>
    /// <param name="exception"> The exception to display. </param>
    public static void DisplayException(Exception exception)
    {
        Debug.Log(exception.Message);
        //PopupManager?.CloseActivePopup();
    }
}
