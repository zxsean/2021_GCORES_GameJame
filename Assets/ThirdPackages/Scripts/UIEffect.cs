using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

#endif

namespace Coffee.UIExtensions
{
    /// <summary>
    ///     UIEffect.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UIEffect/UIEffect", 1)]
    public class UIEffect : UIEffectBase
    {
        public enum BlurEx
        {
            None = 0,
            Ex = 1
        }

        //################################
        // Constant or Static Members.
        //################################
        public const string shaderName = "UI/Hidden/UI-Effect";
        private static readonly ParameterTexture _ptex = new ParameterTexture(4, 1024, "_ParamTex");

        [Tooltip("Advanced blurring remove common artifacts in the blur effect for uGUI.")] [SerializeField]
        private readonly bool m_AdvancedBlur = false;

        [FormerlySerializedAs("m_Blur")]
        [Tooltip("How far is the blurring from the graphic.")]
        [SerializeField]
        [Range(0, 1)]
        private float m_BlurFactor = 1;

        [Tooltip("Blur effect mode")] [SerializeField]
        private readonly BlurMode m_BlurMode = BlurMode.None;

        [Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")] [SerializeField] [Range(0, 1)]
        private float m_ColorFactor = 1;

        [Tooltip("Color effect mode")] [SerializeField]
        private readonly ColorMode m_ColorMode = ColorMode.Multiply;


        //################################
        // Serialize Members.
        //################################
        [FormerlySerializedAs("m_ToneLevel")]
        [Tooltip("Effect factor between 0(no effect) and 1(complete effect).")]
        [SerializeField]
        [Range(0, 1)]
        private float m_EffectFactor = 1;

        [FormerlySerializedAs("m_ToneMode")] [Tooltip("Effect mode")] [SerializeField]
        private readonly EffectMode m_EffectMode = EffectMode.None;

        //################################
        // Public Members.
        //################################
#if UNITY_2017_1_OR_NEWER
        public override AdditionalCanvasShaderChannels requiredChannels
        {
            get
            {
                if (advancedBlur)
                    return isTMPro
                        ? AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2
                        : AdditionalCanvasShaderChannels.TexCoord1;
                return AdditionalCanvasShaderChannels.None;
            }
        }
#endif

        /// <summary>
        ///     Effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        [Obsolete("Use effectFactor instead (UnityUpgradable) -> effectFactor")]
        public float toneLevel
        {
            get => m_EffectFactor;
            set
            {
                m_EffectFactor = Mathf.Clamp(value, 0, 1);
                SetDirty();
            }
        }

        /// <summary>
        ///     Effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float effectFactor
        {
            get => m_EffectFactor;
            set
            {
                m_EffectFactor = Mathf.Clamp(value, 0, 1);
                SetDirty();
            }
        }

        /// <summary>
        ///     Color effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float colorFactor
        {
            get => m_ColorFactor;
            set
            {
                m_ColorFactor = Mathf.Clamp(value, 0, 1);
                SetDirty();
            }
        }

        /// <summary>
        ///     How far is the blurring from the graphic.
        /// </summary>
        [Obsolete("Use blurFactor instead (UnityUpgradable) -> blurFactor")]
        public float blur
        {
            get => m_BlurFactor;
            set
            {
                m_BlurFactor = Mathf.Clamp(value, 0, 1);
                SetDirty();
            }
        }

        /// <summary>
        ///     How far is the blurring from the graphic.
        /// </summary>
        [Obsolete("Use effectFactor instead (UnityUpgradable) -> effectFactor")]
        public float blurFactor
        {
            get => m_BlurFactor;
            set
            {
                m_BlurFactor = Mathf.Clamp(value, 0, 1);
                SetDirty();
            }
        }

        /// <summary>
        ///     Effect mode(readonly).
        /// </summary>
        [Obsolete("Use effectMode instead (UnityUpgradable) -> effectMode")]
        public EffectMode toneMode => m_EffectMode;

        /// <summary>
        ///     Effect mode(readonly).
        /// </summary>
        public EffectMode effectMode => m_EffectMode;

        /// <summary>
        ///     Color effect mode(readonly).
        /// </summary>
        public ColorMode colorMode => m_ColorMode;

        /// <summary>
        ///     Blur effect mode(readonly).
        /// </summary>
        public BlurMode blurMode => m_BlurMode;

        /// <summary>
        ///     Color for the color effect.
        /// </summary>
        public Color effectColor
        {
            get => graphic.color;
            set
            {
                graphic.color = value;
                SetDirty();
            }
        }

        /// <summary>
        ///     Gets the parameter texture.
        /// </summary>
        public override ParameterTexture ptex => _ptex;

        /// <summary>
        ///     Advanced blurring remove common artifacts in the blur effect for uGUI.
        /// </summary>
        public bool advancedBlur => isTMPro ? material && material.IsKeywordEnabled("EX") : m_AdvancedBlur;

        /// <summary>
        ///     Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled) return;

            var normalizedIndex = ptex.GetNormalizedIndex(this);

            if (m_BlurMode != BlurMode.None && advancedBlur)
            {
                vh.GetUIVertexStream(tempVerts);
                vh.Clear();
                var count = tempVerts.Count;

                // Bundle
                var bundleSize = targetGraphic is Text || isTMPro ? 6 : count;
                Rect posBounds = default;
                var uvBounds = default(Rect);
                var size = default(Vector3);
                var tPos = default(Vector3);
                var tUV = default(Vector3);
                var expand = (float) blurMode * 6 * 2;

                for (var i = 0; i < count; i += bundleSize)
                {
                    // min/max for bundled-quad
                    GetBounds(tempVerts, i, bundleSize, ref posBounds, ref uvBounds, true);

                    // Pack uv mask.
                    var uvMask = new Vector2(Packer.ToFloat(uvBounds.xMin, uvBounds.yMin),
                        Packer.ToFloat(uvBounds.xMax, uvBounds.yMax));

                    // Quad
                    for (var j = 0; j < bundleSize; j += 6)
                    {
                        var cornerPos1 = tempVerts[i + j + 1].position;
                        var cornerPos2 = tempVerts[i + j + 4].position;

                        // Is outer quad?
                        var hasOuterEdge = bundleSize == 6
                                           || !posBounds.Contains(cornerPos1)
                                           || !posBounds.Contains(cornerPos2);
                        if (hasOuterEdge)
                        {
                            Vector3 cornerUv1 = tempVerts[i + j + 1].uv0;
                            Vector3 cornerUv2 = tempVerts[i + j + 4].uv0;

                            var centerPos = (cornerPos1 + cornerPos2) / 2;
                            var centerUV = (cornerUv1 + cornerUv2) / 2;
                            size = cornerPos1 - cornerPos2;

                            size.x = 1 + expand / Mathf.Abs(size.x);
                            size.y = 1 + expand / Mathf.Abs(size.y);
                            size.z = 1 + expand / Mathf.Abs(size.z);

                            tPos = centerPos - Vector3.Scale(size, centerPos);
                            tUV = centerUV - Vector3.Scale(size, centerUV);
                        }

                        // Vertex
                        for (var k = 0; k < 6; k++)
                        {
                            var vt = tempVerts[i + j + k];

                            var pos = vt.position;
                            var uv0 = vt.uv0;

                            if (hasOuterEdge && (pos.x < posBounds.xMin || posBounds.xMax < pos.x))
                            {
                                pos.x = pos.x * size.x + tPos.x;
                                uv0.x = uv0.x * size.x + tUV.x;
                            }

                            if (hasOuterEdge && (pos.y < posBounds.yMin || posBounds.yMax < pos.y))
                            {
                                pos.y = pos.y * size.y + tPos.y;
                                uv0.y = uv0.y * size.y + tUV.y;
                            }

                            vt.uv0 = new Vector2(Packer.ToFloat((uv0.x + 0.5f) / 2f, (uv0.y + 0.5f) / 2f),
                                normalizedIndex);
                            vt.position = pos;

                            if (isTMPro)
                            {
#if UNITY_2017_1_OR_NEWER
                                vt.uv2 = uvMask;
#endif
                            }
                            else
                            {
                                vt.uv1 = uvMask;
                            }

                            tempVerts[i + j + k] = vt;
                        }
                    }
                }

                vh.AddUIVertexTriangleStream(tempVerts);
                tempVerts.Clear();
            }
            else
            {
                var count = vh.currentVertCount;
                var vt = default(UIVertex);
                for (var i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref vt, i);
                    var uv0 = vt.uv0;
                    vt.uv0 = new Vector2(
                        Packer.ToFloat((uv0.x + 0.5f) / 2f, (uv0.y + 0.5f) / 2f),
                        normalizedIndex
                    );
                    vh.SetUIVertex(vt, i);
                }
            }
        }

        protected override void SetDirty()
        {
            foreach (var m in materials) ptex.RegisterMaterial(m);
            ptex.SetData(this, 0, m_EffectFactor); // param.x : effect factor
            ptex.SetData(this, 1, m_ColorFactor); // param.y : color factor
            ptex.SetData(this, 2, m_BlurFactor); // param.z : blur factor
        }

        //################################
        // Private Members.
        //################################
        private static void GetBounds(List<UIVertex> verts, int start, int count, ref Rect posBounds, ref Rect uvBounds,
            bool global)
        {
            var minPos = new Vector2(float.MaxValue, float.MaxValue);
            var maxPos = new Vector2(float.MinValue, float.MinValue);
            var minUV = new Vector2(float.MaxValue, float.MaxValue);
            var maxUV = new Vector2(float.MinValue, float.MinValue);
            for (var i = start; i < start + count; i++)
            {
                var vt = verts[i];

                var uv = vt.uv0;
                var pos = vt.position;

                // Left-Bottom
                if (minPos.x >= pos.x && minPos.y >= pos.y)
                    minPos = pos;
                // Right-Top
                else if (maxPos.x <= pos.x && maxPos.y <= pos.y) maxPos = pos;

                // Left-Bottom
                if (minUV.x >= uv.x && minUV.y >= uv.y)
                    minUV = uv;
                // Right-Top
                else if (maxUV.x <= uv.x && maxUV.y <= uv.y) maxUV = uv;
            }

            // Shrink coordinate for detect edge
            posBounds.Set(minPos.x + 0.001f, minPos.y + 0.001f, maxPos.x - minPos.x - 0.002f,
                maxPos.y - minPos.y - 0.002f);
            uvBounds.Set(minUV.x, minUV.y, maxUV.x - minUV.x, maxUV.y - minUV.y);
        }

#pragma warning disable 0414
        [Obsolete] [HideInInspector] [SerializeField] [Range(0, 1)]
        private readonly float m_ShadowBlur = 1;

        [Obsolete] [HideInInspector] [SerializeField]
        private ShadowStyle m_ShadowStyle;

        [Obsolete] [HideInInspector] [SerializeField]
        private readonly Color m_ShadowColor = Color.black;

        [Obsolete] [HideInInspector] [SerializeField]
        private readonly Vector2 m_EffectDistance = new Vector2(1f, -1f);

        [Obsolete] [HideInInspector] [SerializeField]
        private readonly bool m_UseGraphicAlpha = true;

        [Obsolete] [HideInInspector] [SerializeField]
        private readonly Color m_EffectColor = Color.white;

        [Obsolete] [HideInInspector] [SerializeField]
        private List<UIShadow.AdditionalShadow> m_AdditionalShadows = new List<UIShadow.AdditionalShadow>();
#pragma warning restore 0414

#if UNITY_EDITOR
        /// <summary>
        ///     Gets the material.
        /// </summary>
        /// <returns>The material.</returns>
        protected override Material GetMaterial()
        {
            if (isTMPro) return null;
            return MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_EffectMode, m_ColorMode,
                m_BlurMode, m_AdvancedBlur ? BlurEx.Ex : BlurEx.None);
        }

#pragma warning disable 0612
        protected override void UpgradeIfNeeded()
        {
            // Upgrade for v3.0.0
            if (IsShouldUpgrade(300))
            {
                if (m_ColorMode != ColorMode.Multiply)
                {
                    var col = targetGraphic.color;
                    col.r = m_EffectColor.r;
                    col.g = m_EffectColor.g;
                    col.b = m_EffectColor.b;
                    targetGraphic.color = col;
                    m_ColorFactor = m_EffectColor.a;
                }

                if (m_ShadowStyle != ShadowStyle.None || m_AdditionalShadows.Any(x => x.style != ShadowStyle.None))
                {
                    if (m_ShadowStyle != ShadowStyle.None)
                    {
                        var shadow = gameObject.GetComponent<UIShadow>() ?? gameObject.AddComponent<UIShadow>();
                        shadow.style = m_ShadowStyle;
                        shadow.effectDistance = m_EffectDistance;
                        shadow.effectColor = m_ShadowColor;
                        shadow.useGraphicAlpha = m_UseGraphicAlpha;
                        shadow.blurFactor = m_ShadowBlur;
                    }

                    foreach (var s in m_AdditionalShadows)
                    {
                        if (s.style == ShadowStyle.None) continue;

                        var shadow = gameObject.AddComponent<UIShadow>();
                        shadow.style = s.style;
                        shadow.effectDistance = s.effectDistance;
                        shadow.effectColor = s.effectColor;
                        shadow.useGraphicAlpha = s.useGraphicAlpha;
                        shadow.blurFactor = s.blur;
                    }

                    m_ShadowStyle = ShadowStyle.None;
                    m_AdditionalShadows = null;

                    if (m_EffectMode == EffectMode.None && m_ColorMode == ColorMode.Multiply &&
                        m_BlurMode == BlurMode.None) DestroyImmediate(this, true);
                }

                var tone = (int) m_EffectMode;
                const int Mono = 5;
                const int Cutoff = 6;
                const int Hue = 7;
                if (tone == Hue)
                {
                    var go = gameObject;
                    var hue = m_EffectFactor;
                    DestroyImmediate(this, true);
                    var hsv = go.GetComponent<UIHsvModifier>() ?? go.AddComponent<UIHsvModifier>();
                    hsv.hue = hue;
                    hsv.range = 1;
                }

                // Cutoff/Mono
                if (tone == Cutoff || tone == Mono)
                {
                    var go = gameObject;
                    var factor = m_EffectFactor;
                    var transitionMode = tone == Cutoff
                        ? UITransitionEffect.EffectMode.Cutoff
                        : UITransitionEffect.EffectMode.Fade;
                    DestroyImmediate(this, true);
                    var trans = go.GetComponent<UITransitionEffect>() ?? go.AddComponent<UITransitionEffect>();
                    trans.effectFactor = factor;

                    var sp = new SerializedObject(trans).FindProperty("m_EffectMode");
                    sp.intValue = (int) transitionMode;
                    sp.serializedObject.ApplyModifiedProperties();
                }
            }
        }
#pragma warning restore 0612
#endif
    }
}