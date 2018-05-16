using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine
{
    /// <summary>
    /// What is it: 
    /// Motivation: 
    /// </summary>
    public enum ESuperSamplingCount
    {
        x1 = 1,
        x2 = 2,
        x4 = 4,
        x8 = 8,
        x16 = 16,
    }

    public class RenderTextureSamplerSettings : Core.Engine.ImageInputSettings
    {
        public Core.Engine.EImageSource source = Core.Engine.EImageSource.ActiveCameras;
        public Core.Engine.EImageDimension m_RenderSize = Core.Engine.EImageDimension.x720p_HD;
        public ESuperSamplingCount m_SuperSampling = ESuperSamplingCount.x1;
        public float m_SuperKernelPower = 16f;
        public float m_SuperKernelScale = 1f;
        public string m_CameraTag;
        public UnityEngine.ColorSpace m_ColorSpace = UnityEngine.ColorSpace.Gamma;
        public bool m_FlipFinalOutput = false;

        public override Type inputType
        {
            get { return typeof(RenderTextureSampler); }
        }

        public override bool ValidityCheck( List<string> errors )
        {
            return true;
        }
    }
}