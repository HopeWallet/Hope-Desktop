using System;
using System.Linq;
using UnityEngine;
using Zenject;

/// <summary>
/// Class which installs all dependencies.
/// </summary>
public class AppInstaller : MonoInstaller<AppInstaller>
{

    public AppSettingsInstaller appSettings;
    public TradableAssetButtonManager.Settings tradableAssetButtonSettings;
    public EthereumTransactionButtonManager.Settings transactionButtonSettings;
    public UIManager.UIProvider uiProvider;

    /// <summary>
    /// Installs all bindings.
    /// </summary>
    public override void InstallBindings()
    {
        BindSettings();
        BindSingletonTypes();
        BindUniqueInstances();
        BindScriptableObjectTypes();
        BindFactories();
    }

    /// <summary>
    /// Binds all settings dependencies for certain classes.
    /// </summary>
    private void BindSettings()
    {
        Container.BindInstance(tradableAssetButtonSettings).AsSingle().NonLazy();
        Container.BindInstance(transactionButtonSettings).AsSingle().NonLazy();
        Container.BindInstance(uiProvider).AsSingle().NonLazy();
    }

    /// <summary>
    /// Binds all scriptable object types.
    /// </summary>
    private void BindScriptableObjectTypes()
    {
        // Bind all contracts.
        Resources.LoadAll<FixedContractBase>("").Where(contract => contract.NetworkType == appSettings.ethereumNetworkSettings.networkType)
                                        .ForEach(contract => Container.BindInstance(contract.CreateContract()).AsSingle().NonLazy());
    }

    /// <summary>
    /// Binds unique instances of all the types to the classes that need them.
    /// </summary>
    private void BindUniqueInstances()
    {
        Container.Bind<TransactionHelper>().AsTransient();
    }

    /// <summary>
    /// Binds all singleton types to their required dependencies.
    /// </summary>
    private void BindSingletonTypes()
    {
        Container.BindInterfacesAndSelfTo<UpdateManager>().AsSingle().NonLazy();
        Container.Bind<PeriodicUpdateManager>().AsSingle().NonLazy();

        Container.Bind<DebugManager>().AsSingle().NonLazy();

        Container.Bind<TradableAssetManager>().AsSingle().NonLazy();
        Container.Bind<TradableAssetButtonManager>().AsSingle().NonLazy();
        Container.Bind<TradableAssetImageManager>().AsSingle().NonLazy();

        Container.Bind<EthereumTransactionManager>().AsSingle().NonLazy();
        Container.Bind<EthereumTransactionButtonManager>().AsSingle().NonLazy();

        Container.Bind<UserWalletManager>().AsSingle().NonLazy();

        Container.Bind<TokenContractManager>().AsSingle().NonLazy();

        Container.Bind<EthereumNetworkManager>().AsSingle().NonLazy();
        Container.Bind<GasPriceObserver>().AsSingle().NonLazy();

        Container.BindInstance(GetComponent<UIManager>()).AsSingle().NonLazy();
        Container.Bind<OptionsDropdownActionAssigner>().AsSingle().NonLazy();
        Container.Bind<MenuFactoryManager>().AsSingle().NonLazy();
        Container.Bind<PopupManager>().AsSingle().NonLazy();
        Container.Bind<ButtonClickObserver>().AsSingle().NonLazy();
        Container.Bind<MouseClickObserver>().AsSingle().NonLazy();
    }

    /// <summary>
    /// Binds the required factories to their prefabs and spawn transforms.
    /// </summary>
    private void BindFactories()
    {
        BindButtonFactories();
        BindPopupFactories();
        BindMenuFactories();
    }

    /// <summary>
    /// Binds the factories for the buttons.
    /// </summary>
    private void BindButtonFactories()
    {
        BindButtonFactory<TransactionInfoButton>(transactionButtonSettings.spawnTransform);
        BindButtonFactory<AssetButton>(tradableAssetButtonSettings.spawnTransform);
        BindButtonFactory<LockedPRPSItemButton>(null);
    }

    /// <summary>
    /// Binds the factories for the popups.
    /// </summary>
    private void BindPopupFactories()
    {
        BindPopupFactory<LoadingPopup>();
        BindPopupFactory<AddTokenPopup>();
        BindPopupFactory<SendAssetPopup>();
        BindPopupFactory<ConfirmSendAssetPopup>();
        BindPopupFactory<HideAssetPopup>();
        BindPopupFactory<ConfirmHideAssetPopup>();
        BindPopupFactory<ReceiveAssetPopup>();
        BindPopupFactory<TransactionInfoPopup>();
        BindPopupFactory<PRPSLockPopup>();
        BindPopupFactory<ConfirmPRPSLockPopup>();
        BindPopupFactory<GeneralTransactionConfirmationPopup>();
    }

    /// <summary>
    /// Binds the factories for the ui menus.
    /// </summary>
    private void BindMenuFactories()
    {
        BindMenuFactory<CreatePasswordMenu>();
        BindMenuFactory<ImportOrCreateMenu>();
        BindMenuFactory<CreateWalletMenu>();
        BindMenuFactory<ImportWalletMenu>();
        BindMenuFactory<UnlockWalletMenu>();
        BindMenuFactory<OpenWalletMenu>();
    }

    /// <summary>
    /// Binds the types for a menu factory.
    /// </summary>
    /// <typeparam name="TMenu"> The type of the menu. </typeparam>
    private void BindMenuFactory<TMenu>() where TMenu : Menu<TMenu> => BindFactory<TMenu, Menu<TMenu>.Factory>(uiProvider.uiRoot.transform);

    /// <summary>
    /// Binds the types for a button factory.
    /// </summary>
    /// <typeparam name="TButton"> The type of the button. </typeparam>
    /// <param name="spawnTransform"> The spawn transform to have the factory create the buttons under. </param>
    private void BindButtonFactory<TButton>(Transform spawnTransform) where TButton : FactoryButton<TButton> => BindFactory<TButton, FactoryButton<TButton>.Factory>(spawnTransform);

    /// <summary>
    /// Binds the types for a popup factory.
    /// </summary>
    /// <typeparam name="TPopup"> The type of the popup. </typeparam>
    private void BindPopupFactory<TPopup>() where TPopup : FactoryPopup<TPopup> => BindFactory<TPopup, FactoryPopup<TPopup>.Factory>(uiProvider.uiRoot.transform);

    /// <summary>
    /// Binds a certain factory type under a spawn transform.
    /// </summary>
    /// <typeparam name="TType"> The type of the object we are creating. </typeparam>
    /// <typeparam name="TFactory"> The type of the factory being created. </typeparam>
    /// <param name="spawnTransform"> The transform where the factory will create objects under. </param>
    private void BindFactory<TType, TFactory>(Transform spawnTransform) where TType : MonoBehaviour where TFactory : Factory<TType>
    {
        Container.BindFactory<TType, TFactory>()
                 .FromComponentInNewPrefab(Resources.LoadAll<TType>("")[0])
                 .UnderObject(spawnTransform);
    }

}