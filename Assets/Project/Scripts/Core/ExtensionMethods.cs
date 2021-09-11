using System;

namespace Project.Scripts.Core
{
    public static class ListExtensions
    {
        public static T Random<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array is empty/null. How the hell should I get a random item from that?");

            return array[UnityEngine.Random.Range(0, array.Length)];
        }
    }
}