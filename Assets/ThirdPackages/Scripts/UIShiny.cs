using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR

#endif

namespace Coffee.UIExtensions
{
    /// <summary>
    ///     UIEffect.
    /// </summary>
    [AddComponentMenu("UI/UIEffect/UIShiny", 2)]
    public class UIShiny : UIEffectBase
    {
        //################################
        // Constant or Static Members.
        //################################
        public const string shaderName = "UI/Hidden/UI-Effect-Shiny";
        private static readonly ParameterTexture _ptex = new ParameterTexture(8, 128, "_ParamTex");

        //################################
        // Private Members.
        //################################
        private float _lastRotation;

        [Tooltip("Brightness for shiny effect.")] [FormerlySerializedAs("m_Alpha")] [SerializeField] [Range(0, 1)]
        private float m_Brightness = 1f;

        [Header("Advanced Option")] [Tooltip("The area for effect.")] [SerializeField]
        protected EffectArea m_EffectArea;


        //################################
        // Serialize Members.
        //################################
        [Tooltip("Location for shiny effect.")] [FormerlySerializedAs("m_Location")] [SerializeField] [Range(0, 1)]
        private float m_EffectFactor;

        [Tooltip("Gloss factor for shiny effect.")] [FormerlySerializedAs("m_Highlight")] [SerializeField] [Range(0, 1)]
        private float m_Gloss = 1;

        [SerializeField] private EffectPlayer m_Player;

        [Tooltip("Rotation for shiny effect.")] [SerializeField] [Range(-180, 180)]
        private float m_Rotation;

        [Tooltip("Softness for shiny effect.")] [SerializeField] [Range(0.01f, 1)]
        private float m_Softness = 1f;

        [Tooltip("Width for shiny effect.")] [SerializeField] [Range(0, 1)]
        private float m_Width = 0.25f;


        //################################
        // Public Members.
        //################################

        /// <summary>
        ///     Effect factor between 0(start) and 1(end).
        /// </summary>
        [Obsolete("Use effectFactor instead (UnityUpgradable) -> effectFactor")]
        public float location
        {
            get => m_EffectFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_EffectFactor, value))
                {
                    m_EffectFactor = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Effect factor between 0(start) and 1(end).
        /// </summary>
        public float effectFactor
        {
            get => m_EffectFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_EffectFactor, value))
                {
                    m_EffectFactor = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Width for shiny effect.
        /// </summary>
        public float width
        {
            get => m_Width;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Width, value))
                {
                    m_Width = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Softness for shiny effect.
        /// </summary>
        public float softness
        {
            get => m_Softness;
            set
            {
                value = Mathf.Clamp(value, 0.01f, 1);
                if (!Mathf.Approximately(m_Softness, value))
                {
                    m_Softness = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Brightness for shiny effect.
        /// </summary>
        [Obsolete("Use brightness instead (UnityUpgradable) -> brightness")]
        public float alpha
        {
            get => m_Brightness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Brightness, value))
                {
                    m_Brightness = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Brightness for shiny effect.
        /// </summary>
        public float brightness
        {
            get => m_Brightness;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Brightness, value))
                {
                    m_Brightness = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Gloss factor for shiny effect.
        /// </summary>
        [Obsolete("Use gloss instead (UnityUpgradable) -> gloss")]
        public float highlight
        {
            get => m_Gloss;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Gloss, value))
                {
                    m_Gloss = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Gloss factor for shiny effect.
        /// </summary>
        public float gloss
        {
            get => m_Gloss;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Gloss, value))
                {
                    m_Gloss = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Rotation for shiny effect.
        /// </summary>
        public float rotation
        {
            get => m_Rotation;
            set
            {
                if (!Mathf.Approximately(m_Rotation, value))
                {
                    m_Rotation = _lastRotation = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     The area for effect.
        /// </summary>
        public EffectArea effectArea
        {
            get => m_EffectArea;
            set
            {
                if (m_EffectArea != value)
                {
                    m_EffectArea = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Play shinning on enable.
        /// </summary>
        [Obsolete("Use Play/Stop method instead")]
        public bool play
        {
            get => _player.play;
            set => _player.play = value;
        }

        /// <summary>
        ///     Play shinning loop.
        /// </summary>
        [Obsolete]
        public bool loop
        {
            get => _player.loop;
            set => _player.loop = value;
        }

        /// <summary>
        ///     Shinning duration.
        /// </summary>
        public float duration
        {
            get => _player.duration;
            set => _player.duration = Mathf.Max(value, 0.1f);
        }

        /// <summary>
        ///     Delay on loop.
        /// </summary>
        [Obsolete]
        public float loopDelay
        {
            get => _player.loopDelay;
            set => _player.loopDelay = Mathf.Max(value, 0);
        }

        /// <summary>
        ///     Shinning update mode.
        /// </summary>
        public AnimatorUpdateMode updateMode
        {
            get => _player.updateMode;
            set => _player.updateMode = value;
        }

        /// <summary>
        ///     Gets the parameter texture.
        /// </summary>
        public override ParameterTexture ptex => _ptex;

        private EffectPlayer _player => m_Player ?? (m_Player = new EffectPlayer());

        /// <summary>
        ///     This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _player.OnEnable(f => effectFactor = f);
        }

        /// <summary>
        ///     This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            _player.OnDisable();
        }

        /// <summary>
        ///     Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled)
                return;

            var isText = isTMPro || graphic is Text;
            var normalizedIndex = ptex.GetNormalizedIndex(this);

            // rect.
            var rect = m_EffectArea.GetEffectArea(vh, rectTransform.rect);

            // rotation.
            var rad = m_Rotation * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            dir.x *= rect.height / rect.width;
            dir = dir.normalized;

            // Calculate vertex position.
            UIVertex vertex = default;
            Vector2 nomalizedPos;
            var localMatrix = new Matrix2x3(rect, dir.x, dir.y); // Get local matrix.
            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                m_EffectArea.GetNormalizedFactor(i, localMatrix, vertex.position, isText, out nomalizedPos);

                vertex.uv0 = new Vector2(
                    Packer.ToFloat(vertex.uv0.x, vertex.uv0.y),
                    Packer.ToFloat(nomalizedPos.y, normalizedIndex)
                );

                vh.SetUIVertex(vertex, i);
            }
        }

        /// <summary>
        ///     Play effect.
        /// </summary>
        public void Play(bool reset = true)
        {
            _player.Play(reset);
        }

        /// <summary>
        ///     Stop effect.
        /// </summary>
        public void Stop(bool reset = true)
        {
            _player.Stop(reset);
        }

        protected override void SetDirty()
        {
            foreach (var m in materials) ptex.RegisterMaterial(m);
            ptex.SetData(this, 0, m_EffectFactor); // param1.x : location
            ptex.SetData(this, 1, m_Width); // param1.y : width
            ptex.SetData(this, 2, m_Softness); // param1.z : softness
            ptex.SetData(this, 3, m_Brightness); // param1.w : blightness
            ptex.SetData(this, 4, m_Gloss); // param2.x : gloss

            if (!Mathf.Approximately(_lastRotation, m_Rotation) && targetGraphic)
            {
                _lastRotation = m_Rotation;
                SetVerticesDirty();
            }
        }

#pragma warning disable 0414
        [Obsolete] [HideInInspector] [SerializeField]
        private readonly bool m_Play = false;

        [Obsolete] [HideInInspector] [SerializeField]
        private readonly bool m_Loop = false;

        [Obsolete] [HideInInspector] [SerializeField] [Range(0.1f, 10)]
        private readonly float m_Duration = 1;

        [Obsolete] [HideInInspector] [SerializeField] [Range(0, 10)]
        private readonly float m_LoopDelay = 1;

        [Obsolete] [HideInInspector] [SerializeField]
        private readonly AnimatorUpdateMode m_UpdateMode = AnimatorUpdateMode.Normal;
#pragma warning restore 0414


#if UNITY_EDITOR
        protected override Material GetMaterial()
        {
            if (isTMPro) return null;

            return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName));
        }

#pragma warning disable 0612
        protected override void UpgradeIfNeeded()
        {
            // Upgrade for v3.0.0
            if (IsShouldUpgrade(300))
            {
                _player.play = m_Play;
                _player.duration = m_Duration;
                _player.loop = m_Loop;
                _player.loopDelay = m_LoopDelay;
                _player.updateMode = m_UpdateMode;
            }
        }
#pragma warning restore 0612

#endif
    }
}