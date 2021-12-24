using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public partial class PostProcessClip : PlayableAsset,ITimelineClipAsset
{
    public PostProcessBehaviour Template = new PostProcessBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<PostProcessBehaviour> playable = ScriptPlayable<PostProcessBehaviour>.Create(graph, Template);
        PostProcessBehaviour cloone = playable.GetBehaviour();
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.Extrapolation | ClipCaps.Blending;
}