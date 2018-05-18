using System;
using System.Collections.Generic;
using Unity_Technologies.Recorder.Framework.Core.Engine;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine
{
    public class RenderTextureInputSettings : ImageInputSettings
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
