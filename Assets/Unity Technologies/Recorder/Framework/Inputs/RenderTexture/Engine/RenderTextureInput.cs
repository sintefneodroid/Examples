using System;
using Unity_Technologies.Recorder.Framework.Core.Engine;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine
{
    public class RenderTextureInput : BaseRenderTextureInput
    {
        RenderTextureInputSettings cbSettings
        {
            get { return (RenderTextureInputSettings)this.settings; }
        }

        public override void BeginRecording(RecordingSession session)
        {
            if (this.cbSettings.m_SourceRTxtr == null)
                throw new Exception("No Render Texture object provided as source");

            this.outputHeight = this.cbSettings.m_SourceRTxtr.height;
            this.outputWidth = this.cbSettings.m_SourceRTxtr.width;
            this.outputRT = this.cbSettings.m_SourceRTxtr;
        }
    }
}