using UnityEngine;
using UnityEngine.UI;

public class StartView : MonoBehaviour
{
    public Animation anim;
    public Button startBtn;

    private bool CanClicked { get; set; }

    private void Start()
    {
        CanClicked = false;
        AudioMgr.PlayContinueMusic(Game.NormalMusic);
        anim.Play("startview", () => { CanClicked = true; });
        startBtn.onClick.AddListener(() =>
        {
            if (!CanClicked) return;
            Game.Start();
        });
    }
}