using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Misc;

#if UNITY_EDITOR

#endif


namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts
{
    [ExecuteInEditMode]
    public abstract class RecorderBase : MonoBehaviour
    {
        public enum ResolutionUnit
        {
            Percent,
            Pixels,
        }

        public enum FrameRateMode
        {
            Variable,
            Constant,
        }

        public enum CaptureControl
        {
            Manual,
            FrameRange,
            TimeRange,
        }


        [SerializeField] protected DataPath m_outputDir = new DataPath(DataPath.Root.Current, "Capture");

        [SerializeField] protected ResolutionUnit m_resolution = ResolutionUnit.Percent;
        [SerializeField] [Range(1,100)] protected int m_resolutionPercent = 100;
        [SerializeField] protected int m_resolutionWidth = 1920;

        [SerializeField] protected FrameRateMode m_framerateMode = FrameRateMode.Constant;
        [SerializeField] protected int m_targetFramerate = 30;
        [SerializeField] protected bool m_fixDeltaTime = true;
        [SerializeField] protected bool m_waitDeltaTime = true;
        [SerializeField] [Range(1,10)]protected int m_captureEveryNthFrame = 1;

        [SerializeField] protected CaptureControl m_captureControl = CaptureControl.FrameRange;
        [SerializeField] protected int m_startFrame = 0;
        [SerializeField] protected int m_endFrame = 100;
        [SerializeField] protected float m_startTime = 0.0f;
        [SerializeField] protected float m_endTime = 10.0f;
        [SerializeField] bool m_recordOnStart = false;

        protected bool m_recording = false;
        protected bool m_aborted = false;
        protected int m_initialFrame = 0;
        protected float m_initialTime = 0.0f;
        protected float m_initialRealTime = 0.0f;
        protected int m_frame = 0;
        protected int m_recordedFrames = 0;
        protected int m_recordedSamples = 0;


        public DataPath outputDir
        {
            get { return this.m_outputDir; }
            set { this.m_outputDir = value; }
        }

        public ResolutionUnit resolutionUnit
        {
            get { return this.m_resolution; }
            set { this.m_resolution = value; }
        }
        public int resolutionPercent
        {
            get { return this.m_resolutionPercent; }
            set { this.m_resolutionPercent = value; }
        }
        public int resolutionWidth
        {
            get { return this.m_resolutionWidth; }
            set { this.m_resolutionWidth = value; }
        }

        public FrameRateMode framerateMode
        {
            get { return this.m_framerateMode; }
            set { this.m_framerateMode = value; }
        }
        public int targetFramerate
        {
            get { return this.m_targetFramerate; }
            set { this.m_targetFramerate = value; }
        }
        public bool fixDeltaTime
        {
            get { return this.m_fixDeltaTime; }
            set { this.m_fixDeltaTime = value; }
        }
        public bool waitDeltaTime
        {
            get { return this.m_waitDeltaTime; }
            set { this.m_waitDeltaTime = value; }
        }
        public int captureEveryNthFrame
        {
            get { return this.m_captureEveryNthFrame; }
            set { this.m_captureEveryNthFrame = value; }
        }

        public CaptureControl captureControl
        {
            get { return this.m_captureControl; }
            set { this.m_captureControl = value; }
        }
        public int startFrame
        {
            get { return this.m_startFrame; }
            set { this.m_startFrame = value; }
        }
        public int endFrame
        {
            get { return this.m_endFrame; }
            set { this.m_endFrame = value; }
        }
        public float startTime
        {
            get { return this.m_startTime; }
            set { this.m_startTime = value; }
        }
        public float endTime
        {
            get { return this.m_endTime; }
            set { this.m_endTime = value; }
        }
        public bool isRecording
        {
            get { return this.m_recording; }
            set {
                if (value) { this.BeginRecording(); }
                else { this.EndRecording(); }
            }
        }
        public bool recordOnStart { set { this.m_recordOnStart = value; } }



        public virtual bool BeginRecording()
        {
            if(this.m_recording) { return false; }

            // delta time control
            if (this.m_framerateMode == FrameRateMode.Constant && this.m_fixDeltaTime)
            {
                Time.maximumDeltaTime = (1.0f / this.m_targetFramerate);
                if (!this.m_waitDeltaTime)
                {
                    Time.captureFramerate = this.m_targetFramerate;
                }
            }

            this.m_initialFrame = Time.renderedFrameCount;
            this.m_initialTime = Time.unscaledTime;
            this.m_initialRealTime = Time.realtimeSinceStartup;
            this.m_recordedFrames = 0;
            this.m_recordedSamples = 0;
            this.m_recording = true;
            return true;
        }

        public virtual void EndRecording()
        {
            if (!this.m_recording) { return; }

            if (this.m_framerateMode == FrameRateMode.Constant && this.m_fixDeltaTime)
            {
                if (!this.m_waitDeltaTime)
                {
                    Time.captureFramerate = 0;
                }
            }

            this.m_recording = false;
            this.m_aborted = true;
        }


        protected void GetCaptureResolution(ref int w, ref int h)
        {
            if(this.m_resolution == ResolutionUnit.Percent)
            {
                float scale = this.m_resolutionPercent * 0.01f;
                w = (int)(w * scale);
                h = (int)(h * scale);
            }
            else
            {
                float aspect = (float)h / w;
                w = this.m_resolutionWidth;
                h = (int)(this.m_resolutionWidth * aspect);
            }
        }

        protected IEnumerator Wait()
        {
            yield return new WaitForEndOfFrame();

            float wt = (1.0f / this.m_targetFramerate) * (Time.renderedFrameCount - this.m_initialFrame);
            while (Time.realtimeSinceStartup - this.m_initialRealTime < wt)
            {
                Thread.Sleep(1);
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            this.m_targetFramerate = Mathf.Max(1, this.m_targetFramerate);
            this.m_startFrame = Mathf.Max(0, this.m_startFrame);
            this.m_endFrame = Mathf.Max(this.m_startFrame, this.m_endFrame);
            this.m_startTime = Mathf.Max(0.0f, this.m_startTime);
            this.m_endTime = Mathf.Max(this.m_startTime, this.m_endTime);
        }
#endif // UNITY_EDITOR

        protected virtual void Start()
        {
            this.m_initialFrame = Time.renderedFrameCount;
            this.m_initialTime = Time.unscaledTime;
            this.m_initialRealTime = Time.realtimeSinceStartup;

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
#endif
            {
                if (this.m_recordOnStart)
                {
                    this.BeginRecording();
                }
            }
            this.m_recordOnStart = false;
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
#endif
            {
                this.EndRecording();
            }
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
#endif
            {
                if (this.m_captureControl == CaptureControl.FrameRange)
                {
                    if (!this.m_aborted && this.m_frame >= this.m_startFrame && this.m_frame <= this.m_endFrame)
                    {
                        if (!this.m_recording) { this.BeginRecording(); }
                    }
                    else if (this.m_recording)
                    {
                        this.EndRecording();
                    }
                }
                else if (this.m_captureControl == CaptureControl.TimeRange)
                {
                    float time = Time.unscaledTime - this.m_initialTime;
                    if (!this.m_aborted && time >= this.m_startTime && time <= this.m_endTime)
                    {
                        if (!this.m_recording) { this.BeginRecording(); }
                    }
                    else if (this.m_recording)
                    {
                        this.EndRecording();
                    }
                }
                else if (this.m_captureControl == CaptureControl.Manual)
                {
                }

                if(this.m_framerateMode == FrameRateMode.Constant && this.m_fixDeltaTime && this.m_waitDeltaTime)
                {
                    this.StartCoroutine(this.Wait());
                }
            }
        }

    }
}
