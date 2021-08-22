using UnityEngine;
using UnityEngine.UI;

public class BossView : MonoBehaviour
{
    public RawImage image;
    private Material mat;

    private static int ProgressID = Shader.PropertyToID("_Progress");

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