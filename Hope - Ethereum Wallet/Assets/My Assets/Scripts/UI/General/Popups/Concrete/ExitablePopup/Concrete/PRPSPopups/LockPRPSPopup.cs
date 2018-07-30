using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

public sealed class LockPRPSPopup : ExitablePopupComponent<LockPRPSPopup>
{
	public InfoMessage infoMessage;

    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }
}