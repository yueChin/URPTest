using UnityEngine.Rendering.Universal;

public class ZoomBlurRenderFeature : ScriptableRendererFeature
{
    private ZoomBlurPass m_ZoomBlurPass;

    public override void Create()
    {
        m_ZoomBlurPass = new ZoomBlurPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ZoomBlurPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ZoomBlurPass);
    }
}

