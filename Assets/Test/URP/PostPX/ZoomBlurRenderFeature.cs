using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public partial class ZoomBlurRenderFeature : ScriptableRendererFeature
{
    private ZoomBlurPass m_ZoomBlurPass;

    public override void Create()
    {
        m_ZoomBlurPass = new ZoomBlurPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ZoomBlurPass);
    }
}