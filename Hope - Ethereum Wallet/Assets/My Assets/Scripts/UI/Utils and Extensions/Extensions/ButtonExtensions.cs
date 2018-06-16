using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ButtonExtensions
{

    /// <summary>
    /// Presses the button and displays the button press graphics.
    /// </summary>
    /// <param name="button"> The button to press. </param>
    public static void Press(this Button button) => button.OnSubmit(new BaseEventData(EventSystem.current));

    /// <summary>
    /// Gets the bounds of a button.
    /// </summary>
    /// <param name="button"> The button to get the bounds for. </param>
    /// <returns> The bounds of the button. </returns>
    public static Rect GetButtonBounds(this Button button)
    {
        var rectTransform = button.GetComponent<RectTransform>();
        var position = rectTransform.position;
        var rect = rectTransform.rect;

        return new Rect(new Vector2(position.x - rect.width / 2, position.y - rect.height / 2), rect.size);
    }

}