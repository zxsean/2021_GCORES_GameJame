using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    public class MaterialCache
    {
        public static List<MaterialCache> materialCaches = new List<MaterialCache>();
        public ulong hash { get; private set; }

        public int referenceCount { get; private set; }

        public Texture texture { get; private set; }

        public Material material { get; private set; }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void ClearCache()
        {
            foreach (var cache in materialCaches) cache.material = null;
            materialCaches.Clear();
        }
#endif

        public static MaterialCache Register(ulong hash, Texture texture, Func<Material> onCreateMaterial)
        {
            var cache = materialCaches.FirstOrDefault(x => x.hash == hash);
            if (cache != null && cache.material)
            {
                if (cache.material)
                {
                    cache.referenceCount++;
                }
                else
                {
                    materialCaches.Remove(cache);
                    cache = null;
                }
            }

            if (cache == null)
            {
                cache = new MaterialCache
                {
                    hash = hash,
                    material = onCreateMaterial(),
                    referenceCount = 1
                };
                materialCaches.Add(cache);
            }

            return cache;
        }

        public static MaterialCache Register(ulong hash, Func<Material> onCreateMaterial)
        {
            var cache = materialCaches.FirstOrDefault(x => x.hash == hash);
            if (cache != null) cache.referenceCount++;
            if (cache == null)
            {
                cache = new MaterialCache
                {
                    hash = hash,
                    material = onCreateMaterial(),
                    referenceCount = 1
                };
                materialCaches.Add(cache);
            }

            return cache;
        }

        public static void Unregister(MaterialCache cache)
        {
            if (cache == null) return;

            cache.referenceCount--;
            if (cache.referenceCount <= 0)
            {
                materialCaches.Remove(cache);
                cache.material = null;
            }
        }
    }
}