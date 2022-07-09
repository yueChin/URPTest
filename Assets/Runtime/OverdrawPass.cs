using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace OverdrawForURP
{
	public class OverdrawPass : ScriptableRenderPass
	{
		private string m_ProfilerTag;
		private FilteringSettings m_FilteringSettings;
		private readonly List<ShaderTagId> m_TagIdList = new List<ShaderTagId>();
#if UNITY_2020_2_OR_NEWER
		private readonly ProfilingSampler m_Sampler;
#else
		private ProfilingSampler profilingSampler;
#endif
		private readonly bool m_IsOpaque;
		private readonly Material m_Material;

		public OverdrawPass(string profilerTag, RenderQueueRange renderQueueRange, Shader shader, bool isOpaque)
		{
			this.m_ProfilerTag = profilerTag;
			this.m_IsOpaque = isOpaque;

#if UNITY_2020_2_OR_NEWER
			profilingSampler = new ProfilingSampler(nameof(OverdrawPass));
			m_Sampler = new ProfilingSampler(profilerTag);
#else
			profilingSampler = new ProfilingSampler(profilerTag);
#endif
			m_TagIdList.Add(new ShaderTagId("UniversalForward"));
			m_TagIdList.Add(new ShaderTagId("LightweightForward"));
			m_TagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			m_FilteringSettings = new FilteringSettings(renderQueueRange, LayerMask.NameToLayer("Everything"));

			m_Material = CoreUtils.CreateEngineMaterial(shader);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
#if UNITY_2020_2_OR_NEWER
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, m_Sampler))
#else
			CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
			using (new ProfilingScope(cmd, profilingSampler))
#endif
			{
				Camera camera = renderingData.cameraData.camera;
				if (m_IsOpaque)
				{
					if (renderingData.cameraData.isSceneViewCamera 
					    || (camera.TryGetComponent(out UniversalAdditionalCameraData urpCameraData)
					    && urpCameraData.renderType == CameraRenderType.Base))
					{
						cmd.ClearRenderTarget(true, true, Color.black);
					}
				}
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();

				SortingCriteria sortFlags = m_IsOpaque ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
				DrawingSettings drawSettings = CreateDrawingSettings(m_TagIdList, ref renderingData, sortFlags);
				drawSettings.overrideMaterial = m_Material;
				context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}
}
