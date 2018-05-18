using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Unity_Technologies.Recorder.Framework.Core.Engine.Timeline
{
    /// <summary>
    /// What is it: Implements a Timeline Clip asset that can be inserted onto a timeline track to trigger a recording of something.
    /// Motivation: Allow Timeline to trigger recordings
    /// 
    /// Note: Instances of this call Own their associated Settings asset's lifetime.
    /// </summary>
    [System.ComponentModel.DisplayName("Recorder Clip")]
    public class RecorderClip : PlayableAsset, ITimelineClipAsset
    {
        public delegate void RecordingClipDoneDelegate(RecorderClip clip);

        public static RecordingClipDoneDelegate OnClipDone;

        [UnityEngine.SerializeField]
        public RecorderSettings m_Settings;

        public Type recorderType
        {
            get { return this.m_Settings == null ? null : this.m_Settings.recorderType; }
        }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, UnityEngine.GameObject owner)
        {
            var playable = ScriptPlayable<RecorderPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            if (this.recorderType != null && UnityHelpers.IsPlaying())
            {
                behaviour.session = new RecordingSession()
                {
                    m_Recorder = RecordersInventory.GenerateNewRecorder(this.recorderType, this.m_Settings),
                    m_RecorderGO = SceneHook.HookupRecorder(),
                };
                behaviour.OnEnd = () =>
                {
                    try
                    {
                        if (OnClipDone != null) OnClipDone(this);     
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.Log("OnClipDone call back generated an exception: " + ex.Message );
                        UnityEngine.Debug.LogException(ex);
                    }
                };
            }
            return playable;
        }

        public virtual void OnDestroy()
        {
            UnityHelpers.Destroy( this.m_Settings, true );
        }
    }
}
