using UnityEngine;
using UnityEngine.UI;

public class BossView : MonoBehaviour
{
    private static readonly int ProgressID = Shader.PropertyToID("_Progress");
    public RawImage image;
    private Material mat;

    private void Awake()
    {
        mat = image.material;
        mat.SetFloat(ProgressID, 1.0f);
    }

    public void SetProgress(float progress)
    {
        mat.SetFloat(ProgressID, progress);
    }
}