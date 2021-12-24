using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PostProcessMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCnt = playable.GetInputCount();
        for (int i = 0; i < inputCnt; i++)
        {
            ScriptPlayable<PostProcessBehaviour> playableInput = (ScriptPlayable<PostProcessBehaviour>)playable.GetInput(0);
            PostProcessBehaviour input = playableInput.GetBehaviour();
            float intpuWeight = playable.GetInputWeight(i);
            if (Mathf.Approximately(intpuWeight,0f))
            {
                continue;
            }
            float normalizedTime = (float)(playableInput.GetTime() / playableInput.GetDuration());
            input.ChangeWeight(normalizedTime);
        }
    }
}
