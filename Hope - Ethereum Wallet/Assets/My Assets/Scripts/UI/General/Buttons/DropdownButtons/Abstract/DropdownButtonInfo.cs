using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class which contains the info for each button in a dropdown button.
/// </summary>
[Serializable]
public class DropdownButtonInfo
{
    public UnityAction onClickAction;
    public Sprite buttonImage;
    public string buttonText;
}