#if UNITY_2017_3_OR_NEWER
using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine
{
    public class ScreenCaptureInputSettings : Core.Engine.ImageInputSettings
    {
        public override Type inputType
        {
            get { return typeof(ScreenCaptureInput); }
        }

        public override bool ValidityCheck( List<string> errors )
        {
            return true;
        }
    }
}

#endif