using System;
using System.Collections.Generic;
using System.Linq;
using UISettings;
using UnityEngine;
using Zenject;

/// <summary>
/// Class which contains basic info of the root object where all UI elements are children of.
/// </summary>
public class UIManager : MonoBehaviour, IEscapeButtonObserver
{

    [SerializeField]
    [Tooltip("Extra menus to use with the UIManager, except they will not be instantiated.")]
    private Menu[] extraMenus; // Extra menus that cannot be instantiated, but still need to be used in the UIManager.

    private Settings settings;
    private UIProvider uiProvider;
    private MenuFactoryManager menuFactoryManager;
    private PopupManager popupManager;
    private UserWalletManager userWalletManager;

    private readonly Stack<Menu> menus = new Stack<Menu>();
    private readonly List<Menu> createdMenus = new List<Menu>();

    /// <summary>
    /// Initializes the UIManager with all the required dependencies.
    /// </summary>
    /// <param name="settings"> The settings to use with this UIManager. </param>
    /// <param name="uiProvider"> The provider to use to retrieve the root gameobject of the ui. </param>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="menuFactoryManager"> The active MenuFactoryManager which is used to create menus of certain types. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void Construct(Settings settings,
        UIProvider uiProvider,
        ButtonObserver buttonObserver,
        PopupManager popupManager,
        MenuFactoryManager menuFactoryManager,
        UserWalletManager userWalletManager)
    {
        this.settings = settings;
        this.uiProvider = uiProvider;
        this.popupManager = popupManager;
        this.menuFactoryManager = menuFactoryManager;
        this.userWalletManager = userWalletManager;

        buttonObserver.AddEscapeButtonObserver(this);
    }

    /// <summary>
    /// Starts the UIManager by choosing the first menu to open.
    /// </summary>
    private void Start()
    {
        createdMenus.AddItems(extraMenus);

        if (userWalletManager.CanReadWallet())
            OpenMenu<UnlockWalletMenu>();
        else
            OpenMenu<CreatePasswordMenu>();
    }

    /// <summary>
    /// Closes the active popup if one is open, if not, calls the back button on the active menu.
    /// </summary>
    /// <param name="clickType"> The ClickType of the escape button press. </param>
    public void EscapeButtonPressed(ClickType clickType)
    {
        if (clickType == ClickType.Down)
        {
            if (!popupManager.CloseActivePopup(typeof(LoadingPopup)))
                menus.Peek().OnBackPressed();
        }
    }

    /// <summary>
    /// Opens a menu given the type of menu to open.
    /// </summary>
    /// <typeparam name="T"> The type of menu to open. </typeparam>
    public void OpenMenu<T>() where T : Menu<T>
    {
        var sameTypeMenus = createdMenus.OfType<T>();

        if (sameTypeMenus.Count() > 0)
        {
            EnableNewMenu(sameTypeMenus.First());
            return;
        }
        else
        {
            var newMenu = menuFactoryManager.CreateMenu<T>();
            EnableNewMenu(newMenu);
            createdMenus.Add(newMenu);
        }
    }

    /// <summary>
    /// Closes the menu at the very top of the stack.
    /// </summary>
    public void CloseMenu()
    {
        if (menus.Count == 0)
            return;

        var menuToRemove = menus.Pop();

        if (menuToRemove.DestroyWhenClosed)
        {
            Destroy(menuToRemove.gameObject);
            createdMenus.Remove(menuToRemove);
        }
        else
        {
            menuToRemove.gameObject.SetActive(false);
        }

        foreach (Menu menu in menus)
        {
            menu.gameObject.SetActive(true);

            if (menu.DisableMenusUnderneath)
                break;
        }
    }

    /// <summary>
    /// Enables a new menu and disables all menus below if required.
    /// </summary>
    /// <param name="newMenu"> The new menu to enable. </param>
    private void EnableNewMenu(Menu newMenu)
    {
        newMenu.gameObject.SetActive(true);

        if (newMenu.DisableMenusUnderneath)
            menus.ForEach(menu => menu.gameObject.SetActive(false));

        if (menus.Count > 0)
        {
            var topCanvas = newMenu.GetComponent<Canvas>();
            var previousCanvas = menus.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }

        menus.Push(newMenu);
    }

    /// <summary>
    /// Class which provides the root gameobject of the ui.
    /// </summary>
    [Serializable]
    public class UIProvider
    {
        public GameObject uiRoot;
    }

    /// <summary>
    /// Class which holds the main settings of the ui as a whole.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public GeneralSettings generalSettings;
        public UIPrefabs uiPrefabs;
    }
}
