using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class LockPRPSManager
{
    private readonly PRPS prpsContract;
    private readonly DUBI dubiContract;

    public LockPRPSManager(PRPS prpsContract, DUBI dubiContract)
    {
        this.prpsContract = prpsContract;
        this.dubiContract = dubiContract;
    }
}