using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrabFeature : ScriptableRendererFeature
{
    private GrabPass m_ScriptablePass;
    public GrabSettings m_Setting;

    public override void Create()
    {
        m_ScriptablePass = new GrabPass(m_Setting);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera)
            return;

        if (renderingData.postProcessingEnabled == false)
            return;

        m_ScriptablePass.SetUp(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}