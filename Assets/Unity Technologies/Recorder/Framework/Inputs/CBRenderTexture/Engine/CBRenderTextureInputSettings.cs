using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine
{
    public class CBRenderTextureInputSettings : Core.Engine.ImageInputSettings
    {
        public Core.Engine.EImageSource source = Core.Engine.EImageSource.ActiveCameras;
        public string m_CameraTag;
        public bool m_FlipFinalOutput = false;
        public bool m_AllowTransparency = false;
        public bool m_CaptureUI = false;

        public override Type inputType
        {
            get { return typeof(CBRenderTextureInput); }
        }

        public override bool ValidityCheck( List<string> errors )
        {
            var ok = base.ValidityCheck(errors);
            if (this.source == Core.Engine.EImageSource.TaggedCamera && string.IsNullOrEmpty(this.m_CameraTag))
            {
                ok = false;
                errors.Add("Missing tag for camera selection");
            }

            return ok;
        }
    }
}
