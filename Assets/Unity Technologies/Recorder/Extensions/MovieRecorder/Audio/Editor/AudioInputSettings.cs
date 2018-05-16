#if UNITY_2017_3_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace UnityEditor.Recorder.Input
{
    public class AudioInputSettings : Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting
    {
        public bool                         m_PreserveAudio = true;

#if RECORD_AUDIO_MIXERS
        [System.Serializable]
        public struct MixerGroupRecorderListItem
        {
            [SerializeField]
            public AudioMixerGroup m_MixerGroup;
            
            [SerializeField]
            public bool m_Isolate;
        }
        public MixerGroupRecorderListItem[] m_AudioMixerGroups;
#endif

        public override Type inputType
        {
            get { return typeof(AudioInput); }
        }

        public override bool ValidityCheck(List<string> errors)
        {
            return true;
        }
    }
}
#endif