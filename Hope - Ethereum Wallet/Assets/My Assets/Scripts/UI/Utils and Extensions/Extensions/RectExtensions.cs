using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class which contains extension methods for the RectTransform component.
/// </summary>
public static class RectExtensions
{

    /// <summary>
    /// Gets the bounds of the <see cref="RectTransform"/>.
    /// Can be used to check if other ui components are within these bounds.
    /// </summary>
    /// <param name="rectTransform"> The <see cref="RectTransform"/> to get the bounds for. </param>
    /// <returns> The <see cref="Rect"/> which represents the full bounds of the <see cref="RectTransform"/>. </returns>
    public static Rect GetBounds(this RectTransform rectTransform)
    {
        var position = rectTransform.position;
        var rect = rectTransform.rect;

        return new Rect(new Vector2(position.x - (rect.width / 2), position.y - (rect.height / 2)), rect.size);
    }

}