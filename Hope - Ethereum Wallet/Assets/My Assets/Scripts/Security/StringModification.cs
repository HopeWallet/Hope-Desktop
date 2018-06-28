using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class used for modifying strings based on given rules.
/// </summary>
public static class StringModification
{

	public const string CHAR_LOOKUP = "abcdefghijklmnopqrstuvwxyzZYXWVUTSRQPONMLKJIHGFEDCBA1029384756";

	/// <summary>
	/// Modifies a given string based on a modifier string, modification number, and character lookup table.
	/// </summary>
	/// <param name="original"> Original string to modify. </param>
	/// <param name="modifier"> The modifier string to multiply/divide/shift left/shift right. </param>
	/// <param name="modNum"> Modification number which determines which operation to perform and the seed for the random shuffle. </param>
	/// <param name="charLookup"> The character lookup table to derive modified string characters from. </param>
	/// <returns> The modified string. </returns>
	public static string Modify(this string original, string modifier, int modNum, string charLookup = CHAR_LOOKUP)
	{
		switch(modNum)
		{
			case 1: case 2: case 3: case 4:
				original = original.Multiply(modifier, charLookup);
				break;
			case 5: case 6: case 7: case 8: 
				original =  original.Divide(modifier, charLookup);
				break;
			case 9: case 10: case 11: case 12: 
				original =  original.ShiftLeft(modifier, charLookup);
				break;
			case 13: case 14: case 15: case 16: 
				original =  original.ShiftRight(modifier, charLookup);
				break;
			default:
				return original;
		}
		return original.Shuffle(modNum * modifier[modNum - 1]);
	}

    /// <summary>
    /// Multiplies two strings by each other and returns the result.
    /// </summary>
    /// <param name="original"> The string to multiply. </param>
    /// <param name="multiplier"> The multiplier string. </param>
    /// <param name="charLookup"> The character lookup table to use to retrieve the characters after the multiplication. </param>
    /// <returns> The multiplied string. </returns>
    public static string Multiply(this string original, string multiplier, string charLookup = CHAR_LOOKUP) 
        => original.PerformOperation(multiplier, charLookup, (str1, str2, i) => (double)str1[i] * str2[i]);

    /// <summary>
    /// Divides one string by another and returns the result.
    /// </summary>
    /// <param name="original"> The string to divide. </param>
    /// <param name="divisor"> The divisor string. </param>
    /// <param name="charLookup"> The character lookup table to use to retrieve the characters after the division. </param>
    /// <returns> The first string divided by the second. </returns>
    public static string Divide(this string original, string divisor, string charLookup = CHAR_LOOKUP) 
        => original.PerformOperation(divisor, charLookup, (str1, str2, i) => (double)str1[i] / str2[i]);

    /// <summary>
    /// Bit shifts one string left by the values of the second.
    /// </summary>
    /// <param name="original"> The string to shift. </param>
    /// <param name="modifier"> The string to use as our bit shifter. </param>
    /// <param name="charLookup"> The character lookup table to use to retrieve our resultant characters. </param>
    /// <returns> The first string bit shifted left by the second. </returns>
    public static string ShiftLeft(this string original, string modifier, string charLookup = CHAR_LOOKUP) 
        => original.PerformOperation(modifier, charLookup, (str1, str2, i) => str1[i] << str2[i]);

    /// <summary>
    /// Bit shifts one string right by the values of the second.
    /// </summary>
    /// <param name="original"> The string to shift. </param>
    /// <param name="modifier"> The string to use as our bit shifter. </param>
    /// <param name="charLookup"> The character lookup table to use to retrieve our resultant characters. </param>
    /// <returns> The first string bit shifted right by the second. </returns>
    public static string ShiftRight(this string original, string modifier, string charLookup = CHAR_LOOKUP) 
        => original.PerformOperation(modifier, charLookup, (str1, str2, i) => str1[i] >> str2[i]);

    /// <summary>
    /// Combines two strings together and randomizes their values.
    /// </summary>
    /// <param name="str1"> The first string to combine. </param>
    /// <param name="str2"> The second string to combine. </param>
    /// <returns></returns>
    public static string CombineAndRandomize(this string str1, string str2)
	{
		int total = 0;

		str2.ToList().ForEach(c => total += c);

		return (str1 + str2).Shuffle(total);
	}

	/// <summary>
	/// Performs an operation on the first string given the second modifier string.
	/// </summary>
	/// <param name="original"> The string to modify. </param>
	/// <param name="modifier"> The modifier string. </param>
	/// <param name="charLookup"> The string of characters to use to lookup new characters after the string operation. </param>
	/// <param name="runOperation"></param>
	/// <returns> The modified string. </returns>
	private static string PerformOperation(this string original, string modifier, string charLookup, Func<string, string, int, double> runOperation)
	{
		var result = "";
		var length = GetSafeLength(original, modifier);

		for (int i = 0; i < length; i++)
			result += charLookup[Mathf.Abs((int)(((runOperation(original, modifier, i) * 100) % charLookup.Length)))];

		return result;
	}

	/// <summary>
	/// Shuffles the characters of a string given a key.
	/// </summary>
	/// <param name="toShuffle"> The string to shuffle. </param>
	/// <param name="key"> The key, or seed, to determine how we are shuffling the string. </param>
	/// <returns> The shuffled string. </returns>
	private static string Shuffle(this string toShuffle, int key)
	{
		var size = toShuffle.Length;
		var chars = toShuffle.ToArray();
		var exchanges = GetShuffleExchanges(size, key);

		for (int i = size - 1; i > 0; i--)
		{
			var num = exchanges[size - 1 - i];
			var temp = chars[i];

			chars[i] = chars[num];
			chars[num] = temp;
		}

		return new string(chars);
	}

	private static int[] GetShuffleExchanges(int size, int key)
	{
		var exchanges = new int[size - 1];
		var rand = new System.Random(key);

		for (int i = size - 1; i > 0; i--)
		{
			var num = rand.Next(i + 1);
			exchanges[size - 1 - i] = num;
		}

		return exchanges;
	}

    /// <summary>
    /// Gets the length of what the resultant string would be when performing an operation.
    /// The string with the lesser length gets priority.
    /// </summary>
    /// <param name="str1"> The first string to check. </param>
    /// <param name="str2"> The second string to check. </param>
    /// <returns> The length to use for the new resultant string. </returns>
    private static int GetSafeLength(string str1, string str2) => str1.Length < str2.Length ? str1.Length : str2.Length;
}
