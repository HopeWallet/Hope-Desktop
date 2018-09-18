using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For changing the fast and slow text on a transaction speed slider
/// </summary>
public sealed class SliderManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI slowText, fastText;

	/// <summary>
	/// Adds the listener for the sliders
	/// </summary>
	private void Awake() => transform.GetComponent<Slider>().onValueChanged.AddListener(SliderChanged);

	/// <summary>
	/// Changes text colors depending on the value
	/// </summary>
	/// <param name="value"> The current value of the slider</param>
	private void SliderChanged(float value)
	{
		ModifyText(slowText, ((1 - value) * 0.4f) + 0.6f, 1 - value);
		ModifyText(fastText, (value * 0.4f) + 0.6f, value);
	}

	/// <summary>
	/// Animates the given text's color depending on the value
	/// </summary>
	/// <param name="text"> The text component </param>
	/// <param name="colorValue"> The value of color </param>
	/// <param name="value"> The value of the slider </param>
	private void ModifyText(TextMeshProUGUI text, float colorValue, float value)
	{
		text.gameObject.AnimateColor(new Color(colorValue, colorValue, colorValue), 0.05f);
		text.fontSize = (5f * value) + 15f;
	}
}
