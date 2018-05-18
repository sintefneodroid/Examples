using System;
using System.IO;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.GIF
{
    [Recorder(typeof(GIFRecorderSettings),"Video", "UTJ/GIF" )]
    public class GIFRecorder : GenericRecorder<GIFRecorderSettings>
    {
        fcAPI.fcGifContext m_ctx;
        fcAPI.fcStream m_stream;

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }
            this.m_Settings.m_DestinationPath.CreateDirectory();

            return true;
        }

        public override void EndRecording(RecordingSession session)
        {
            this.m_ctx.Release();
            this.m_stream.Release();
            base.EndRecording(session);
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (this.m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (BaseRenderTextureInput)this.m_Inputs[0];
            var frame = input.outputRT;

            if(!this.m_ctx)
            {
                var settings = this.m_Settings.m_GifEncoderSettings;
                settings.width = frame.width;
                settings.height = frame.height;
                this.m_ctx = fcAPI.fcGifCreateContext(ref settings);
                var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, frame.width, frame.height, "gif");
                var path = Path.Combine( this.m_Settings.m_DestinationPath.GetFullPath(), fileName);
                this.m_stream = fcAPI.fcCreateFileStream(path);
                fcAPI.fcGifAddOutputStream(this.m_ctx, this.m_stream);
            }

            fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                fcAPI.fcGifAddFramePixels(this.m_ctx, data, fmt, session.recorderTime);
            });
        }

    }
}
