#if UNITY_2018_1_OR_NEWER

using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine
{

    public class Camera360InputSettings : Core.Engine.ImageInputSettings
    {
        public Core.Engine.EImageSource source = Core.Engine.EImageSource.MainCamera;
        public string m_CameraTag;
        public bool m_FlipFinalOutput = false;
        public bool m_RenderStereo = true;
        public float m_StereoSeparation = 0.065f;
        public int m_MapSize = 1024;
        public int m_OutputWidth = 1024;
        public int m_OutputHeight = 2048;

        public override Type inputType
        {
            get { return typeof(Camera360Input); }
        }

        public override bool ValidityCheck( List<string> errors )
        {
            bool ok = base.ValidityCheck(errors);

            if (this.source == Core.Engine.EImageSource.TaggedCamera && string.IsNullOrEmpty(this.m_CameraTag))
            {
                ok = false;
                errors.Add("Missing camera tag");
            }

            if (this.m_OutputWidth != (1 << (int)Math.Log(this.m_OutputWidth, 2)))
            {
                ok =false;
                errors.Add("Output width must be a power of 2.");
            }

            if (this.m_OutputWidth < 128 || this.m_OutputWidth > 8 * 1024)
            {
                ok = false;
                errors.Add( string.Format( "Output width must fall between {0} and {1}.", 128, 8*1024 ));
            }

            if (this.m_OutputHeight != (1 << (int)Math.Log(this.m_OutputHeight, 2)))
            {
                ok =false;
                errors.Add("Output height must be a power of 2.");
            }

            if (this.m_OutputHeight < 128 || this.m_OutputHeight > 8 * 1024)
            {
                ok = false;
                errors.Add( string.Format( "Output height must fall between {0} and {1}.", 128, 8*1024 ));
            }

            if (this.m_MapSize != (1 << (int)Math.Log(this.m_MapSize, 2)))
            {
                ok = false;
                errors.Add("Cube Map size must be a power of 2.");
            }

            if( this.m_MapSize < 16 || this.m_MapSize > 8 * 1024 )
            {
                ok = false;
                errors.Add( string.Format( "Cube Map size must fall between {0} and {1}.", 16, 8*1024 ));
            }

            if (this.m_RenderStereo && this.m_StereoSeparation < float.Epsilon)
            {
                ok = false;
                errors.Add("Stereo separation value is too small.");
            }

            return ok;
        }
    }

}

#endif