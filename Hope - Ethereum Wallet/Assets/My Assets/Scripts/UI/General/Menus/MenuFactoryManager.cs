﻿using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class which manages the factories and creation of certain menus.
/// </summary>
public sealed class MenuFactoryManager
{
    private readonly List<object> menuFactories = new List<object>();

    /// <summary>
    /// Initializes all the menu factories.
    /// </summary>
    public MenuFactoryManager(ImportOrCreateMnemonicMenu.Factory importOrCreateMenuFactory,
        CreateMnemonicMenu.Factory walletCreateMenuFactory,
        ImportMnemonicMenu.Factory walletImportMenuFactory,
        WalletListMenu.Factory walletUnlockMenuFactory,
        OpenWalletMenu.Factory openedWalletMenuFactory,
        ChooseWalletMenu.Factory chooseWalletMenuFactory,
        CreateWalletMenu.Factory createWalletMenuFactory,
        ConfirmMnemonicMenu.Factory confirmMnemonicMenuFactory,
        WalletCreatedMenu.Factory walletCreatedMenuFactory,
        OpenLedgerWalletMenu.Factory openLedgerWalletMenuFactory,
        OpenTrezorWalletMenu.Factory openTrezorWalletMenuFactory,
		ReEnterPasswordMenu.Factory reEnterPasswordMenuFactory)
    {
        menuFactories.AddItems(importOrCreateMenuFactory,
                               walletCreateMenuFactory,
                               walletImportMenuFactory,
                               walletUnlockMenuFactory,
                               openedWalletMenuFactory,
                               chooseWalletMenuFactory,
                               createWalletMenuFactory,
                               confirmMnemonicMenuFactory,
                               walletCreatedMenuFactory,
                               openLedgerWalletMenuFactory,
                               openTrezorWalletMenuFactory,
							   reEnterPasswordMenuFactory);
    }

    /// <summary>
    /// Creates a new menu given the type.
    /// </summary>
    /// <typeparam name="T"> The type of menu to create. </typeparam>
    /// <returns> The newly created menu. </returns>
    public T CreateMenu<T>() where T : Menu<T> => menuFactories.OfType<Menu<T>.Factory>().First().Create();
}