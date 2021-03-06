using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class ZoomBlurPass : ScriptableRenderPass
{
    private static readonly string s_RenderTag = "Render ZoomBlur Effects";
    
    private static readonly int s_MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int s_TempTargetId = Shader.PropertyToID("_TempTargetZoomBlur");
    private static readonly int s_FocusPowerId = Shader.PropertyToID("_FocusPower");
    private static readonly int s_FocusDetailId = Shader.PropertyToID("_FocusDetail");
    private static readonly int s_FocusScreenPositionId = Shader.PropertyToID("_FocusScreenPosition");
    private static readonly int s_ReferenceResolutionId = Shader.PropertyToID("_ReferenceResolutionX");
    
    private RenderTargetIdentifier m_CurrentTarget;
    private ZoomBlurSettings m_ZoomBlur;
    private readonly Material m_ZoomBlurMaterial;

    public ZoomBlurPass(RenderPassEvent passEvent)
    {
        renderPassEvent = passEvent;
        Shader shader = Shader.Find("PostEffect/ZoomBlur");
        if (shader == null)
        {
            Debug.LogError("Shader Not Found");
            return;
        }

        m_ZoomBlurMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.m_CurrentTarget = currentTarget;
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (m_ZoomBlurMaterial == null)
        {
            Debug.LogError("Material Not Found");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            return;
        }
        VolumeStack stack = VolumeManager.instance.stack;
        m_ZoomBlur = stack.GetComponent<ZoomBlurSettings>();
        if (m_ZoomBlur == null || !m_ZoomBlur.IsActive())
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get(s_RenderTag);
        Render(cmd,ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void Render(CommandBuffer cmd ,ref RenderingData renderingData)
    {
        ref CameraData cameraData = ref renderingData.cameraData;
        RenderTargetIdentifier source = m_CurrentTarget;
        int destination = s_TempTargetId;
        int width = cameraData.camera.scaledPixelWidth;
        int height = cameraData.camera.scaledPixelHeight;
        
        m_ZoomBlurMaterial.SetFloat(s_FocusPowerId,m_ZoomBlur.FocusPower.value);
        m_ZoomBlurMaterial.SetInt(s_FocusDetailId, m_ZoomBlur.FocusDetials.value);
        m_ZoomBlurMaterial.SetVector(s_FocusScreenPositionId,m_ZoomBlur.FocusScreenPosition.value);
        m_ZoomBlurMaterial.SetInt(s_ReferenceResolutionId,m_ZoomBlur.ReferenceResolutionX.value);

        int shaderPass = 0;
        cmd.SetGlobalTexture(s_MainTexId,source);
        cmd.GetTemporaryRT(destination, width, height, 0, FilterMode.Point,RenderTextureFormat.Default);
        cmd.Blit(source,destination);
        cmd.Blit(destination,source,m_ZoomBlurMaterial,shaderPass);
    }
}