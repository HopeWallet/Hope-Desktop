using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;
using System;
using System.Threading.Tasks;
using Zenject;

/// <summary>
/// Class which manages the base password data for the AES encryption of the wallet.
/// </summary>
[Serializable]
public class PlayerPrefPassword : ScriptableObject
{

    [SerializeField] public int[] ops = new int[16];
    [SerializeField] public string[] keys = new string[17];
    [SerializeField] private string charLookup;

    private Dictionary<string, string> prefDictionary;

    private string[] extraCharLookups;

    private int prefCounter;

    private const int PASSWORD_LENGTH = 16;

    /// <summary>
    /// Extracts the encryption password from the SecurePlayerPrefs.
    /// </summary>
    /// <returns> The encryption password to access the wallet data. </returns>
    public string ExtractEncryptionPassword(string seed)
    {
        string pass =  DeriveEncryptionPassword(prefDictionary[keys[keys.Length - 1]], prefDictionary[keys[0]],
                                                i => prefDictionary[keys[i]]).CombineAndRandomize(seed);

        prefDictionary.Clear();

        return pass;
    }

    /// <summary>
    /// Generates an encryption password to use to encrypt WalletData.
    /// </summary>
    /// <returns> The encryption password to use to encrypt the WalletData object. </returns>
    public string GenerateEncryptionPassword(string seed)
    {
        string operationStringDeterminant = PasswordUtils.GenerateFixedLengthPassword(PASSWORD_LENGTH);
        string password = PasswordUtils.GenerateFixedLengthPassword(PASSWORD_LENGTH);

        prefDictionary = new Dictionary<string, string> { { keys[0], password }, { keys[keys.Length - 1], operationStringDeterminant } };

        return DeriveEncryptionPassword(operationStringDeterminant, password,
                                        i => PasswordUtils.GenerateFixedLengthPassword(PASSWORD_LENGTH),
                                        (i, pass) => prefDictionary.Add(keys[i], pass)).CombineAndRandomize(seed);
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
    /// Adds a certain number of character lookup tables to use to derive passwords.
    /// </summary>
    /// <param name="extraCharLookups"> The array of character lookups to add. </param>
    public void AddCharLookups(string[] extraCharLookups) => this.extraCharLookups = extraCharLookups;

    /// <summary>
    /// Takes the player prefs saved in the registry and moves them into a dictionary.
    /// </summary>
    /// <param name="walletNum"> The wallet number to get the PlayerPrefs for. </param>
    /// <returns> The dictionary of player prefs. </returns>
    public void PopulatePrefDictionary(int walletNum)
    {
        prefDictionary = new Dictionary<string, string>();

        keys.SafeForEach(key => prefDictionary.Add(key, SecurePlayerPrefs.GetString(key + "_" + walletNum)));
    }

    /// <summary>
    /// Generates the correct PlayerPref keys for the given wallet number.
    /// </summary>
    /// <param name="walletNum"> The wallet number to generate the PlayerPrefs for. </param>
    /// <param name="onPrefsGenerated"> The action to call once all PlayerPrefs have been generated. </param>
    private void GenerateCorrectKeys(int walletNum, Action onPrefsGenerated)
    {
        prefCounter = 0;

        prefDictionary.Keys.ForEach(key => SecurePlayerPrefsAsync.SetString(key + "_" + walletNum, prefDictionary[key], () =>
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
        string key = await Task.Run(() => PasswordUtils.GenerateFixedLengthPassword(PASSWORD_LENGTH)).ConfigureAwait(false);
        string value = await Task.Run(() => PasswordUtils.GenerateRandomPassword()).ConfigureAwait(false) + await Task.Run(() => RandomUtils.GenerateRandomHexLetter()).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => SecurePlayerPrefsAsync.SetString(key, value));
    }

    /// <summary>
    /// Derives the encryption password given many parameters.
    /// </summary>
    /// <param name="operationStringDeterminant"> The string used to determine the string which contains all operation numbers. </param>
    /// <param name="password"> The starting password to use to create the end result password. </param>
    /// <param name="getPass"> Func which gets the next password to use for derivation. </param>
    /// <param name="usePass"> Action which allows the newest password to be used. </param>
    /// <returns> The derived password from the input parameters. </returns>
    private string DeriveEncryptionPassword(string operationStringDeterminant, string password, Func<int, string> getPass, Action<int, string> usePass = null)
    {
        var operationNumberString = operationStringDeterminant;
        var operationNumbers = StringToIntList(operationNumberString);
        var operationNumbersIndex = Mathf.Clamp(GetIndexFromModulus(operationNumbers), 1, 15);
        var indexToPassDictionary = new Dictionary<int, string>();

        DetermineOperationNumbers(getPass, operationNumberString, ref operationNumbers, operationNumbersIndex, indexToPassDictionary);
        return ExecuteModification(password, getPass, usePass, operationNumbers, indexToPassDictionary);
    }

    /// <summary>
    /// Determines the operation numbers that need to be applied to modify the 
    /// </summary>
    /// <param name="getPass"> Func called each iteration to get the password at the given index. </param>
    /// <param name="operationNumberString"> The string containing the starting operation numbers. </param>
    /// <param name="operationNumbers"> The list of operation numbers to use for the password modification. </param>
    /// <param name="operationNumbersIndex"> The index to use to get the operation numbers. </param>
    /// <param name="indexToPassDictionary"> The dictionary of indices to their respective password string. </param>
    private void DetermineOperationNumbers(Func<int, string> getPass, string operationNumberString, ref List<int> operationNumbers, int operationNumbersIndex, Dictionary<int, string> indexToPassDictionary)
    {
        for (int i = 0; i < operationNumbersIndex; i++)
        {
            operationNumberString = indexToPassDictionary.ContainsKey(operationNumbersIndex) ? indexToPassDictionary[operationNumbersIndex] : getPass(operationNumbersIndex);
            indexToPassDictionary[operationNumbersIndex] = operationNumberString;
            operationNumbers = StringToIntList(operationNumberString);
            operationNumbersIndex = Mathf.Clamp(GetIndexFromModulus(operationNumbers), 1, 15);
        }
    }

    /// <summary>
    /// Iteratively modifies the password based on the dictionary of strings.
    /// </summary>
    /// <param name="password"> The password to modify. </param>
    /// <param name="getPass"> Func called each iteration to get the password at that index. </param>
    /// <param name="usePass"> Action to call on each iteration passing the index and password. </param>
    /// <param name="operationNumbers"> The list of operation values to apply to the password at each index. </param>
    /// <param name="indexToPassDictionary"> The dictionary of passwords. </param>
    /// <returns> The modified password. </returns>
    private string ExecuteModification(string password, Func<int, string> getPass, Action<int, string> usePass,
        List<int> operationNumbers, Dictionary<int, string> indexToPassDictionary)
    {
        for (int i = 1; i < keys.Length - 1; i++)
        {
            var newPass = indexToPassDictionary.ContainsKey(i) ? indexToPassDictionary[i] : getPass(i);
            password = ModifyString(password, newPass, operationNumbers, i);
            usePass?.Invoke(i, newPass);
        }

        return password;
    }

    /// <summary>
    /// Modifies a string given another string modifier.
    /// Modifies it based on the default character lookup table, and the extra lookup tables.
    /// </summary>
    /// <param name="password"> The current derived password. </param>
    /// <param name="modifier"> The modifier to apply to the password. </param>
    /// <param name="operationNumbers"> The set of numbers in hexadecimal range. </param>
    /// <param name="index"> The current loop index. </param>
    /// <returns> Returns the modified password. </returns>
    private string ModifyString(string password, string modifier, IEnumerable<int> operationNumbers, int index)
    {
        int operationNum = Mathf.Clamp(operationNumbers.ElementAt(index) - 9, 0, 1); // Numbers in the 10-15 range use the default and this class's charLookups.
        int oppositeOperation = Convert.ToInt32(!Convert.ToBoolean(operationNum)); // Numbers in the 0-9 range use the extra charLookups.

        extraCharLookups.SafeForEach(str => password = password.Modify(modifier, oppositeOperation * ops[index], str));

        return password.Modify(modifier, operationNum * ops[index]).Modify(modifier, operationNum, charLookup);
    }

    /// <summary>
    /// Sums up all the numbers in a list, and takes the modulus of it and the length to produce a pseudo random index.
    /// </summary>
    /// <param name="numbers"> The array of numbers to get the random index from. </param>
    /// <returns> The index which exists in the range of numbers. </returns>
    private int GetIndexFromModulus(List<int> numbers)
    {
        int total = 0;
        numbers.ForEach(i => total += i);
        return total % numbers.Count;
    }

    /// <summary>
    /// Converts a string into a set of ints.
    /// Each index of the string contains a hexadecimal number, which will be converted into an int and stored in each array index.
    /// </summary>
    /// <param name="str"> The string of hexadecimal numbers. </param>
    /// <returns> The set of ints containing all hexadecimals in the string. </returns>
    private List<int> StringToIntList(string str) => str.Select(c => int.Parse(c.ToString().ToUpper(), NumberStyles.HexNumber)).ToList();

}