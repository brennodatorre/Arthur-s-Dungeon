using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationEffect : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material pixelateMaterial;

        public CustomRenderPass(Material material)
        {
            this.pixelateMaterial = material;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;  // After everything is drawn
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Get the camera's current render target
            CommandBuffer cmd = CommandBufferPool.Get("PixelationPass");

            // Set the target to the screen
            cmd.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CameraTarget, pixelateMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    private CustomRenderPass customPass;
    public Material pixelationMaterial;

    public override void Create()
    {
        customPass = new CustomRenderPass(pixelationMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customPass);
    }
}
