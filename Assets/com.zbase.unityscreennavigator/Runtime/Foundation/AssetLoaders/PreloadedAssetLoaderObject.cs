﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZBase.UnityScreenNavigator.Foundation.AssetLoaders
{
    [CreateAssetMenu(fileName = "PreloadedAssetLoader", menuName = "Screen Navigator/Loaders/Preloaded Asset Loader")]
    public sealed class PreloadedAssetLoaderObject : AssetLoaderObject, IAssetLoader, IInitializable, IDeinitializable
    {
        [SerializeField] private List<KeyAssetPair> _preloadedObjects = new();

        private readonly PreloadedAssetLoader _loader = new();

        public List<KeyAssetPair> PreloadedObjects => _preloadedObjects;

        public void Initialize()
        {
            var src = _preloadedObjects;
            var dest = _loader.preloadedObjects;
            dest.Clear();

            var count = src.Count;

            for (var i = 0; i < count; i++)
            {
                var preloadedObject = src[i];
                var key = preloadedObject.Key;

                if (string.IsNullOrEmpty(key))
                {
                    ErrorIfKeyIsNull(i, this);
                    continue;
                }

                var asset = preloadedObject.Asset;

                if (asset == false)
                {
                    ErrorIfAssetIsNull(i, this);
                    continue;
                }

                if (dest.TryAdd(key, asset) == false)
                {
                    ErrorIfDuplicate(i, key, this);
                }
            }
        }

        public void Deinitialize()
        {
            _loader.preloadedObjects.Clear();
        }

        public override AssetLoadHandle<T> Load<T>(string key)
        {
            return _loader.Load<T>(key);
        }

        public override AssetLoadHandle<T> LoadAsync<T>(string key)
        {
            return _loader.LoadAsync<T>(key);
        }

        public override void Release(AssetLoadHandleId handleId)
        {
            _loader.Release(handleId);
        }

        [HideInCallstack, DoesNotReturn, Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private static void ErrorIfKeyIsNull(int index, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError($"Key at {index} is null or empty", context);
        }

        [HideInCallstack, DoesNotReturn, Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private static void ErrorIfAssetIsNull(int index, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError($"Asset at {index} is null", context);
        }
        
        [HideInCallstack, DoesNotReturn, Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private static void ErrorIfDuplicate(int index, string key, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError($"Asset at index {index} cannot be registered because the key `{key}` is already existing", context);
        }

        [Serializable]
        public struct KeyAssetPair
        {
            [SerializeField] private KeySourceType _keySource;
            [SerializeField] private string _key;
            [SerializeField] private Object _asset;

            public KeySourceType KeySource
            {
                readonly get => _keySource;
                set => _keySource = value;
            }

            public string Key
            {
                readonly get => GetKey();
                set => _key = value;
            }

            public Object Asset
            {
                readonly get => _asset;
                set => _asset = value;
            }

            private readonly string GetKey()
            {
                if (_keySource == KeySourceType.AssetName)
                    return _asset == false ? "" : _asset.name;

                return _key;
            }
        }
    }
}