using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

public class TransitionView : MonoBehaviour
{
    public UITransitionEffect effect;

    public void PlayShow()
    {
        effect.Show();
    }

    public void PlayHide()
    {
        effect.Hide();
    }
}
