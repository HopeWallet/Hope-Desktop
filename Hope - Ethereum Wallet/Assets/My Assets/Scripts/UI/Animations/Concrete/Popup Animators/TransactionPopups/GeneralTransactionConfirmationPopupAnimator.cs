using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class GeneralTransactionConfirmationPopupAnimator : UIAnimator
{
    protected override void AnimateIn()
    {
        FinishedAnimating();
    }

    protected override void AnimateOut()
    {
        FinishedAnimating();
    }
}