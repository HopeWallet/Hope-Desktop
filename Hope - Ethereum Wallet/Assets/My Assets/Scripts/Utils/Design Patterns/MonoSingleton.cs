using UnityEngine;

/// <summary>
/// Class which acts as a singleton for monobehaviour classes.
/// </summary>
/// <typeparam name="T"> The type of this singleton. </typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{

    /// <summary>
    /// The instance of the singleton monobehaviour class.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// Initializes this singleton by getting the instance if null, or destroying this component if it is not null.
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this as T;
    }

    /// <summary>
    /// Sets the instance to null once this component is destroyed.
    /// </summary>
    protected virtual void OnDestroy() => Instance = null;

}
