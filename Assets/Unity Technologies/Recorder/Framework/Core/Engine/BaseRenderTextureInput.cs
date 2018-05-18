using UnityEngine;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    /// <summary>
    /// What is this: Base class for inputs that provide a RenderRexture as output to Recorders
    /// Motivation  : Having a base class for render texture Inputs greatly simplifies Recorders job of supporting
    ///               multiple input implementations for fetching images.
    /// </summary>    
    public abstract class BaseRenderTextureInput : RecorderInput
    {
        public RenderTexture outputRT { get; set; }

        public int outputWidth { get; protected set; }
        public int outputHeight { get; protected set; }

        public void ReleaseBuffer()
        {
            if (this.outputRT != null)
            {
                if (this.outputRT == RenderTexture.active)
                    RenderTexture.active = null;

                this.outputRT.Release();
                this.outputRT = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                this.ReleaseBuffer();
        }
    }
}
