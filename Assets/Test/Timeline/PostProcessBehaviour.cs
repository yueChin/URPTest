﻿using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class PostProcessBehaviour : PlayableBehaviour
{
    public Volume Volume;

    public VolumeProfile Profile;

    public AnimationCurve Weight = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    
    public int Layer;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (Profile != null)
        {
            QuickVolume();
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (Volume != null)
        {
            GameObject.DestroyImmediate(Volume.gameObject);
        }
    }

    public void QuickVolume()
    {
        if (Volume == null)
        {
            
        }
    }
    
    public void ChangeWeight(float time)
    {
        if (Volume == null)
        {
            return;
        }

        Volume.weight = Weight.Evaluate(time);
    }
}
