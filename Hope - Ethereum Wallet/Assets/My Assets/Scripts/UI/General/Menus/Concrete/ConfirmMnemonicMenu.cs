using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public sealed class ConfirmMnemonicMenu : WalletLoadMenuBase<ConfirmMnemonicMenu>
{

    public Button backButton;

    private void Start()
    {
        backButton.onClick.AddListener(GoBack);
    }

    public override void LoadWallet()
    {

    }
}
