using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine
{
    public class RenderTextureInputSettings : Core.Engine.ImageInputSettings
    {
        public UnityEngine.RenderTexture m_SourceRTxtr;

        public override Type inputType
        {
            get { return typeof(RenderTextureInput); }
        }

        public override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (this.m_SourceRTxtr == null)
            {
                ok = false;
                errors.Add("Missing source render texture object/asset.");
            }

            return ok;
        }

    }
}
