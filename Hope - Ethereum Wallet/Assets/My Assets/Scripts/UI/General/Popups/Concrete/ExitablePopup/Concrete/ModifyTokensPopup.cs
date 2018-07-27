using UnityEngine;
using UnityEngine.UI;

public sealed class ModifyTokensPopup : ExitablePopupComponent<ModifyTokensPopup>
{

    [SerializeField] private Button addCustomToken;

    protected override void OnStart()
    {
        addCustomToken.onClick.AddListener(AddCustomToken);
    }

    private void AddCustomToken()
    {
        popupManager.GetPopup<AddTokenPopup>(true);
    }
}