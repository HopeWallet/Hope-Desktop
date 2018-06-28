using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Class which contains various utils related to randomization.
/// </summary>
public static class RandomUtils
{

    /// <summary>
    /// Generates a BigInteger.
    /// If a collection is passed in, makes sure the new random is not already contained in the collection.
    /// </summary>
    /// <param name="numbersToIgnore"> The collection of numbers to ensure the new random number is not a part of. </param>
    /// <returns> The newly created BigInteger. </returns>
    public static BigInteger GenerateRandomBigInteger(ICollection<BigInteger> numbersToIgnore = null) 
    {
        var rand = new Random();

        if (numbersToIgnore == null)
            return new BigInteger(rand.Next());

        var val = rand.Next();
        while (numbersToIgnore.Contains(val))
            val = rand.Next();

        numbersToIgnore.Add(val);

        return val;
    }

    /// <summary>
    /// Generates a random string based on another string as a seed.
    /// </summary>
    /// <param name="seed"> The string seed to use to generate the random string. </param>
    /// <param name="strLength"> The length of the random string. </param>
    /// <returns> The randomly generated string. </returns>
    public static string GenerateSeededRandomString(string seed, int strLength = 40)
    {
        var random = new Random(seed.GetHashCode());
        var bytes = new byte[strLength];

        random.NextBytes(bytes);

        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)Mathf.Lerp(40, 125, bytes[i] / 255f);

        return Encoding.ASCII.GetString(bytes);
    }

}