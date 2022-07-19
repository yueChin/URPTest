using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
class GrabPass : ScriptableRenderPass
{
    private static readonly string c_RenderTag = "grab pass";//可在framedebug中看渲染
    RenderTargetIdentifier currentTarget;
    RenderTargetHandle tempColorTarget;
    private string m_GrabPassName = "_GrabPassTexture";//shader中的grabpass名字
    public GrabPass(GrabSettings setting)
    {
        renderPassEvent = setting.Event;

        tempColorTarget.Init(m_GrabPassName);
    }

    public void SetUp(RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get(c_RenderTag);

        cmd.GetTemporaryRT(tempColorTarget.id, Screen.width, Screen.height);//获取临时rt
        cmd.SetGlobalTexture(m_GrabPassName, tempColorTarget.Identifier());//设置给shader中
        Blit(cmd, currentTarget, tempColorTarget.Identifier());

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }
}