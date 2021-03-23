using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrushPassRenderFeature : ScriptableRendererFeature
{
    private BrushPassRenderFeature _brushPass;

    class BrushRenderPass : ScriptableRenderPass
    {
        private                 FilteringSettings _filteringSettings;
        private                 RenderStateBlock  _renderStateBlock;
        private                 List<ShaderTagId> _shaderTagIds = new List<ShaderTagId>();
        private                 string            profilerTag;
        private                 bool              isOpaque;
        private static readonly int               drawObjectPassDataPropId = Shader.PropertyToID("_DrawObjectPassData");
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            throw new System.NotImplementedException();
        }
    }
    public override void Create()
    {
        _brushPass = new BrushPassRenderFeature();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        
        
    }
}
