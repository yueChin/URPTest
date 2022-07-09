using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OverdrawForURP
{
	public class OverdrawRendererFeature : ScriptableRendererFeature
	{
#if UNITY_EDITOR || USE_RUNTIME_OVERDRAW
		private OverdrawPass m_OpaquePass;
		private OverdrawPass m_TransparentPass;

		[SerializeField] 
		private Shader opaqueShader = null;
		[SerializeField] 
		private Shader transparentShader = null;
#endif

		public override void Create()
		{
#if UNITY_EDITOR || USE_RUNTIME_OVERDRAW
			if (!opaqueShader || !transparentShader)
			{
				return;
			}
			m_OpaquePass = new OverdrawPass("Overdraw Render Opaque", RenderQueueRange.opaque, opaqueShader, true)
				{
					renderPassEvent = RenderPassEvent.AfterRenderingSkybox
				};
			m_TransparentPass = new OverdrawPass("Overdraw Render Transparent", RenderQueueRange.transparent, transparentShader, false)
				{
					renderPassEvent = RenderPassEvent.AfterRenderingTransparents
				};
#endif
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
#if UNITY_EDITOR || USE_RUNTIME_OVERDRAW
			renderer.EnqueuePass(m_OpaquePass);
			renderer.EnqueuePass(m_TransparentPass);
#endif
		}
	}
}
