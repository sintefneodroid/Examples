using System;
using System.IO;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.EXR
{
#if UNITY_2017_3_OR_NEWER
    [Obsolete("'UTJ/EXR' is obsolete, concider using 'Unity/Image Sequence' instead", false)]
    [Framework.Core.Engine.Recorder(typeof(EXRRecorderSettings),"Video", "UTJ/Legacy/OpenEXR" )]
#else
    [Recorder(typeof(EXRRecorderSettings),"Video", "UTJ/OpenEXR" )]
#endif
    public class EXRRecorder : Framework.Core.Engine.GenericRecorder<EXRRecorderSettings>
    {
        static readonly string[] s_channelNames = { "R", "G", "B", "A" };
        UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrContext m_ctx;

        public override bool BeginRecording(Framework.Core.Engine.RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            this.m_Settings.m_DestinationPath.CreateDirectory();

            this.m_ctx = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrCreateContext(ref this.m_Settings.m_ExrEncoderSettings);
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
            var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, frame.width, frame.height, "exr");
            var path = Path.Combine( this.settings.m_DestinationPath.GetFullPath(), fileName);

            UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcLock(frame, (data, fmt) =>
            {
                UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrBeginImage(this.m_ctx, path, frame.width, frame.height);
                int channels = (int)fmt & 7;
                for (int i = 0; i < channels; ++i)
                {
                    UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrAddLayerPixels(this.m_ctx, data, fmt, i, s_channelNames[i]);
                }
                UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrEndImage(this.m_ctx);
            });
        }

    }
}
