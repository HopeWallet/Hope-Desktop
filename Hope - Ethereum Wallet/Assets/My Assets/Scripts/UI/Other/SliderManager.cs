using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For changing the fast and slow text on a transaction speed slider
/// </summary>
public sealed class SliderManager : MonoBehaviour
{
	[SerializeField] private GameObject slowText, fastText;

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
		ModifyText(slowText, ((1 - value) * 0.4f) + 0.6f);
		ModifyText(fastText, (value * 0.4f) + 0.6f);
	}

	/// <summary>
	/// Animates the given text's color depending on the value
	/// </summary>
	/// <param name="text"> The text object </param>
	/// <param name="value"> The value of color </param>
	private void ModifyText(GameObject text, float value) => text.AnimateColor(new Color(value, value, value), 0.05f);
}
