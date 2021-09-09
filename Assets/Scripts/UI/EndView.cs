using UnityEngine;

public class EndView : MonoBehaviour
{
    public RectTransform content;

    [SerializeField] private float speed;

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