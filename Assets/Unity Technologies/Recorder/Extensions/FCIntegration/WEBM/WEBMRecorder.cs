using System;
using System.IO;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.WEBM
{
#if UNITY_2017_3_OR_NEWER
    [Obsolete("'UTJ/WEBM' is obsolete, concider using 'Unity/Movie' instead", false)]
    [Recorder(typeof(WEBMRecorderSettings),"Video", "UTJ/Legacy/WebM" )]
#else
    [Recorder(typeof(WEBMRecorderSettings),"Video", "UTJ/WebM" )]
#endif
    public class WEBMRecorder : GenericRecorder<WEBMRecorderSettings>
    {
        fcAPI.fcWebMContext m_ctx;
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

            if (!this.m_ctx)
            {
                var settings = this.m_Settings.m_WebmEncoderSettings;
                settings.video = true;
                settings.audio = false;
                settings.videoWidth = frame.width;
                settings.videoHeight = frame.height;
                if (this.m_Settings.m_AutoSelectBR)
                {
                    settings.videoTargetBitrate = (int)(( (frame.width * frame.height/1000.0) / 245 + 1.16) * (settings.videoTargetFramerate / 48.0 + 0.5) * 1000000);
                }

                settings.videoTargetFramerate = (int)Math.Ceiling(this.m_Settings.m_FrameRate);
                this.m_ctx = fcAPI.fcWebMCreateContext(ref settings);
                var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, settings.videoWidth, settings.videoHeight, "webm");
                var path = Path.Combine( this.m_Settings.m_DestinationPath.GetFullPath(), fileName);
                this.m_stream = fcAPI.fcCreateFileStream(path);
                fcAPI.fcWebMAddOutputStream(this.m_ctx, this.m_stream);
            }

            fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                fcAPI.fcWebMAddVideoFramePixels(this.m_ctx, data, fmt, session.recorderTime);
            });
        }

    }
}
