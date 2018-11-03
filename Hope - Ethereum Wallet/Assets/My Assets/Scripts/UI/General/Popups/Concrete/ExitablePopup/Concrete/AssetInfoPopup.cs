using TMPro;
using UnityEngine;

/// <summary>
/// The asset info popup
/// </summary>
public class AssetInfoPopup : ExitablePopupComponent<AssetInfoPopup>
{
	[SerializeField] private TextMeshProUGUI formTitle;

	/// <summary>
	/// Sets the title text according to the symbol of the asset
	/// </summary>
	/// <param name="assetSymbol"> The asset symbol </param>
	public void SetDetails(string assetSymbol)
	{
		formTitle.text = assetSymbol + " Info";
	}
}
