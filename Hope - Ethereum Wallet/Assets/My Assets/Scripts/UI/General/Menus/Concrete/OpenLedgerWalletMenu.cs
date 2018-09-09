using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLedgerWalletMenu : WalletLoadMenuBase<OpenLedgerWalletMenu>
{
    public event Action OnLedgerConnected;
    public event Action OnLedgerDisconnected;
    public override event Action OnWalletLoading;

    public override void LoadWallet()
    {
    }
}