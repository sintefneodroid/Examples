using System;
using System.IO;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.PNG
{
#if UNITY_2017_3_OR_NEWER
    [Obsolete("'UTJ/PNG' is obsolete, concider using 'Unity/Image Sequence' instead", false)]
    [Framework.Core.Engine.Recorder(typeof(PNGRecorderSettings),"Video", "UTJ/Legacy/PNG" )]
#else
    [Recorder(typeof(PNGRecorderSettings),"Video", "UTJ/PNG" )]
#endif
    
    public class PNGRecorder : Framework.Core.Engine.GenericRecorder<PNGRecorderSettings>
    {
        UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcPngContext m_ctx;

        public override bool BeginRecording(Framework.Core.Engine.RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            this.m_Settings.m_DestinationPath.CreateDirectory();

            this.m_ctx = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcPngCreateContext(ref this.m_Settings.m_PngEncoderSettings);
            return this.m_ctx;
        }

        public override void EndRecording(Framework.Core.Engine.RecordingSession session)
        {
            this.m_ctx.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(Framework.Core.Engine.RecordingSession session)
        {
            if (this.m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (Framework.Core.Engine.BaseRenderTextureInput)this.m_Inputs[0];
            var frame = input.outputRT;
            var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, frame.width, frame.height, "png");
            var path = Path.Combine(this.m_Settings.m_DestinationPath.GetFullPath(), fileName);

            UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcLock(frame, (data, fmt) =>
            {
                UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcPngExportPixels(this.m_ctx, path, data, frame.width, frame.height, fmt, 0);
            });
        }

    }
}
