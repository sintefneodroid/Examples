using System;
using System.IO;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine;

namespace Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine
{
    [Recorder(typeof(ImageRecorderSettings),"Video", "Unity/Image sequence" )]
    public class ImageRecorder : GenericRecorder<ImageRecorderSettings>
    {

        public override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            this.m_Settings.m_DestinationPath.CreateDirectory();

            return true;
        }

        public override void RecordFrame(RecordingSession session)
        {
            if (this.m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            Texture2D tex = null;
#if UNITY_2017_3_OR_NEWER
            if (this.m_Inputs[0] is ScreenCaptureInput)
            {
                tex = ((ScreenCaptureInput)this.m_Inputs[0]).image;
                if (this.m_Settings.m_OutputFormat == PNGRecordeOutputFormat.EXR)
                {
                    var textx = new Texture2D(tex.width, tex.height, TextureFormat.RGBAFloat, false);
                    textx.SetPixels(tex.GetPixels());
                    tex = textx;
                }
                else if (this.m_Settings.m_OutputFormat == PNGRecordeOutputFormat.PNG)
                {
                    var textx = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
                    textx.SetPixels(tex.GetPixels());
                    tex = textx;
                }
            }
            else
#endif
            {
                var input = (BaseRenderTextureInput)this.m_Inputs[0];
                var width = input.outputRT.width;
                var height = input.outputRT.height;
                tex = new Texture2D(width, height, this.m_Settings.m_OutputFormat != PNGRecordeOutputFormat.EXR ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat, false);
                var backupActive = RenderTexture.active;
                RenderTexture.active = input.outputRT;
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                tex.Apply();
                RenderTexture.active = backupActive;
            }

            byte[] bytes;
            string ext;
            switch (this.m_Settings.m_OutputFormat)
            {
                case PNGRecordeOutputFormat.PNG:
                    bytes = ImageConversion.EncodeToPNG(tex);
                    ext = "png";
                    break;
                case PNGRecordeOutputFormat.JPEG:
                    bytes = ImageConversion.EncodeToJPG(tex);
                    ext = "jpg";
                    break;
                case PNGRecordeOutputFormat.EXR:
                    bytes = ImageConversion.EncodeToEXR(tex);
                    ext = "exr";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(this.m_Inputs[0] is BaseRenderTextureInput || this.m_Settings.m_OutputFormat != PNGRecordeOutputFormat.JPEG)
                UnityHelpers.Destroy(tex);

            var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, tex.width, tex.height, ext);
            var path = Path.Combine( this.m_Settings.m_DestinationPath.GetFullPath(), fileName);

            File.WriteAllBytes( path, bytes);
        }
    }
}
