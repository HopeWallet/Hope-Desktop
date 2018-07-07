using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class which manages the factories and creation of certain menus.
/// </summary>
public class MenuFactoryManager
{

    private List<object> menuFactories = new List<object>();

    /// <summary>
    /// Initializes all the menu factories.
    /// </summary>
    public MenuFactoryManager(CreatePasswordMenu.Factory createPasswordMenuFactory,
        ImportOrCreateMnemonicMenu.Factory importOrCreateMenuFactory,
        CreateMnemonicMenu.Factory walletCreateMenuFactory,
        ImportWalletMenu.Factory walletImportMenuFactory,
        UnlockWalletMenu.Factory walletUnlockMenuFactory,
        OpenWalletMenu.Factory openedWalletMenuFactory,
        ChooseWalletMenu.Factory chooseWalletMenuFactory,
        CreateWalletMenu.Factory createWalletMenuFactory)
    {
        menuFactories.AddItems(createPasswordMenuFactory,
                               importOrCreateMenuFactory,
                               walletCreateMenuFactory,
                               walletImportMenuFactory,
                               walletUnlockMenuFactory,
                               openedWalletMenuFactory,
                               chooseWalletMenuFactory,
                               createWalletMenuFactory);
    }

    /// <summary>
    /// Creates a new menu given the type.
    /// </summary>
    /// <typeparam name="T"> The type of menu to create. </typeparam>
    /// <returns> The newly created menu. </returns>
    public T CreateMenu<T>() where T : Menu<T> => menuFactories.OfType<Menu<T>.Factory>().First().Create();

}
