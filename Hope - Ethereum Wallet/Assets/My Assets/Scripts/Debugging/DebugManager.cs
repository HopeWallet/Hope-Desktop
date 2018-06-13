using System;
using Zenject;

/// <summary>
/// Class for managing the enabling and disabling of various debugging techniques.
/// </summary>
public class DebugManager : InjectableSingleton<DebugManager>
{

    public readonly Settings settings;

    /// <summary>
    /// Initializes the DebugManager.
    /// </summary>
    /// <param name="settings"> The settings of the DebugManager. </param>
    [Inject]
    public DebugManager(Settings settings) : base()
    {
        this.settings = settings;
    }

    /// <summary>
    /// Class which contains the settings for this manager.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public bool displayNethereumDebugStatements;
    }

}
