using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationCallback : MonoBehaviour
{
    public void OnAnimationCallback(AnimationEvent i_event)
    {
        SendMessage(i_event.stringParameter);
    }

}
