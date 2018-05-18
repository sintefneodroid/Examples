using UnityEngine;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    /// <summary>
    /// What is it: helper templated class of Recorder that provides a getter for the settings fields that returns the exected type os settings.
    /// Motivation: Root class Recorder has a field for the settings but it's a nuisance to always have to cast it to the 
    ///             specialized type, when accessed from the specialized recorder class. 
    /// </summary>
    public abstract class GenericRecorder<TSettings> : Recorder where TSettings : RecorderSettings
    {
        [SerializeField]
        protected TSettings m_Settings;
        public override RecorderSettings settings
        {
            get { return this.m_Settings; }
            set { this.m_Settings = (TSettings)value; }
        }
    }
}
