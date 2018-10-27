using System;
using UnityEngine;

namespace Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base
{
    /// <summary>
    /// Class which holds the SecurePlayerPrefs settings.
    /// </summary>
    [Serializable]
    public sealed class SecurePlayerPrefsSettings : ScriptableObject
    {
        [SerializeField] public string securePlayerPrefSeed;
        [SerializeField] public string securePlayerPrefDataEntropy;
    }
}