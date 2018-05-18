#if UNITY_2017_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_2018_1_OR_NEWER
    using Unity.Collections;
#else
    using UnityEngine.Collections;
#endif
using UnityEditor;
using UnityEditor.Media;
using UnityEditor.Recorder.Input;

namespace UnityEditor.Recorder
{
#if RECORD_AUDIO_MIXERS
    class WavWriter
    {
        BinaryWriter binwriter;

        // Use this for initialization
        public void Start (string filename)
        {
            var stream = new FileStream (filename, FileMode.Create);
            binwriter = new BinaryWriter (stream);
            for(int n = 0; n < 44; n++)
                binwriter.Write ((byte)0);
        }

        public void Stop()
        {
            var closewriter = binwriter;
            binwriter = null;
            int subformat = 3; // float
            int numchannels = 2;
            int numbits = 32;
            int samplerate = AudioSettings.outputSampleRate;
            Debug.Log ("Closing file");
            long pos = closewriter.BaseStream.Length;
            closewriter.Seek (0, SeekOrigin.Begin);
            closewriter.Write ((byte)'R'); closewriter.Write ((byte)'I'); closewriter.Write ((byte)'F'); closewriter.Write ((byte)'F');
            closewriter.Write ((uint)(pos - 8));
            closewriter.Write ((byte)'W'); closewriter.Write ((byte)'A'); closewriter.Write ((byte)'V'); closewriter.Write ((byte)'E');
            closewriter.Write ((byte)'f'); closewriter.Write ((byte)'m'); closewriter.Write ((byte)'t'); closewriter.Write ((byte)' ');
            closewriter.Write ((uint)16);
            closewriter.Write ((ushort)subformat);
            closewriter.Write ((ushort)numchannels);
            closewriter.Write ((uint)samplerate);
            closewriter.Write ((uint)((samplerate * numchannels * numbits) / 8));
            closewriter.Write ((ushort)((numchannels * numbits) / 8));
            closewriter.Write ((ushort)numbits);
            closewriter.Write ((byte)'d'); closewriter.Write ((byte)'a'); closewriter.Write ((byte)'t'); closewriter.Write ((byte)'a');
            closewriter.Write ((uint)(pos - 36));
            closewriter.Seek ((int)pos, SeekOrigin.Begin);
            closewriter.Flush ();
        }

        public void Feed(NativeArray<float> data)
        {
            Debug.Log ("Writing wav chunk " + data.Length);

            if (binwriter == null)
                return;

            for(int n = 0; n < data.Length; n++)
                binwriter.Write (data[n]);
        }
    }
#endif

    [Unity_Technologies.Recorder.Framework.Core.Engine.Recorder(typeof(MediaRecorderSettings), "Video", "Unity/Movie")]
    public class MediaRecorder : Unity_Technologies.Recorder.Framework.Core.Engine.GenericRecorder<MediaRecorderSettings>
    {
        private MediaEncoder m_Encoder;
#if RECORD_AUDIO_MIXERS
        private WavWriter[]  m_WavWriters;
#endif
        private Texture2D m_ReadBackTexture;

        public override bool BeginRecording(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            if (!base.BeginRecording(session))
                return false;

            try
            {
                this.m_Settings.m_DestinationPath.CreateDirectory();
            }
            catch (Exception)
            {
                Debug.LogError(string.Format( "Movie recorder output directory \"{0}\" could not be created.", this.m_Settings.m_DestinationPath.GetFullPath()));
                return false;
            }

            int width;
            int height;
            if (this.m_Inputs[0] is Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInput)
            {
                var input = (Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInput)this.m_Inputs[0];
                width = input.outputWidth;
                height = input.outputHeight;
            }
            else
            {
                var input = (Unity_Technologies.Recorder.Framework.Core.Engine.BaseRenderTextureInput)this.m_Inputs[0];
                if (input == null)
                {
                    if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                        Debug.Log("MediaRecorder could not find input.");
                    return false;
                }
                width = input.outputWidth;
                height = input.outputHeight;
            }

            if (width <= 0 || height <= 0)
            {
                if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                    Debug.Log(string.Format(
                        "MovieRecorder got invalid input resolution {0} x {1}.", width, height));
                return false;
            }

            if (width > 4096 || height > 2160 && this.m_Settings.m_OutputFormat == MediaRecorderOutputFormat.MP4)
            {
                Debug.LogError("Mp4 format does not support requested resolution.");
            }

            var cbRenderTextureInput = this.m_Inputs[0] as Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInput;

            bool includeAlphaFromTexture = cbRenderTextureInput != null && cbRenderTextureInput.cbSettings.m_AllowTransparency;
            if (includeAlphaFromTexture && this.m_Settings.m_OutputFormat == MediaRecorderOutputFormat.MP4)
            {
                Debug.LogWarning("Mp4 format does not support alpha.");
                includeAlphaFromTexture = false;
            }

            var videoAttrs = new VideoTrackAttributes()
            {
                frameRate = RationalFromDouble(session.settings.m_FrameRate),
                width = (uint)width,
                height = (uint)height,
#if UNITY_2018_1_OR_NEWER
                includeAlpha = includeAlphaFromTexture,
                bitRateMode = (VideoBitrateMode)this.m_Settings.m_VideoBitRateMode
#else
                includeAlpha = includeAlphaFromTexture
#endif
            };

            if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                Debug.Log(
                    string.Format(
                        "MovieRecorder starting to write video {0}x{1}@[{2}/{3}] fps into {4}",
                        width, height, videoAttrs.frameRate.numerator,
                        videoAttrs.frameRate.denominator,
                        this.m_Settings.m_DestinationPath.GetFullPath()));

            var audioInput = (AudioInput)this.m_Inputs[1];
            var audioAttrsList = new List<AudioTrackAttributes>();
            var audioAttrs =
                new AudioTrackAttributes()
                {
                    sampleRate = new MediaRational
                    {
                        numerator = audioInput.sampleRate,
                        denominator = 1
                    },
                    channelCount = audioInput.channelCount,
                    language = ""
                };
            audioAttrsList.Add(audioAttrs);

            if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                Debug.Log( string.Format( "MovieRecorder starting to write audio {0}ch @ {1}Hz", audioAttrs.channelCount, audioAttrs.sampleRate.numerator));

#if RECORD_AUDIO_MIXERS
            var audioSettings = input.audioSettings;
            m_WavWriters = new WavWriter [audioSettings.m_AudioMixerGroups.Length];

            for (int n = 0; n < m_WavWriters.Length; n++)
            {
                if (audioSettings.m_AudioMixerGroups[n].m_MixerGroup == null)
                    continue;

                var path = Path.Combine(
                    m_Settings.m_DestinationPath,
                    "recording of " + audioSettings.m_AudioMixerGroups[n].m_MixerGroup.name + ".wav");
                if (Verbose.enabled)
                    Debug.Log("Starting wav recording into file " + path);
                m_WavWriters[n].Start(path);
            }
#endif

            try
            {
                var fileName = this.m_Settings.m_BaseFileName.BuildFileName( session, this.recordedFramesCount, width, height, this.m_Settings.m_OutputFormat.ToString().ToLower());
                var path = this.m_Settings.m_DestinationPath.GetFullPath() + "/" + fileName;

                this.m_Encoder = new MediaEncoder( path, videoAttrs, audioAttrsList.ToArray() );
                return true;
            }
            catch
            {
                if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
                    Debug.LogError("MovieRecorder unable to create MovieEncoder.");
            }

            return false;
        }

        public override void RecordFrame(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            if (this.m_Inputs.Count != 2)
                throw new Exception("Unsupported number of sources");

            int width;
            int height;
            if (this.m_Inputs[0] is Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInput)
            {
                var input = (Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInput)this.m_Inputs[0];
                width = input.outputWidth;
                height = input.outputHeight;
                this.m_Encoder.AddFrame(input.image);
            }
            else
            {
                var input = (Unity_Technologies.Recorder.Framework.Core.Engine.BaseRenderTextureInput)this.m_Inputs[0];
                width = input.outputWidth;
                height = input.outputHeight;

                if (!this.m_ReadBackTexture) this.m_ReadBackTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                var backupActive = RenderTexture.active;
                RenderTexture.active = input.outputRT;
                this.m_ReadBackTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                this.m_Encoder.AddFrame(this.m_ReadBackTexture);
                RenderTexture.active = backupActive;
            }

            var audioInput = (AudioInput)this.m_Inputs[1];
            if (!audioInput.audioSettings.m_PreserveAudio)
                return;

#if RECORD_AUDIO_MIXERS
            for (int n = 0; n < m_WavWriters.Length; n++)
                if (m_WavWriters[n] != null)
                    m_WavWriters[n].Feed(audioInput.mixerGroupAudioBuffer(n));
#endif

            this.m_Encoder.AddSamples(audioInput.mainBuffer);
        }

        public override void EndRecording(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            base.EndRecording(session);
            if (this.m_Encoder != null)
            {
                this.m_Encoder.Dispose();
                this.m_Encoder = null;
            }

            // When adding a file to Unity's assets directory, trigger a refresh so it is detected.
            if (this.m_Settings.m_DestinationPath.root == Unity_Technologies.Recorder.Framework.Core.Engine.OutputPath.ERoot.AssetsPath )
                AssetDatabase.Refresh();
        }

        // https://stackoverflow.com/questions/26643695/converting-decimal-to-fraction-c
        static long GreatestCommonDivisor(long a, long b)
        {
            if (a == 0)
                return b;

            if (b == 0)
                return a;

            return (a < b) ? GreatestCommonDivisor(a, b % a) : GreatestCommonDivisor(b, a % b);
        }

        static MediaRational RationalFromDouble(double value)
        {
            double integral = Math.Floor(value);
            double frac = value - integral;

            const long precision = 10000000;

            long gcd = GreatestCommonDivisor((long)Math.Round(frac * precision), precision);
            long denom = precision / gcd;

            return new MediaRational()
            {
                numerator = (int)((long)integral * denom + ((long)Math.Round(frac * (double)precision)) / gcd),
                denominator = (int)denom
            };
        }
    }
}
#endif