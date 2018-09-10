using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;
using Hope.Security.HashGeneration;
using Hope.Random.Bytes;
using Hope.Random.Strings;

/// <summary>
/// Class which derives a password by hashing a password with many different pieces of data saved to the PlayerPrefs.
/// </summary>
[Serializable]
public class PlayerPrefPasswordDerivation : ScriptableObject
{
    [SerializeField] public string[] keys;

    private readonly Dictionary<string, byte[]> prefDictionary = new Dictionary<string, byte[]>();

    private int prefCounter;

    private const int PASSWORD_LENGTH = 16;

    public byte[] Derive(byte[] startingPassword)
    {
        byte[] derivedSeed = startingPassword;
        for (int i = 0; i < keys.Length; i++)
        {
            byte[] last = derivedSeed.SHA3_512();
            byte[] newBytes = RandomBytes.Secure.Blake2.GetBytes(32);

            prefDictionary.Add(keys[i], newBytes);
            derivedSeed = last.Concat(newBytes).ToArray().Keccak_512();

            last.ClearBytes();
        }

        derivedSeed = derivedSeed.Concat(startingPassword.Blake2_512()).ToArray();
        startingPassword.ClearBytes();

        return derivedSeed.Blake2_512();
    }

    public byte[] Restore(byte[] startingPassword)
    {
        byte[] derivedSeed = startingPassword;
        for (int i = 0; i < keys.Length; i++)
        {
            byte[] last = derivedSeed.SHA3_512();
            byte[] newBytes = prefDictionary[keys[i]];

            derivedSeed = last.Concat(newBytes).ToArray().Keccak_512();

            last.ClearBytes();
        }

        derivedSeed = derivedSeed.Concat(startingPassword.Blake2_512()).ToArray();
        startingPassword.ClearBytes();

        return derivedSeed.Blake2_512();
    }

    /// <summary>
    /// Sets the player pref values to correspond with the dictionary.
    /// </summary>
    /// <param name="walletNum"> The wallet number to setup the player prefs for. </param>
    /// <param name="onPrefsSetup"> Action to call once all the PlayerPrefs were setup. </param>
    public void SetupPlayerPrefs(int walletNum, Action onPrefsSetup = null)
    {
        if (prefDictionary == null)
        {
            ExceptionManager.DisplayException(new Exception("Unable to save PlayerPrefs in the PlayerPrefPassword."));
            return;
        }

        GenerateSpoofKeys();
        GenerateCorrectKeys(walletNum, onPrefsSetup);
        prefDictionary.Clear();
    }

    /// <summary>
    /// Takes the player prefs saved in the registry and moves them into a dictionary.
    /// </summary>
    /// <param name="walletNum"> The wallet number to get the PlayerPrefs for. </param>
    /// <returns> The dictionary of player prefs. </returns>
    public void PopulatePrefDictionary(int walletNum)
    {
        keys.SafeForEach(key => prefDictionary.Add(key, SecurePlayerPrefs.GetString((key + "_" + walletNum).Keccak_256()).GetBase64Bytes()));
    }

    /// <summary>
    /// Generates the correct PlayerPref keys for the given wallet number.
    /// </summary>
    /// <param name="walletNum"> The wallet number to generate the PlayerPrefs for. </param>
    /// <param name="onPrefsGenerated"> The action to call once all PlayerPrefs have been generated. </param>
    private void GenerateCorrectKeys(int walletNum, Action onPrefsGenerated)
    {
        prefCounter = 0;

        prefDictionary.Keys.ForEach(key => SecurePlayerPrefsAsync.SetString((key + "_" + walletNum).Keccak_256(), prefDictionary[key].GetBase64String(), () =>
        {
            if (++prefCounter >= keys.Length)
                onPrefsGenerated?.Invoke();
        }));
    }

    /// <summary>
    /// Generates spoof keys and adds them to the player prefs.
    /// </summary>
    /// <param name="iterations"> The amount of times to iterate through the length of the dictionary and add the spoof keys. </param>
    private void GenerateSpoofKeys(int iterations = 10)
    {
        for (int i = 0; i < iterations * prefDictionary.Keys.Count; i++)
            AsyncTaskScheduler.Schedule(GenerateSpoofKey);
    }

    /// <summary>
    /// Generates a spoof key asynchronously.
    /// </summary>
    /// <returns> The task used for generating the spoof key. </returns>
    private async Task GenerateSpoofKey()
    {
        string key = await Task.Run(() => RandomString.Secure.SHA3.GetString(PASSWORD_LENGTH)).ConfigureAwait(false);
        string value = await Task.Run(() => RandomString.Secure.SHA3.GetString()).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => SecurePlayerPrefsAsync.SetString(key, value));
    }
}