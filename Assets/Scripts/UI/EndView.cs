using UnityEngine;

public class EndView : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public RectTransform content;

    private void Start()
    {
        AudioMgr.PlayMusic(Game.EndMusic);
    }

    private void Update()
    {
        if (content.anchoredPosition.y <= 1240)
        {
            var pos = content.anchoredPosition;
            pos.y += speed * Time.deltaTime;
            content.anchoredPosition = pos;
        }
    }
}