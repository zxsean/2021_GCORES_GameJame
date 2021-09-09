using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    /// <summary>
    ///     UIGradient.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/MeshEffectForTextMeshPro/UIGradient", 101)]
    public class UIGradient : BaseMeshEffect
    {
        //################################
        // Constant or Static Members.
        //################################

        /// <summary>
        ///     Gradient direction.
        /// </summary>
        public enum Direction
        {
            Horizontal,
            Vertical,
            Angle,
            Diagonal
        }

        /// <summary>
        ///     Gradient space for Text.
        /// </summary>
        public enum GradientStyle
        {
            Rect,
            Fit,
            Split
        }


        //################################
        // Private Members.
        //################################
        private static readonly Vector2[] s_SplitedCharacterPosition =
            {Vector2.up, Vector2.one, Vector2.right, Vector2.zero};

        [Tooltip("Color1: Top or Left.")] [SerializeField]
        private Color m_Color1 = Color.white;

        [Tooltip("Color2: Bottom or Right.")] [SerializeField]
        private Color m_Color2 = Color.white;

        [Tooltip("Color3: For diagonal.")] [SerializeField]
        private Color m_Color3 = Color.white;

        [Tooltip("Color4: For diagonal.")] [SerializeField]
        private Color m_Color4 = Color.white;

        [Tooltip("Color space to correct color.")] [SerializeField]
        private ColorSpace m_ColorSpace = ColorSpace.Uninitialized;


        //################################
        // Serialize Members.
        //################################

        [Tooltip("Gradient Direction.")] [SerializeField]
        private Direction m_Direction;

        [Tooltip("Gradient style for Text.")] [SerializeField]
        private GradientStyle m_GradientStyle;

        [Tooltip("Ignore aspect ratio.")] [SerializeField]
        private bool m_IgnoreAspectRatio = true;

        [Tooltip("Gradient offset for Horizontal, Vertical or Angle.")] [SerializeField] [Range(-1, 1)]
        private float m_Offset1;

        [Tooltip("Gradient offset for Diagonal.")] [SerializeField] [Range(-1, 1)]
        private float m_Offset2;

        [Tooltip("Gradient rotation.")] [SerializeField] [Range(-180, 180)]
        private float m_Rotation;


        //################################
        // Public Members.
        //################################
        public Graphic targetGraphic => graphic;

        /// <summary>
        ///     Gradient Direction.
        /// </summary>
        public Direction direction
        {
            get => m_Direction;
            set
            {
                if (m_Direction != value)
                {
                    m_Direction = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Color1: Top or Left.
        /// </summary>
        public Color color1
        {
            get => m_Color1;
            set
            {
                if (m_Color1 != value)
                {
                    m_Color1 = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Color2: Bottom or Right.
        /// </summary>
        public Color color2
        {
            get => m_Color2;
            set
            {
                if (m_Color2 != value)
                {
                    m_Color2 = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Color3: For diagonal.
        /// </summary>
        public Color color3
        {
            get => m_Color3;
            set
            {
                if (m_Color3 != value)
                {
                    m_Color3 = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Color4: For diagonal.
        /// </summary>
        public Color color4
        {
            get => m_Color4;
            set
            {
                if (m_Color4 != value)
                {
                    m_Color4 = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Gradient rotation.
        /// </summary>
        public float rotation
        {
            get =>
                m_Direction == Direction.Horizontal ? -90
                : m_Direction == Direction.Vertical ? 0
                : m_Rotation;
            set
            {
                if (!Mathf.Approximately(m_Rotation, value))
                {
                    m_Rotation = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Gradient offset for Horizontal, Vertical or Angle.
        /// </summary>
        public float offset
        {
            get => m_Offset1;
            set
            {
                if (m_Offset1 != value)
                {
                    m_Offset1 = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Gradient offset for Diagonal.
        /// </summary>
        public Vector2 offset2
        {
            get => new Vector2(m_Offset2, m_Offset1);
            set
            {
                if (m_Offset1 != value.y || m_Offset2 != value.x)
                {
                    m_Offset1 = value.y;
                    m_Offset2 = value.x;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Gradient style for Text.
        /// </summary>
        public GradientStyle gradientStyle
        {
            get => m_GradientStyle;
            set
            {
                if (m_GradientStyle != value)
                {
                    m_GradientStyle = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Color space to correct color.
        /// </summary>
        public ColorSpace colorSpace
        {
            get => m_ColorSpace;
            set
            {
                if (m_ColorSpace != value)
                {
                    m_ColorSpace = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        ///     Ignore aspect ratio.
        /// </summary>
        public bool ignoreAspectRatio
        {
            get => m_IgnoreAspectRatio;
            set
            {
                if (m_IgnoreAspectRatio != value)
                {
                    m_IgnoreAspectRatio = value;
                    SetVerticesDirty();
                }
            }
        }


        /// <summary>
        ///     Call used to modify mesh.
        /// </summary>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            // Gradient space.
            Rect rect = default;
            var vertex = default(UIVertex);
            if (m_GradientStyle == GradientStyle.Rect)
            {
                // RectTransform.
                rect = graphic.rectTransform.rect;
            }
            else if (m_GradientStyle == GradientStyle.Split)
            {
                // Each characters.
                rect.Set(0, 0, 1, 1);
            }
            else if (m_GradientStyle == GradientStyle.Fit)
            {
                // Fit to contents.
                rect.xMin = rect.yMin = float.MaxValue;
                rect.xMax = rect.yMax = float.MinValue;
                for (var i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    rect.xMin = Mathf.Min(rect.xMin, vertex.position.x);
                    rect.yMin = Mathf.Min(rect.yMin, vertex.position.y);
                    rect.xMax = Mathf.Max(rect.xMax, vertex.position.x);
                    rect.yMax = Mathf.Max(rect.yMax, vertex.position.y);
                }
            }

            // Gradient rotation.
            var rad = rotation * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            if (!m_IgnoreAspectRatio && Direction.Angle <= m_Direction)
            {
                dir.x *= rect.height / rect.width;
                dir = dir.normalized;
            }

            // Calculate vertex color.
            Color color;
            Vector2 nomalizedPos;
            var localMatrix = new Matrix2x3(rect, dir.x, dir.y); // Get local matrix.
            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);

                // Normalize vertex position by local matrix.
                if (m_GradientStyle == GradientStyle.Split)
                    // Each characters.
                    nomalizedPos = localMatrix * s_SplitedCharacterPosition[i % 4] + offset2;
                else
                    nomalizedPos = localMatrix * vertex.position + offset2;

                // Interpolate vertex color.
                if (direction == Direction.Diagonal)
                    color = Color.LerpUnclamped(
                        Color.LerpUnclamped(m_Color1, m_Color2, nomalizedPos.x),
                        Color.LerpUnclamped(m_Color3, m_Color4, nomalizedPos.x),
                        nomalizedPos.y);
                else
                    color = Color.LerpUnclamped(m_Color2, m_Color1, nomalizedPos.y);

                // Correct color.
                vertex.color *= m_ColorSpace == ColorSpace.Gamma ? color.gamma
                    : m_ColorSpace == ColorSpace.Linear ? color.linear
                    : color;

                vh.SetUIVertex(vertex, i);
            }
        }

        /// <summary>
        ///     Matrix2x3.
        /// </summary>
        private struct Matrix2x3
        {
            public readonly float m00;
            public readonly float m01;
            public readonly float m02;
            public readonly float m10;
            public readonly float m11;
            public readonly float m12;

            public Matrix2x3(Rect rect, float cos, float sin)
            {
                const float center = 0.5f;
                var dx = -rect.xMin / rect.width - center;
                var dy = -rect.yMin / rect.height - center;
                m00 = cos / rect.width;
                m01 = -sin / rect.height;
                m02 = dx * cos - dy * sin + center;
                m10 = sin / rect.width;
                m11 = cos / rect.height;
                m12 = dx * sin + dy * cos + center;
            }

            public static Vector2 operator *(Matrix2x3 m, Vector2 v)
            {
                return new Vector2(
                    m.m00 * v.x + m.m01 * v.y + m.m02,
                    m.m10 * v.x + m.m11 * v.y + m.m12
                );
            }
        }
    }
}