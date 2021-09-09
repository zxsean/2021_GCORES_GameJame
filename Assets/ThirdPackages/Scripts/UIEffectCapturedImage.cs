using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    /// <summary>
    ///     UIEffectCapturedImage
    /// </summary>
    [AddComponentMenu("UI/UIEffect/UIEffectCapturedImage", 200)]
    public class UIEffectCapturedImage : RawImage
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        /// <summary>
        ///     Desampling rate.
        /// </summary>
        public enum DesamplingRate
        {
            None = 0,
            x1 = 1,
            x2 = 2,
            x4 = 4,
            x8 = 8
        }

        //################################
        // Constant or Static Members.
        //################################
        public const string shaderName = "UI/Hidden/UI-EffectCapture";

        private static int s_CopyId;
        private static int s_EffectId1;
        private static int s_EffectId2;
        private static int s_EffectFactorId;
        private static int s_ColorFactorId;
        private static CommandBuffer s_CommandBuffer;


        //################################
        // Private Members.
        //################################
        private RenderTexture _rt;
        private RenderTargetIdentifier _rtId;

        [Tooltip("How far is the blurring from the graphic.")]
        [FormerlySerializedAs("m_Blur")]
        [SerializeField]
        [Range(0, 1)]
        private float m_BlurFactor = 1;

        [Tooltip("Blur iterations.")] [FormerlySerializedAs("m_Iterations")] [SerializeField] [Range(1, 8)]
        private int m_BlurIterations = 3;

        [Tooltip("Blur effect mode.")] [SerializeField]
        private readonly BlurMode m_BlurMode = BlurMode.DetailBlur;

        [Tooltip("Capture automatically on enable.")] [SerializeField]
        private bool m_CaptureOnEnable;

        [Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")] [SerializeField] [Range(0, 1)]
        private float m_ColorFactor = 1;

        [Tooltip("Color effect mode.")] [SerializeField]
        private readonly ColorMode m_ColorMode = ColorMode.Multiply;

        [Tooltip("Desampling rate of the generated RenderTexture.")] [SerializeField]
        private DesamplingRate m_DesamplingRate = DesamplingRate.x1;

        [Tooltip("Color for the color effect.")] [SerializeField]
        private Color m_EffectColor = Color.white;


        //################################
        // Serialize Members.
        //################################
        [Tooltip("Effect factor between 0(no effect) and 1(complete effect).")]
        [FormerlySerializedAs("m_ToneLevel")]
        [SerializeField]
        [Range(0, 1)]
        private float m_EffectFactor = 1;

        [Tooltip("Effect material.")] [SerializeField]
        private Material m_EffectMaterial;

        [Tooltip("Effect mode.")] [FormerlySerializedAs("m_ToneMode")] [SerializeField]
        private readonly EffectMode m_EffectMode = EffectMode.None;

        [Tooltip("FilterMode for capturing.")] [SerializeField]
        private FilterMode m_FilterMode = FilterMode.Bilinear;

        [Tooltip("Fits graphic size to screen on captured.")]
        [FormerlySerializedAs("m_KeepCanvasSize")]
        [SerializeField]
        private bool m_FitToScreen = true;

        [Tooltip("Desampling rate of reduction buffer to apply effect.")] [SerializeField]
        private DesamplingRate m_ReductionRate = DesamplingRate.x1;


        //################################
        // Public Members.
        //################################
        /// <summary>
        ///     Effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        [Obsolete("Use effectFactor instead (UnityUpgradable) -> effectFactor")]
        public float toneLevel
        {
            get => m_EffectFactor;
            set => m_EffectFactor = Mathf.Clamp(value, 0, 1);
        }

        /// <summary>
        ///     Effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float effectFactor
        {
            get => m_EffectFactor;
            set => m_EffectFactor = Mathf.Clamp(value, 0, 1);
        }

        /// <summary>
        ///     Color effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float colorFactor
        {
            get => m_ColorFactor;
            set => m_ColorFactor = Mathf.Clamp(value, 0, 1);
        }

        /// <summary>
        ///     How far is the blurring from the graphic.
        /// </summary>
        [Obsolete("Use blurFactor instead (UnityUpgradable) -> blurFactor")]
        public float blur
        {
            get => m_BlurFactor;
            set => m_BlurFactor = Mathf.Clamp(value, 0, 4);
        }

        /// <summary>
        ///     How far is the blurring from the graphic.
        /// </summary>
        public float blurFactor
        {
            get => m_BlurFactor;
            set => m_BlurFactor = Mathf.Clamp(value, 0, 4);
        }

        /// <summary>
        ///     Tone effect mode.
        /// </summary>
        [Obsolete("Use effectMode instead (UnityUpgradable) -> effectMode")]
        public EffectMode toneMode => m_EffectMode;

        /// <summary>
        ///     Effect mode.
        /// </summary>
        public EffectMode effectMode => m_EffectMode;

        /// <summary>
        ///     Color effect mode.
        /// </summary>
        public ColorMode colorMode => m_ColorMode;

        /// <summary>
        ///     Blur effect mode.
        /// </summary>
        public BlurMode blurMode => m_BlurMode;

        /// <summary>
        ///     Color for the color effect.
        /// </summary>
        public Color effectColor
        {
            get => m_EffectColor;
            set => m_EffectColor = value;
        }

        /// <summary>
        ///     Effect material.
        /// </summary>
        public virtual Material effectMaterial => m_EffectMaterial;

        /// <summary>
        ///     Desampling rate of the generated RenderTexture.
        /// </summary>
        public DesamplingRate desamplingRate
        {
            get => m_DesamplingRate;
            set => m_DesamplingRate = value;
        }

        /// <summary>
        ///     Desampling rate of reduction buffer to apply effect.
        /// </summary>
        public DesamplingRate reductionRate
        {
            get => m_ReductionRate;
            set => m_ReductionRate = value;
        }

        /// <summary>
        ///     FilterMode for capturing.
        /// </summary>
        public FilterMode filterMode
        {
            get => m_FilterMode;
            set => m_FilterMode = value;
        }

        /// <summary>
        ///     Captured texture.
        /// </summary>
        public RenderTexture capturedTexture => _rt;

        /// <summary>
        ///     Blur iterations.
        /// </summary>
        [Obsolete("Use blurIterations instead (UnityUpgradable) -> blurIterations")]
        public int iterations
        {
            get => m_BlurIterations;
            set => m_BlurIterations = value;
        }

        /// <summary>
        ///     Blur iterations.
        /// </summary>
        public int blurIterations
        {
            get => m_BlurIterations;
            set => m_BlurIterations = value;
        }

        /// <summary>
        ///     Fits graphic size to screen.
        /// </summary>
        [Obsolete("Use fitToScreen instead (UnityUpgradable) -> fitToScreen")]
        public bool keepCanvasSize
        {
            get => m_FitToScreen;
            set => m_FitToScreen = value;
        }

        /// <summary>
        ///     Fits graphic size to screen on captured.
        /// </summary>
        public bool fitToScreen
        {
            get => m_FitToScreen;
            set => m_FitToScreen = value;
        }

        /// <summary>
        ///     Target RenderTexture to capture.
        /// </summary>
        [Obsolete]
        public RenderTexture targetTexture
        {
            get => null;
            set { }
        }

        /// <summary>
        ///     Capture automatically on enable.
        /// </summary>
        public bool captureOnEnable
        {
            get => m_CaptureOnEnable;
            set => m_CaptureOnEnable = value;
        }

        /// <summary>
        ///     This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_CaptureOnEnable && Application.isPlaying) Capture();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_CaptureOnEnable && Application.isPlaying)
            {
                _Release(false);
                texture = null;
            }
        }

        /// <summary>
        ///     This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            Release();
            base.OnDestroy();
        }

        /// <summary>
        ///     Callback function when a UI element needs to generate vertices.
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // When not displaying, clear vertex.
            if (texture == null || color.a < 1 / 255f || canvasRenderer.GetAlpha() < 1 / 255f)
            {
                vh.Clear();
            }
            else
            {
                base.OnPopulateMesh(vh);
                var count = vh.currentVertCount;
                UIVertex vt = default;
                var c = color;
                for (var i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref vt, i);
                    vt.color = c;
                    vh.SetUIVertex(vt, i);
                }
            }
        }

        /// <summary>
        ///     Gets the size of the desampling.
        /// </summary>
        public void GetDesamplingSize(DesamplingRate rate, out int w, out int h)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var res = UnityStats.screenRes.Split('x');
                w = int.Parse(res[0]);
                h = int.Parse(res[1]);
            }
            else
#endif
            {
                w = Screen.width;
                h = Screen.height;
            }

            if (rate == DesamplingRate.None)
                return;

            var aspect = (float) w / h;
            if (w < h)
            {
                h = Mathf.ClosestPowerOfTwo(h / (int) rate);
                w = Mathf.CeilToInt(h * aspect);
            }
            else
            {
                w = Mathf.ClosestPowerOfTwo(w / (int) rate);
                h = Mathf.CeilToInt(w / aspect);
            }
        }

        /// <summary>
        ///     Capture rendering result.
        /// </summary>
        public void Capture()
        {
            // Fit to screen.
            var rootCanvas = canvas.rootCanvas;
            if (m_FitToScreen)
            {
                var rootTransform = rootCanvas.transform as RectTransform;
                var size = rootTransform.rect.size;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
                rectTransform.position = rootTransform.position;
            }

            // Cache some ids.
            if (s_CopyId == 0)
            {
                s_CopyId = Shader.PropertyToID("_UIEffectCapturedImage_ScreenCopyId");
                s_EffectId1 = Shader.PropertyToID("_UIEffectCapturedImage_EffectId1");
                s_EffectId2 = Shader.PropertyToID("_UIEffectCapturedImage_EffectId2");

                s_EffectFactorId = Shader.PropertyToID("_EffectFactor");
                s_ColorFactorId = Shader.PropertyToID("_ColorFactor");
                s_CommandBuffer = new CommandBuffer();
            }


            // If size of result RT has changed, release it.
            int w, h;
            GetDesamplingSize(m_DesamplingRate, out w, out h);
            if (_rt && (_rt.width != w || _rt.height != h)) _Release(ref _rt);

            // Generate RT for result.
            if (_rt == null)
            {
                _rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                _rt.filterMode = m_FilterMode;
                _rt.useMipMap = false;
                _rt.wrapMode = TextureWrapMode.Clamp;
                _rtId = new RenderTargetIdentifier(_rt);
            }

            SetupCommandBuffer();
        }

        private void SetupCommandBuffer()
        {
            // Material for effect.
            var mat = m_EffectMaterial;

            if (s_CommandBuffer == null) s_CommandBuffer = new CommandBuffer();

            // [1] Capture from back buffer (back buffer -> copied screen).
            int w, h;
            GetDesamplingSize(DesamplingRate.None, out w, out h);
            s_CommandBuffer.GetTemporaryRT(s_CopyId, w, h, 0, m_FilterMode);
#if UNITY_EDITOR
            s_CommandBuffer.Blit(
                Resources.FindObjectsOfTypeAll<RenderTexture>().FirstOrDefault(x => x.name == "GameView RT"), s_CopyId);
#else
			s_CommandBuffer.Blit(BuiltinRenderTextureType.BindableTexture, s_CopyId);
#endif

            // Set properties for effect.
            s_CommandBuffer.SetGlobalVector(s_EffectFactorId, new Vector4(m_EffectFactor, 0));
            s_CommandBuffer.SetGlobalVector(s_ColorFactorId,
                new Vector4(m_EffectColor.r, m_EffectColor.g, m_EffectColor.b, m_EffectColor.a));

            // [2] Apply base effect with reduction buffer (copied screen -> effect1).
            GetDesamplingSize(m_ReductionRate, out w, out h);
            s_CommandBuffer.GetTemporaryRT(s_EffectId1, w, h, 0, m_FilterMode);
            s_CommandBuffer.Blit(s_CopyId, s_EffectId1, mat, 0);
            s_CommandBuffer.ReleaseTemporaryRT(s_CopyId);

            // Iterate blurring operation.
            if (m_BlurMode != BlurMode.None)
            {
                s_CommandBuffer.GetTemporaryRT(s_EffectId2, w, h, 0, m_FilterMode);
                for (var i = 0; i < m_BlurIterations; i++)
                {
                    // [3] Apply blurring with reduction buffer (effect1 -> effect2, or effect2 -> effect1).
                    s_CommandBuffer.SetGlobalVector(s_EffectFactorId, new Vector4(m_BlurFactor, 0));
                    s_CommandBuffer.Blit(s_EffectId1, s_EffectId2, mat, 1);
                    s_CommandBuffer.SetGlobalVector(s_EffectFactorId, new Vector4(0, m_BlurFactor));
                    s_CommandBuffer.Blit(s_EffectId2, s_EffectId1, mat, 1);
                }

                s_CommandBuffer.ReleaseTemporaryRT(s_EffectId2);
            }

            // [4] Copy to result RT.
            s_CommandBuffer.Blit(s_EffectId1, _rtId);
            s_CommandBuffer.ReleaseTemporaryRT(s_EffectId1);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Graphics.ExecuteCommandBuffer(s_CommandBuffer);

                UpdateTexture();
                return;
            }
#endif
            // Execute command buffer.
            canvas.rootCanvas.GetComponent<CanvasScaler>().StartCoroutine(_CoUpdateTextureOnNextFrame());
        }

        /// <summary>
        ///     Release captured image.
        /// </summary>
        public void Release()
        {
            _Release(true);
            texture = null;
            _SetDirty();
        }

        /// <summary>
        ///     Release genarated objects.
        /// </summary>
        /// <param name="releaseRT">If set to <c>true</c> release cached RenderTexture.</param>
        private void _Release(bool releaseRT)
        {
            if (releaseRT)
            {
                texture = null;
                _Release(ref _rt);
            }

            if (s_CommandBuffer != null)
            {
                s_CommandBuffer.Clear();

                if (releaseRT)
                {
                    s_CommandBuffer.Release();
                    s_CommandBuffer = null;
                }
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void _SetDirty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) EditorUtility.SetDirty(this);
#endif
        }

        private void _Release(ref RenderTexture obj)
        {
            if (obj)
            {
                obj.Release();
                RenderTexture.ReleaseTemporary(obj);
                obj = null;
            }
        }

        /// <summary>
        ///     Set texture on next frame.
        /// </summary>
        private IEnumerator _CoUpdateTextureOnNextFrame()
        {
            yield return new WaitForEndOfFrame();
            UpdateTexture();
        }

        private void UpdateTexture()
        {
#if !UNITY_EDITOR
			// Execute command buffer.
			Graphics.ExecuteCommandBuffer (s_CommandBuffer);
#endif
            _Release(false);
            texture = capturedTexture;
            _SetDirty();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            // Set parameters as 'Medium'.
            m_BlurIterations = 3;
            m_FilterMode = FilterMode.Bilinear;
            m_DesamplingRate = DesamplingRate.x1;
            m_ReductionRate = DesamplingRate.x1;
            base.Reset();
        }

        /// <summary>
        ///     Raises the before serialize event.
        /// </summary>
        public void OnBeforeSerialize()
        {
        }

        /// <summary>
        ///     Raises the after deserialize event.
        /// </summary>
        public void OnAfterDeserialize()
        {
            EditorApplication.delayCall += () => UpdateMaterial(true);
        }

        /// <summary>
        ///     Raises the validate event.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            EditorApplication.delayCall += () => UpdateMaterial(false);
        }

        /// <summary>
        ///     Updates the material.
        /// </summary>
        /// <param name="ignoreInPlayMode">If set to <c>true</c> ignore in play mode.</param>
        protected void UpdateMaterial(bool ignoreInPlayMode)
        {
            if (!this || ignoreInPlayMode && Application.isPlaying) return;

            var mat = MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_EffectMode, m_ColorMode,
                m_BlurMode);
            if (m_EffectMaterial != mat)
            {
                material = null;
                m_EffectMaterial = mat;
                _SetDirty();
            }
        }
#endif
    }
}