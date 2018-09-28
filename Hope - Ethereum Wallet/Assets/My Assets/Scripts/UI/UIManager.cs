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

    private MenuFactoryManager menuFactoryManager;
    private PopupManager popupManager;

    private readonly Stack<Menu> menus = new Stack<Menu>();
    private readonly List<Menu> createdMenus = new List<Menu>();

    private Menu closingMenu,
                 menuToDestroy;

    /// <summary>
    /// The actively opened menu type.
    /// </summary>
    public Type ActiveMenuType => menus.Count > 0 ? menus.Peek().GetType() : null;

    /// <summary>
    /// Initializes the UIManager with all the required dependencies.
    /// </summary>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="menuFactoryManager"> The active MenuFactoryManager which is used to create menus of certain types. </param>
    /// <param name="settings"> The settings for the ui. </param>
    [Inject]
    public void Construct(ButtonClickObserver buttonObserver,
        PopupManager popupManager,
        MenuFactoryManager menuFactoryManager,
        Settings settings)
    {
        this.popupManager = popupManager;
        this.menuFactoryManager = menuFactoryManager;

        settings.generalSettings.blurMaterial.SetFloat("_Size", 1.2f);
        buttonObserver.SubscribeObservable(this);
    }

    private void Awake()
    {
        SetScreenResolution();
    }

    /// <summary>
    /// Starts the UIManager by choosing the first menu to open.
    /// </summary>
    private void Start()
    {
        createdMenus.AddRange(extraMenus);

        SetDefaultSettings();
        OpenMenu<ChooseWalletMenu>();
    }

    private void Update()
    {
        if (menuToDestroy != null && menus.Count > 0 && menuToDestroy != menus.Peek())
        {
            Destroy(menuToDestroy.gameObject);
            menuToDestroy = null;
        }
    }

    /// <summary>
    /// Sets the default setting preferences if there is nothing saved under it already
    /// </summary>
    private void SetDefaultSettings()
    {
        if (!SecurePlayerPrefs.HasKey("idle timeout"))
        {
            SecurePlayerPrefs.SetBool("idle timeout", true);
            SecurePlayerPrefs.SetInt("idle time", 5);
            SecurePlayerPrefs.SetBool("countdown timer", true);
            SecurePlayerPrefs.SetBool("transaction notification", true);
            SecurePlayerPrefs.SetBool("update notification", true);
            SecurePlayerPrefs.SetBool("two-factor authentication", false);
            SecurePlayerPrefs.SetBool("2FA set up", false);
        }
    }

    /// <summary>
    /// Sets the default screen size
    /// </summary>
    private void SetScreenResolution()
    {
        int screenWidth = (int)(Screen.currentResolution.width * 0.666666666f);
        int screenHeight = (int)(Screen.currentResolution.height * 0.7407f);

        Screen.SetResolution(screenWidth, screenHeight, false, 120);
    }

    /// <summary>
    /// Closes the active popup if one is open, if not, calls the back button on the active menu.
    /// </summary>
    /// <param name="clickType"> The ClickType of the escape button press. </param>
    public void EscapeButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (popupManager.AnimatingPopup || popupManager.CloseActivePopup(typeof(LoadingPopup), typeof(InfoPopup)) || popupManager.ActivePopupExists)
            return;

        if (closingMenu?.Animator.Animating != true)
            menus.Peek().GoBack();
    }

    /// <summary>
    /// Destroys all menus that are not currently in use.
    /// </summary>
    public void DestroyUnusedMenus()
    {
        List<Menu> menuCache = new List<Menu>(createdMenus);
        Menu currentMenu = menus.Peek();

        createdMenus.Clear();
        menus.Clear();

        menuCache.Where(menu => !extraMenus.Contains(menu) && menu != currentMenu).ForEach(menu => Destroy(menu.gameObject));
        menuToDestroy = menuCache.Find(menu => !extraMenus.Contains(menu) && menu == currentMenu);

        createdMenus.AddRange(extraMenus);
    }

    /// <summary>
    /// Opens a menu given the type of menu to open.
    /// </summary>
    /// <typeparam name="T"> The type of menu to open. </typeparam>
    public void OpenMenu<T>() where T : Menu<T>
    {
        Menu lastMenu = null;
        if (menus.Count > 0)
            lastMenu = menus.Peek();

        Action openMenuAction = () =>
        {
            if (lastMenu != null)
                lastMenu.gameObject.SetActive(false);

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

        if (lastMenu != null)
            lastMenu.Animator.AnimateDisable(openMenuAction);
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
        public MenuSettings menuSettings;
    }
}
