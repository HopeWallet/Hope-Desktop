using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class ModifyTokensPopup : ExitablePopupComponent<ModifyTokensPopup>
{

    [SerializeField] private Button addCustomToken;
	[SerializeField] private TMP_InputField searchBar;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void OnStart()
    {
		addCustomToken.onClick.AddListener(AddCustomToken);
		searchBar.onValueChanged.AddListener(SearchInputChanged);
	}

    private void AddCustomToken()
    {
        popupManager.GetPopup<AddTokenPopup>(true);
    }

    private void SearchInputChanged(string search)
	{
	}
}