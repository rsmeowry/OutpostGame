using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Shaders
{
public class RenderToDepthTexture : ScriptableRendererFeature {

	public LayerMask layerMask;

	public RenderPassEvent _event = RenderPassEvent.BeforeRenderingSkybox;
	/* 
	- Set to at least AfterRenderingPrePasses if depth texture is generated via a Depth Prepass,
		or it will just be overriden
	- Set to at least BeforeRenderingSkybox if depth texture is generated via a Copy Depth pass,
	  	Anything before this is already included in the texture! (Though not for Scene View as that always uses a prepass)
	*/

	class RenderToDepthTexturePass : ScriptableRenderPass {

		private ProfilingSampler m_ProfilingSampler;
		private FilteringSettings m_FilteringSettings;
		private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

		private RTHandle depthTex;

		public RenderToDepthTexturePass(LayerMask layerMask) {
			m_ProfilingSampler = new ProfilingSampler("RenderToDepthTexture");
			m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
			m_ShaderTagIdList.Add(new ShaderTagId("DepthOnly")); // Only render DepthOnly pass
			depthTex = RTHandles.Alloc(Vector2.one, depthBufferBits: DepthBits.Depth32, dimension: TextureDimension.Tex2D, name: "_CameraDepthTexture");
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
			ConfigureTarget(depthTex);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
			SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);

			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, m_ProfilingSampler)) {
				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
			}

			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public override void OnCameraCleanup(CommandBuffer cmd) { }
	}

	RenderToDepthTexturePass m_ScriptablePass;

	public override void Create() {
		m_ScriptablePass = new RenderToDepthTexturePass(layerMask);
		m_ScriptablePass.renderPassEvent = _event;
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
		if (!renderingData.cameraData.requiresDepthTexture) return;
		renderer.EnqueuePass(m_ScriptablePass);
	}
}
}