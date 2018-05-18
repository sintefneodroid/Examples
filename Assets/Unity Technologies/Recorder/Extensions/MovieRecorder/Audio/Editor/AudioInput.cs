#if UNITY_2017_3_OR_NEWER
using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

#endif
#if UNITY_2018_1_OR_NEWER
    using Unity.Collections;
#else
    using UnityEngine.Collections;
#endif

namespace UnityEditor.Recorder.Input
{
    class AudioRenderer
    {
        private static MethodInfo m_StartMethod;
        private static MethodInfo m_StopMethod;
        private static MethodInfo m_GetSampleCountForCaptureFrameMethod;
        private static MethodInfo m_RenderMethod;

        static AudioRenderer()
        {
            var className = "UnityEngine.AudioRenderer";
            var dllName = "UnityEngine";
            var audioRecorderType = Type.GetType(className + ", " + dllName);
            if (audioRecorderType == null)
            {
                Debug.Log("AudioInput could not find " + className + " type in " + dllName);
                return;
            }
            m_StartMethod = audioRecorderType.GetMethod("Start");
            m_StopMethod = audioRecorderType.GetMethod("Stop");
            m_GetSampleCountForCaptureFrameMethod =
                audioRecorderType.GetMethod("GetSampleCountForCaptureFrame");
            m_RenderMethod = audioRecorderType.GetMethod("Render");
        }

        static public void Start()
        {
            m_StartMethod.Invoke(null, null);
        }

        static public void Stop()
        {
            m_StopMethod.Invoke(null, null);
        }

        static public uint GetSampleCountForCaptureFrame()
        {
            var count = (int)m_GetSampleCountForCaptureFrameMethod.Invoke(null, null);
            return (uint)count;
        }

        static public void Render(NativeArray<float> buffer)
        {
            m_RenderMethod.Invoke(null, new object[] { buffer });
        }
    }

    public class AudioInput : Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInput
    {
        private class BufferManager : IDisposable
        {
            private NativeArray<float>[] m_Buffers;

            public BufferManager(ushort bufferCount, uint sampleFrameCount, ushort channelCount)
            {
                this.m_Buffers = new NativeArray<float>[bufferCount];
                for (int i = 0; i < this.m_Buffers.Length; ++i) this.m_Buffers[i] = new NativeArray<float>((int)sampleFrameCount * (int)channelCount, Allocator.Temp);
            }

            public NativeArray<float> GetBuffer(int index)
            {
                return this.m_Buffers[index];
            }

            public void Dispose()
            {
                foreach (var a in this.m_Buffers)
                    a.Dispose();
            }
        }

        public ushort channelCount { get { return this.m_ChannelCount; } }
        private ushort m_ChannelCount;
        public int sampleRate { get { return AudioSettings.outputSampleRate; } }
        public NativeArray<float> mainBuffer { get { return this.m_BufferManager.GetBuffer(0); } }
        public NativeArray<float> GetMixerGroupBuffer(int n)
        { return this.m_BufferManager.GetBuffer(n + 1); }
        private BufferManager m_BufferManager;

        public AudioInputSettings audioSettings
        { get { return (AudioInputSettings)this.settings; } }

        public override void BeginRecording(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            this.m_ChannelCount = new Func<ushort>(() => {
                    switch (AudioSettings.speakerMode)
                    {
                    case AudioSpeakerMode.Mono:        return 1;
                    case AudioSpeakerMode.Stereo:      return 2;
                    case AudioSpeakerMode.Quad:        return 4;
                    case AudioSpeakerMode.Surround:    return 5;
                    case AudioSpeakerMode.Mode5point1: return 6;
                    case AudioSpeakerMode.Mode7point1: return 7;
                    case AudioSpeakerMode.Prologic:    return 2;
                    default: return 1;
                    }
            })();

            if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                Debug.Log(string.Format(
                              "AudioInput.BeginRecording for capture frame rate {0}", Time.captureFramerate));

            if (this.audioSettings.m_PreserveAudio)
                AudioRenderer.Start();
        }

        public override void NewFrameReady(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            if (!this.audioSettings.m_PreserveAudio)
                return;

            var sampleFrameCount = (uint)AudioRenderer.GetSampleCountForCaptureFrame();
            if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                Debug.Log(string.Format("AudioInput.NewFrameReady {0} audio sample frames @ {1} ch",
                                        sampleFrameCount,
                        this.m_ChannelCount));

            ushort bufferCount =
#if RECORD_AUDIO_MIXERS
                (ushort)(audioSettings.m_AudioMixerGroups.Length + 1)
#else
                1
#endif
            ;

            this.m_BufferManager = new BufferManager(bufferCount, sampleFrameCount, this.m_ChannelCount);
            var mainBuffer = this.m_BufferManager.GetBuffer(0);

#if RECORD_AUDIO_MIXERS
            for (int n = 1; n < bufferCount; n++)
            {
                var group = audioSettings.m_AudioMixerGroups[n - 1];
                if (group.m_MixerGroup == null)
                    continue;

                var buffer = m_BufferManager.GetBuffer(n);
                AudioRenderer.AddMixerGroupRecorder(group.m_MixerGroup, buffer, group.m_Isolate);
            }
#endif

            AudioRenderer.Render(mainBuffer);
        }

        public override void FrameDone(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            if (!this.audioSettings.m_PreserveAudio)
                return;

            this.m_BufferManager.Dispose();
            this.m_BufferManager = null;
        }

        public override void EndRecording(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            if (this.audioSettings.m_PreserveAudio)
                AudioRenderer.Stop();
        }
    }
}
#endif