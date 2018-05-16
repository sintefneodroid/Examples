using System;
using System.IO;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.MP4
{
#if UNITY_2017_3_OR_NEWER
    [Obsolete("'UTJ/MP4' is obsolete, concider using 'Unity/Movie' instead", false)]
    [Framework.Core.Engine.Recorder(typeof(MP4RecorderSettings),"Video", "UTJ/Legacy/MP4" )]
#else
    [Recorder(typeof(MP4RecorderSettings),"Video", "UTJ/MP4" )]
#endif
    public class MP4Recorder : Framework.Core.Engine.GenericRecorder<MP4RecorderSettings>
    {
        UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcMP4Context m_ctx;

        public override bool BeginRecording(Framework.Core.Engine.RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            this.m_Settings.m_DestinationPath.CreateDirectory();

            var input = (Framework.Core.Engine.BaseRenderTextureInput)this.m_Inputs[0];
            if (input.outputWidth > 4096 || input.outputHeight > 2160 )
            {
                Debug.LogError("Mp4 format does not support requested resolution.");
            }

            return true;
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

            if(!this.m_ctx)
            {
                var settings = this.m_Settings.m_MP4EncoderSettings;
                settings.video = true;
                settings.audio = false;
                settings.videoWidth = frame.width;
                settings.videoHeight = frame.height;
                settings.videoTargetFramerate = (int)Math.Ceiling(this.m_Settings.m_FrameRate);
                if (this.m_Settings.m_AutoSelectBR)
                {
                    settings.videoTargetBitrate = (int)(( (frame.width * frame.height/1000.0) / 245 + 1.16) * (settings.videoTargetFramerate / 48.0 + 0.5) * 1000000);
                }
                var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, frame.width, frame.height, "mp4");
                var path = Path.Combine( this.m_Settings.m_DestinationPath.GetFullPath(), fileName);
                this.m_ctx = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcMP4OSCreateContext(ref settings, path);
            }

            UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcMP4AddVideoFramePixels(this.m_ctx, data, fmt, session.recorderTime);
            });
        }

    }
}
