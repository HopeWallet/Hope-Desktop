using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Hope.Security.HashGeneration;
using Hope.Random.Bytes;
using Hope.Random.Strings;
using NBitcoin;

/// <summary>
/// Class which derives a password by hashing a password with many different pieces of data saved to the PlayerPrefs.
/// </summary>
public sealed class PlayerPrefPasswordDerivation
{
    private readonly Dictionary<string, byte[]> prefDictionary = new Dictionary<string, byte[]>();

    private readonly string key;

    private int prefCounter;

    private const int PREF_COUNT = 16;
    private const int PASSWORD_LENGTH = 32;

    /// <summary>
    /// Initializes the PlayerPrefPasswordDerivation with the initial key.
    /// </summary>
    public PlayerPrefPasswordDerivation()
    {
        if (!SecurePlayerPrefs.HasKey(PlayerPrefConstants.PLAYER_PREF_PASS_DERIVATION_KEY))
            SecurePlayerPrefs.SetString(PlayerPrefConstants.PLAYER_PREF_PASS_DERIVATION_KEY, RandomString.Secure.Blake2.GetString(64));

        key = SecurePlayerPrefs.GetString(PlayerPrefConstants.PLAYER_PREF_PASS_DERIVATION_KEY);
    }

    /// <summary>
    /// Derives a password to use to encrypt data given the starting password (seed).
    /// </summary>
    /// <param name="startingPassword"> The starting password to use to derive our encryption password. </param>
    /// <returns> The derived encryption password based on the original password. </returns>
    public byte[] Derive(byte[] startingPassword)
    {
        byte[] derivedSeed = RandomBytes.Secure.Blake2.GetBytes(startingPassword, 128).Concat(startingPassword).ToArray().SHA3_512();
        string tempKey = key;

        for (int i = 0; i < PREF_COUNT; i++)
        {
            tempKey = (tempKey + i.ToString()).Keccak_256();

            byte[] last = derivedSeed.SHA3_512();
            byte[] newBytes = RandomBytes.Secure.Blake2.GetBytes(32);

            prefDictionary.AddOrReplace(tempKey, newBytes);
            derivedSeed = last.Concat(newBytes).ToArray().Keccak_512();

            last.ClearBytes();
        }

        derivedSeed = derivedSeed.Concat(startingPassword.Blake2_512()).ToArray();

        return derivedSeed.Blake2_512();
    }

    /// <summary>
    /// Restores a password to use to encrypt data given the starting password (seed).
    /// </summary>
    /// <param name="startingPassword"> The starting password to use to restore our encryption password. </param>
    /// <returns> The restored encryption password based on the original password. </returns>
    public byte[] Restore(byte[] startingPassword)
    {
        byte[] derivedSeed = RandomBytes.Secure.Blake2.GetBytes(startingPassword, 128).Concat(startingPassword).ToArray().SHA3_512();
        string tempKey = key;

        for (int i = 0; i < PREF_COUNT; i++)
        {
            tempKey = (tempKey + i.ToString()).Keccak_256();

            byte[] last = derivedSeed.SHA3_512();
            byte[] newBytes = prefDictionary[tempKey];

            derivedSeed = last.Concat(newBytes).ToArray().Keccak_512();

            last.ClearBytes();
        }

        derivedSeed = derivedSeed.Concat(startingPassword.Blake2_512()).ToArray();

        return derivedSeed.Blake2_512();
    }

    /// <summary>
    /// Sets the player pref values to correspond with the dictionary.
    /// </summary>
    /// <param name="walletAddress"> The wallet address to set the PlayerPrefs up for. </param>
    /// <param name="onPrefsSetup"> Action to call once all the PlayerPrefs were setup. </param>
    /// <param name="generateSpoofKeys"> Whether spoof player pref keys should be created. </param>
    public void SetupPlayerPrefs(string walletAddress, Action onPrefsSetup, bool generateSpoofKeys = true)
    {
        if (prefDictionary == null)
        {
            ExceptionManager.DisplayException(new Exception("Unable to save PlayerPrefs in the PlayerPrefPassword."));
            return;
        }

        if (generateSpoofKeys)
            GenerateSpoofKeys();

        GenerateCorrectKeys(walletAddress = walletAddress.ToLower(), onPrefsSetup);
        prefDictionary.Clear();
    }

    /// <summary>
    /// Takes the player prefs saved in the registry and moves them into a dictionary.
    /// </summary>
    /// <param name="walletAddress"> The wallet address to set the PlayerPrefs up for. </param>
    /// <returns> The dictionary of player prefs. </returns>
    public void PopulatePrefDictionary(string walletAddress)
    {
        if (prefDictionary.Count > 0)
            prefDictionary.Clear();

        walletAddress = walletAddress.ToLower();
        string tempKey = key;

        for (int i = 0; i < PREF_COUNT; i++)
        {
            tempKey = (tempKey + i.ToString()).Keccak_256();
            prefDictionary.Add(tempKey, SecurePlayerPrefs.GetString((tempKey + "_" + walletAddress).Keccak_256()).GetBase64Bytes());
        }
    }

    /// <summary>
    /// Generates the correct PlayerPref keys for the given wallet number.
    /// </summary>
    /// <param name="walletAddress"> The wallet address to set the PlayerPrefs up for. </param>
    /// <param name="onPrefsGenerated"> The action to call once all PlayerPrefs have been generated. </param>
    private void GenerateCorrectKeys(string walletAddress, Action onPrefsGenerated)
    {
        prefCounter = 0;

        prefDictionary.Keys.ForEach(key => SecurePlayerPrefsAsync.SetString((key + "_" + walletAddress).Keccak_256(), prefDictionary[key].GetBase64String(), () =>
        {
            if (++prefCounter >= PREF_COUNT)
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