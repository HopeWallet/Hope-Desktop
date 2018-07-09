using System;
using System.Collections.Generic;
using System.Linq;
using UISettings;
using UnityEngine;
using Zenject;

/// <summary>
/// Class which contains basic info of the root object where all UI elements are children of.
/// </summary>
public class UIManager : MonoBehaviour, IEscapeButtonObservable
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

    private Menu closingMenu;

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
        ButtonClickObserver buttonObserver,
        PopupManager popupManager,
        MenuFactoryManager menuFactoryManager,
        UserWalletManager userWalletManager)
    {
        this.settings = settings;
        this.uiProvider = uiProvider;
        this.popupManager = popupManager;
        this.menuFactoryManager = menuFactoryManager;
        this.userWalletManager = userWalletManager;

        buttonObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Starts the UIManager by choosing the first menu to open.
    /// </summary>
    private void Start()
    {
        createdMenus.AddItems(extraMenus);

        OpenMenu<ChooseWalletMenu>();
    }

    /// <summary>
    /// Closes the active popup if one is open, if not, calls the back button on the active menu.
    /// </summary>
    /// <param name="clickType"> The ClickType of the escape button press. </param>
    public void EscapeButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (popupManager.CloseActivePopup(typeof(LoadingPopup)))
            return;

        if (closingMenu?.Animator.Animating != true)
            menus.Peek().GoBack();
    }

    /// <summary>
    /// Opens a menu given the type of menu to open.
    /// </summary>
    /// <typeparam name="T"> The type of menu to open. </typeparam>
    public void OpenMenu<T>() where T : Menu<T>
    {
        Action openMenuAction = () =>
        {
            if (menus.Count > 0)
                menus.Peek().gameObject.SetActive(false);

            var sameTypeMenus = createdMenus.OfType<T>();

            if (sameTypeMenus.Any())
            {
                EnableNewMenu(sameTypeMenus.Single());
            }
            else
            {
                var newMenu = menuFactoryManager.CreateMenu<T>();
                EnableNewMenu(newMenu);
                createdMenus.Add(newMenu);
            }
        };

        if (menus.Count > 0)
            menus.Peek().Animator.AnimateDisable(openMenuAction);
        else
            openMenuAction();
    }

    /// <summary>
    /// Closes the menu at the very top of the stack.
    /// </summary>
    public void CloseMenu()
    {
        if (menus.Count == 0)
            return;

        closingMenu = menus.Pop();

        closingMenu.Animator.AnimateDisable(() =>
        {
            if (closingMenu.DestroyWhenClosed)
            {
                Destroy(closingMenu.gameObject);
                createdMenus.Remove(closingMenu);
            }
            else
            {
                closingMenu.gameObject.SetActive(false);
            }

            if (menus.Count > 0)
            {
                var newActiveMenu = menus.Peek();
                newActiveMenu.gameObject.SetActive(true);
                newActiveMenu.Animator.AnimateEnable();
            }

            closingMenu = null;
        });
    }

    /// <summary>
    /// Enables a new menu and disables all menus below if required.
    /// </summary>
    /// <param name="newMenu"> The new menu to enable. </param>
    private void EnableNewMenu(Menu newMenu)
    {
        if (menus.Count > 0)
        {
            var topCanvas = newMenu.GetComponent<Canvas>();
            var previousCanvas = menus.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }

        menus.Push(newMenu);

        newMenu.gameObject.SetActive(true);
        newMenu.Animator?.AnimateEnable();
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
    }
}
