using System;
using System.Collections.Generic;
using System.Numerics;

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

}