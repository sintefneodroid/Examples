using System;
using System.IO;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.GIF
{
    [Framework.Core.Engine.Recorder(typeof(GIFRecorderSettings),"Video", "UTJ/GIF" )]
    public class GIFRecorder : Framework.Core.Engine.GenericRecorder<GIFRecorderSettings>
    {
        UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcGifContext m_ctx;
        UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcStream m_stream;

        public override bool BeginRecording(Framework.Core.Engine.RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }
            this.m_Settings.m_DestinationPath.CreateDirectory();

            return true;
        }

        public override void EndRecording(Framework.Core.Engine.RecordingSession session)
        {
            this.m_ctx.Release();
            this.m_stream.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(Framework.Core.Engine.RecordingSession session)
        {
            if (this.m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (Framework.Core.Engine.BaseRenderTextureInput)this.m_Inputs[0];
            var frame = input.outputRT;

            if(!this.m_ctx)
            {
                var settings = this.m_Settings.m_GifEncoderSettings;
                settings.width = frame.width;
                settings.height = frame.height;
                this.m_ctx = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcGifCreateContext(ref settings);
                var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, frame.width, frame.height, "gif");
                var path = Path.Combine( this.m_Settings.m_DestinationPath.GetFullPath(), fileName);
                this.m_stream = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcCreateFileStream(path);
                UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcGifAddOutputStream(this.m_ctx, this.m_stream);
            }

            UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcGifAddFramePixels(this.m_ctx, data, fmt, session.recorderTime);
            });
        }

    }
}
