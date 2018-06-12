using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Unity_Technologies.Recorder.Framework.Core.Editor.Timeline
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Core.Engine.Timeline.RecorderClip), true)]
    public class RecorderClipEditor : UnityEditor.Editor
    {
        RecorderEditor m_Editor;
        TimelineAsset m_Timeline;
        RecorderSelector m_recorderSelector;

        public void OnEnable()
        {
            this.m_recorderSelector = null;
        }

        public override void OnInspectorGUI()
        {
            try
            {
                if (this.target == null)
                    return;

                // Bug? work arround: on Stop play, Enable is not called.
                if (this.m_Editor != null && this.m_Editor.target == null)
                {
                    Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
                    this.m_Editor = null;
                    this.m_recorderSelector = null;
                }

                if (this.m_recorderSelector == null)
                {
                    this.m_recorderSelector = new RecorderSelector(this.OnRecorderSelected, false);
                    this.m_recorderSelector.Init((this.target as Unity_Technologies.Recorder.Framework.Core.Engine.Timeline.RecorderClip).m_Settings);
                }

                this.m_recorderSelector.OnGui();

                if (this.m_Editor != null)
                {
                    this.m_Editor.showBounds = false;
                    this.m_Timeline = this.FindTimelineAsset();

                    this.PushTimelineIntoRecorder();

                    using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                    {
                        EditorGUILayout.Separator();

                        this.m_Editor.OnInspectorGUI();

                        EditorGUILayout.Separator();

                        this.PushRecorderIntoTimeline();

                        this.serializedObject.Update();
                    }
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                EditorGUILayout.HelpBox("An exception was raised while editing the settings. This can be indicative of corrupted settings.", MessageType.Warning);

                if (GUILayout.Button("Reset settings to default")) this.ResetSettings();

                Debug.LogException(ex);
            }
        }

        void ResetSettings()
        {
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
            this.m_Editor = null;
            this.m_recorderSelector = null;
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy((this.target as Unity_Technologies.Recorder.Framework.Core.Engine.Timeline.RecorderClip).m_Settings, true);
        }

        public void OnRecorderSelected()
        {
            var clip = this.target as Unity_Technologies.Recorder.Framework.Core.Engine.Timeline.RecorderClip;

            if (this.m_Editor != null)
            {
                Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
                this.m_Editor = null;
            }

            if (this.m_recorderSelector.selectedRecorder == null)
                return;

            if (clip.m_Settings != null && Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.GetRecorderInfo(this.m_recorderSelector.selectedRecorder).settingsClass != clip.m_Settings.GetType())
            {
                Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(clip.m_Settings, true);
                clip.m_Settings = null;
            }

            if(clip.m_Settings == null)
                clip.m_Settings = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.GenerateRecorderInitialSettings(clip, this.m_recorderSelector.selectedRecorder );
            this.m_Editor = CreateEditor(clip.m_Settings) as RecorderEditor;
            AssetDatabase.Refresh();
        }

        TimelineAsset FindTimelineAsset()
        {
            if (!AssetDatabase.Contains(this.target))
                return null;

            var path = AssetDatabase.GetAssetPath(this.target);
            var objs = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var obj in objs)
            {
                if (obj != null && AssetDatabase.IsMainAsset(obj))
                    return obj as TimelineAsset;
            }
            return null;
        }

        void PushTimelineIntoRecorder()
        {
            if (this.m_Timeline == null)
                return;

            var settings = this.m_Editor.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings;
            settings.m_DurationMode = Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.Manual;

            // Time
            settings.m_FrameRate = this.m_Timeline.editorSettings.fps;
        }

        void PushRecorderIntoTimeline()
        {
            if (this.m_Timeline == null)
                return;

            var settings = this.m_Editor.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings;
            settings.m_DurationMode = Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.Manual;

            // Time
            this.m_Timeline.editorSettings.fps = (float)settings.m_FrameRate;
        }
    }
}
