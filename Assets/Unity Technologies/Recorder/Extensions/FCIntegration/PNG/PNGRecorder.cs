using System;
using System.IO;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.PNG
{
#if UNITY_2017_3_OR_NEWER
    [Obsolete("'UTJ/PNG' is obsolete, concider using 'Unity/Image Sequence' instead", false)]
    [Recorder(typeof(PNGRecorderSettings),"Video", "UTJ/Legacy/PNG" )]
#else
    [Recorder(typeof(PNGRecorderSettings),"Video", "UTJ/PNG" )]
#endif
    
    public class PNGRecorder : GenericRecorder<PNGRecorderSettings>
    {
        fcAPI.fcPngContext m_ctx;

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            this.m_Settings.m_DestinationPath.CreateDirectory();

            this.m_ctx = fcAPI.fcPngCreateContext(ref this.m_Settings.m_PngEncoderSettings);
            return this.m_ctx;
        }

        public override void EndRecording(RecordingSession session)
        {
            this.m_ctx.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (this.m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (BaseRenderTextureInput)this.m_Inputs[0];
            var frame = input.outputRT;
            var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, frame.width, frame.height, "png");
            var path = Path.Combine(this.m_Settings.m_DestinationPath.GetFullPath(), fileName);

            fcAPI.fcLock(frame, (data, fmt) =>
            {
                fcAPI.fcPngExportPixels(this.m_ctx, path, data, frame.width, frame.height, fmt, 0);
            });
        }

    }
}
