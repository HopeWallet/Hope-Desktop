using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

public class PRPSHodlPopup : ExitablePopupComponent<PRPSHodlPopup>
{

    private readonly List<HodlerItem> items = new List<HodlerItem>();

    private HodlerContract hodlerContract;
    private UserWalletManager userWalletManager;
    private EthereumNetwork ethereumNetwork;

    [Inject]
    public void Construct(HodlerContract hodlerContract, UserWalletManager userWalletManager, EthereumNetworkManager ethereumNetworkManager)
    {
        this.hodlerContract = hodlerContract;
        this.userWalletManager = userWalletManager;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;
    }

    private void OnEnable()
    {
    }

}