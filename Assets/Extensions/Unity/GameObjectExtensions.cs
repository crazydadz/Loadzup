using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Silphid.Extensions
{
    public static class GameObjectExtensions
    {
        public static IEnumerable<GameObject> WithComponent<T>(this IEnumerable<GameObject> gameObjects)
        {
            return gameObjects.Where(g => g.GetComponent<T>() != null);
        }

        public static IEnumerable<T> OfComponentEx<T>(this IEnumerable<GameObject> gameObjects)
        {
            return gameObjects.SelectMany(g => g.GetComponents<T>());
        }
    }
}