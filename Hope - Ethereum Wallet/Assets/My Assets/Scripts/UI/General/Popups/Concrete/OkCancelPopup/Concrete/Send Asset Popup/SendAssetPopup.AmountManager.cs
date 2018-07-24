using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class AmountManager
    {

        private Toggle maxToggle;

        public AmountManager(Toggle maxToggle)
        {
            this.maxToggle = maxToggle;
        }

    }
}