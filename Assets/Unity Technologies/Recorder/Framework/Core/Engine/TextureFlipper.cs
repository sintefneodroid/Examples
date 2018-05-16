using System;
using UnityEngine;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    public class TextureFlipper : IDisposable
    {
        Shader          m_shVFlip;
        Material        m_VFLipMaterial;
        RenderTexture   m_WorkTexture;

        public TextureFlipper()
        {
            this.m_shVFlip = Shader.Find("Hidden/Unity/Recorder/Custom/VerticalFlipper");
            this.m_VFLipMaterial = new Material(this.m_shVFlip);
        }

        public void Flip(RenderTexture target)
        {
            if (this.m_WorkTexture == null || this.m_WorkTexture.width != target.width || this.m_WorkTexture.height != target.height)
            {
                UnityHelpers.Destroy(this.m_WorkTexture);
                this.m_WorkTexture = new RenderTexture(target.width, target.height, target.depth, target.format, RenderTextureReadWrite.Linear);
            }
            Graphics.Blit( target, this.m_WorkTexture, this.m_VFLipMaterial );
            Graphics.Blit( this.m_WorkTexture, target );            
        }

        public void Dispose()
        {
            UnityHelpers.Destroy(this.m_WorkTexture);
            this.m_WorkTexture = null;
            UnityHelpers.Destroy(this.m_VFLipMaterial);
            this.m_VFLipMaterial = null;
        }

    }
}
