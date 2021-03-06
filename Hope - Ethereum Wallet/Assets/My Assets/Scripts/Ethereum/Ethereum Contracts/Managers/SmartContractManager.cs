﻿using System;

/// <summary>
/// Class which manages the different static smart contracts and their settings.
/// </summary>
public static class SmartContractManager
{
    /// <summary>
    /// The settings for all smart contracts.
    /// </summary>
    [Serializable]
    public sealed class Settings
    {
        public Hodler.Settings hodlerSettings;
        public PRPS.Settings prpsSettings;
        public DUBI.Settings dubiSettings;
    }
}