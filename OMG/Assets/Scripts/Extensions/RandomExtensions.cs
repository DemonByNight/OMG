using System.Collections.Generic;

namespace OMG
{
    public static class RandomExtensions
    {
        public static T Random<T>(this List<T> list)
            => list[UnityEngine.Random.Range(0, list.Count)];
    }
}
