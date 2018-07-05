using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletLoaderBase
{

    protected readonly PopupManager popupManager;
    protected readonly PlayerPrefPassword playerPrefPassword;
    protected readonly ProtectedStringDataCache protectedStringDataCache;

    protected ProtectedString[] addresses;

    protected Action onWalletLoaded;

    protected WalletLoaderBase(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
    }
}
