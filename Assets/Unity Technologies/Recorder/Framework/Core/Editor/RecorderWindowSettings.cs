using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// This is just a helper class that should disappear once we have a proper way of saving the recorder window settings...
    /// </summary>
    public class RecorderWindowSettings : ScriptableObject
    {
        public Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings m_Settings;
    }
}