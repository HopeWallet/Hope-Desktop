using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ButtonExtensions
{

    /// <summary>
    /// Presses the button and displays the button press graphics.
    /// </summary>
    /// <param name="button"> The button to press. </param>
    public static void Press(this Button button) => button.OnSubmit(new BaseEventData(EventSystem.current));

}