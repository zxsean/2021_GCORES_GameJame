using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions
{
    public interface IParameterTexture
    {
        int parameterIndex { get; set; }

        ParameterTexture ptex { get; }
    }

    /// <summary>
    ///     Parameter texture.
    /// </summary>
    [Serializable]
    public class ParameterTexture
    {
        private static List<Action> updates;
        private readonly int _channels;
        private readonly byte[] _data;
        private readonly int _instanceLimit;
        private readonly string _propertyName;
        private readonly Stack<int> _stack;
        private bool _needUpload;
        private int _propertyId;


        //################################
        // Private Members.
        //################################

        private Texture2D _texture;

        //################################
        // Public Members.
        //################################

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coffee.UIExtensions.ParameterTexture" /> class.
        /// </summary>
        /// <param name="channels">Channels.</param>
        /// <param name="instanceLimit">Instance limit.</param>
        /// <param name="propertyName">Property name.</param>
        public ParameterTexture(int channels, int instanceLimit, string propertyName)
        {
            _propertyName = propertyName;
            _channels = ((channels - 1) / 4 + 1) * 4;
            _instanceLimit = ((instanceLimit - 1) / 2 + 1) * 2;
            _data = new byte[_channels * _instanceLimit];

            _stack = new Stack<int>(_instanceLimit);
            for (var i = 1; i < _instanceLimit + 1; i++) _stack.Push(i);
        }


        /// <summary>
        ///     Register the specified target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void Register(IParameterTexture target)
        {
            Initialize();
            if (target.parameterIndex <= 0 && 0 < _stack.Count)
                target.parameterIndex = _stack.Pop();
//				Debug.LogFormat("<color=green>@@@ Register {0} : {1}</color>", target, target.parameterIndex);
        }

        /// <summary>
        ///     Unregister the specified target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void Unregister(IParameterTexture target)
        {
            if (0 < target.parameterIndex)
            {
//				Debug.LogFormat("<color=red>@@@ Unregister {0} : {1}</color>", target, target.parameterIndex);
                _stack.Push(target.parameterIndex);
                target.parameterIndex = 0;
            }
        }

        /// <summary>
        ///     Sets the data.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="channelId">Channel identifier.</param>
        /// <param name="value">Value.</param>
        public void SetData(IParameterTexture target, int channelId, byte value)
        {
            var index = (target.parameterIndex - 1) * _channels + channelId;
            if (0 < target.parameterIndex && _data[index] != value)
            {
                _data[index] = value;
                _needUpload = true;
            }
        }

        /// <summary>
        ///     Sets the data.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="channelId">Channel identifier.</param>
        /// <param name="value">Value.</param>
        public void SetData(IParameterTexture target, int channelId, float value)
        {
            SetData(target, channelId, (byte) (Mathf.Clamp01(value) * 255));
        }

        /// <summary>
        ///     Registers the material.
        /// </summary>
        /// <param name="mat">Mat.</param>
        public void RegisterMaterial(Material mat)
        {
            if (_propertyId == 0) _propertyId = Shader.PropertyToID(_propertyName);
            if (mat) mat.SetTexture(_propertyId, _texture);
        }

        /// <summary>
        ///     Gets the index of the normalized.
        /// </summary>
        /// <returns>The normalized index.</returns>
        /// <param name="target">Target.</param>
        public float GetNormalizedIndex(IParameterTexture target)
        {
            return (target.parameterIndex - 0.5f) / _instanceLimit;
        }

        /// <summary>
        ///     Initialize this instance.
        /// </summary>
        private void Initialize()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            if (updates == null)
            {
                updates = new List<Action>();
                Canvas.willRenderCanvases += () =>
                {
                    var count = updates.Count;
                    for (var i = 0; i < count; i++) updates[i].Invoke();
                };
            }

            if (!_texture)
            {
                var isLinear = QualitySettings.activeColorSpace == ColorSpace.Linear;
                _texture = new Texture2D(_channels / 4, _instanceLimit, TextureFormat.RGBA32, false, isLinear);
                _texture.filterMode = FilterMode.Point;
                _texture.wrapMode = TextureWrapMode.Clamp;

                updates.Add(UpdateParameterTexture);
                _needUpload = true;
            }
        }

        private void UpdateParameterTexture()
        {
            if (_needUpload && _texture)
            {
                _needUpload = false;
                _texture.LoadRawTextureData(_data);
                _texture.Apply(false, false);
            }
        }
    }
}