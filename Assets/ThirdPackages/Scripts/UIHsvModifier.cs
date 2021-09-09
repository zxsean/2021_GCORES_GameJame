using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    /// <summary>
    ///     HSV Modifier.
    /// </summary>
    [AddComponentMenu("UI/UIEffect/UIHsvModifier", 4)]
    public class UIHsvModifier : UIEffectBase
    {
        //################################
        // Constant or Static Members.
        //################################
        public const string shaderName = "UI/Hidden/UI-Effect-HSV";
        private static readonly ParameterTexture _ptex = new ParameterTexture(7, 128, "_ParamTex");

        [Header("Adjustment")] [Tooltip("Hue shift [-0.5 ~ 0.5].")] [SerializeField] [Range(-0.5f, 0.5f)]
        private float m_Hue;

        [Tooltip("Color range to affect hsv shift [0 ~ 1].")] [SerializeField] [Range(0, 1)]
        private float m_Range = 0.1f;

        [Tooltip("Saturation shift [-0.5 ~ 0.5].")] [SerializeField] [Range(-0.5f, 0.5f)]
        private float m_Saturation;


        //################################
        // Serialize Members.
        //################################
        [Header("Target")] [Tooltip("Target color to affect hsv shift.")] [SerializeField] [ColorUsage(false)]
        private Color m_TargetColor = Color.red;

        [Tooltip("Value shift [-0.5 ~ 0.5].")] [SerializeField] [Range(-0.5f, 0.5f)]
        private float m_Value;


        //################################
        // Public Members.
        //################################

        /// <summary>
        ///     Target color to affect hsv shift.
        /// </summary>
        public Color targetColor
        {
            get => m_TargetColor;
            set
            {
                if (m_TargetColor != value)
                {
                    m_TargetColor = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Color range to affect hsv shift [0 ~ 1].
        /// </summary>
        public float range
        {
            get => m_Range;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (!Mathf.Approximately(m_Range, value))
                {
                    m_Range = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Saturation shift [-0.5 ~ 0.5].
        /// </summary>
        public float saturation
        {
            get => m_Saturation;
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (!Mathf.Approximately(m_Saturation, value))
                {
                    m_Saturation = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Value shift [-0.5 ~ 0.5].
        /// </summary>
        public float value
        {
            get => m_Value;
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (!Mathf.Approximately(m_Value, value))
                {
                    m_Value = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Hue shift [-0.5 ~ 0.5].
        /// </summary>
        public float hue
        {
            get => m_Hue;
            set
            {
                value = Mathf.Clamp(value, -0.5f, 0.5f);
                if (!Mathf.Approximately(m_Hue, value))
                {
                    m_Hue = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        ///     Gets the parameter texture.
        /// </summary>
        public override ParameterTexture ptex => _ptex;

#if UNITY_EDITOR
        protected override Material GetMaterial()
        {
            if (isTMPro) return null;
            return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName));
        }
#endif

        /// <summary>
        ///     Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled)
                return;

            var normalizedIndex = ptex.GetNormalizedIndex(this);
            UIVertex vertex = default;
            var count = vh.currentVertCount;
            for (var i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);

                vertex.uv0 = new Vector2(
                    Packer.ToFloat(vertex.uv0.x, vertex.uv0.y),
                    normalizedIndex
                );
                vh.SetUIVertex(vertex, i);
            }
        }

        protected override void SetDirty()
        {
            float h, s, v;
            Color.RGBToHSV(m_TargetColor, out h, out s, out v);

            foreach (var m in materials) ptex.RegisterMaterial(m);

            ptex.SetData(this, 0, h); // param1.x : target hue
            ptex.SetData(this, 1, s); // param1.y : target saturation
            ptex.SetData(this, 2, v); // param1.z : target value
            ptex.SetData(this, 3, m_Range); // param1.w : target range
            ptex.SetData(this, 4, m_Hue + 0.5f); // param2.x : hue shift
            ptex.SetData(this, 5, m_Saturation + 0.5f); // param2.y : saturation shift
            ptex.SetData(this, 6, m_Value + 0.5f); // param2.z : value shift
        }

        //################################
        // Private Members.
        //################################
    }
}