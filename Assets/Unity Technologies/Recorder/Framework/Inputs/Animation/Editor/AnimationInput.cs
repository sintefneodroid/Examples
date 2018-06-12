using UnityEditor.Experimental.Animations;

namespace Unity_Technologies.Recorder.Framework.Inputs.Animation.Editor
{
    public class AnimationInput : Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInput
    {
        public GameObjectRecorder m_gameObjectRecorder;
        private float m_time;

        public override void BeginRecording(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            var aniSettings = (this.settings as AnimationInputSettings);

            if (!aniSettings.enabled)
                return;

            var srcGO = aniSettings.gameObject;
#if UNITY_2018_1_OR_NEWER
            this.m_gameObjectRecorder = new GameObjectRecorder(srcGO);
#else
            m_gameObjectRecorder = new GameObjectRecorder {root = srcGO};
#endif
            foreach (var binding in aniSettings.bindingType)
            {
                this.m_gameObjectRecorder.BindComponentsOfType(srcGO, binding, aniSettings.recursive); 
            }

            this.m_time = session.recorderTime;
        }

        public override void NewFrameReady(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
            if (session.recording && (this.settings as AnimationInputSettings).enabled )
            {
                this.m_gameObjectRecorder.TakeSnapshot(session.recorderTime - this.m_time);
                this.m_time = session.recorderTime;
            }
        }

        
    }
}