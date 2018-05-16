using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    public abstract class ImageInputSettings : RecorderInputSetting
    {
        public EImageDimension maxSupportedSize { get; set; } // dynamic & contextual: do not save
        public EImageDimension m_OutputSize = EImageDimension.x720p_HD;
        public EImageAspect m_AspectRatio = EImageAspect.x16_9;
        public bool m_ForceEvenSize = false;

        public override bool ValidityCheck( List<string> errors )
        {
            var ok = true;

            if (this.m_OutputSize > this.maxSupportedSize)
            {
                ok = false;
                errors.Add("Output size exceeds maximum supported size: " + (int)this.maxSupportedSize );
            }

            return ok;
        }
    }
}