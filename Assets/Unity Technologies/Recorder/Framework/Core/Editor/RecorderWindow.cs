using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Packager.Editor;

namespace Unity_Technologies.Recorder.Framework.Core.Editor
{
    public class RecorderWindow : EditorWindow
    {
        enum EState
        {
            Idle,
            WaitingForPlayModeToStartRecording,
            Recording
        }

        RecorderEditor m_Editor;
        EState m_State = EState.Idle;

        RecorderSelector m_recorderSelector;
        string m_Category = string.Empty;

        RecorderWindowSettings m_WindowSettingsAsset;
        int m_FrameCount = 0;

        public static void ShowAndPreselectCategory(string category)
        {
            var window = GetWindow(typeof(RecorderWindow), false, "Recorder " + Unity_Technologies.Recorder.Framework.Core.Engine.RecorderVersion.Stage) as RecorderWindow;

            if (Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.recordersByCategory.ContainsKey(category))
            {
                window.m_Category = category;
                window.m_recorderSelector = null;
            }
        }

        public void OnEnable()
        {
            this.m_recorderSelector = null;
        }

        DateTime m_LastRepaint = DateTime.MinValue;

        protected void Update()
        {
            if (this.m_State == EState.Recording && (DateTime.Now - this.m_LastRepaint).TotalMilliseconds > 50)
            {
                this.Repaint();
            }
        }

        Vector2 m_ScrollPos;

        public void OnGUI()
        {
            try
            {
                this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos);
                try
                {
                    this.m_LastRepaint = DateTime.Now;

                    // Bug? work arround: on Stop play, Enable is not called.
                    if (this.m_Editor != null && this.m_Editor.target == null)
                    {
                        Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
                        this.m_Editor = null;
                        this.m_recorderSelector = null;
                    }

                    if (this.m_recorderSelector == null)
                    {
                        if (this.m_WindowSettingsAsset == null)
                        {
                            var candidates = AssetDatabase.FindAssets("t:RecorderWindowSettings");
                            if (candidates.Length > 0)
                            {
                                var path = AssetDatabase.GUIDToAssetPath(candidates[0]);
                                this.m_WindowSettingsAsset = AssetDatabase.LoadAssetAtPath<RecorderWindowSettings>(path);
                                if (this.m_WindowSettingsAsset == null)
                                {
                                    AssetDatabase.DeleteAsset(path);
                                }
                            }
                            if(this.m_WindowSettingsAsset == null)
                            {
                                this.m_WindowSettingsAsset = CreateInstance<RecorderWindowSettings>();
                                AssetDatabase.CreateAsset(this.m_WindowSettingsAsset, FRPackagerPaths.GetRecorderRootPath() +  "/RecorderWindowSettings.asset");
                                AssetDatabase.Refresh();
                            }
                        }

                        this.m_recorderSelector = new RecorderSelector(this.OnRecorderSelected, false);
                        this.m_recorderSelector.Init(this.m_WindowSettingsAsset.m_Settings, this.m_Category);
                    }

                    if (this.m_State == EState.WaitingForPlayModeToStartRecording && EditorApplication.isPlaying) this.DelayedStartRecording();

                    using (new EditorGUI.DisabledScope(EditorApplication.isPlaying)) this.m_recorderSelector.OnGui();

                    if (this.m_Editor != null)
                    {
                        this.m_Editor.showBounds = true;
                        using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
                        {
                            EditorGUILayout.Separator();
                            this.m_Editor.OnInspectorGUI();
                            EditorGUILayout.Separator();
                        }

                        this.RecordButtonOnGui();
                        GUILayout.Space(50);
                    }
                }
                finally
                {
                    EditorGUILayout.EndScrollView();
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                if (this.m_State == EState.Recording)
                {
                    try
                    {
                        Debug.LogError("Aborting recording due to an exception!\n" + ex.ToString());
                        this.StopRecording();
                    }
                    catch (Exception) {}
                }
                else
                {
                    EditorGUILayout.HelpBox("An exception was raised while editing the settings. This can be indicative of corrupted settings.", MessageType.Warning);

                    if (GUILayout.Button("Reset settings to default"))
                    {
                        this.ResetSettings();
                    }                    
                }
            }
        }

        void ResetSettings()
        {
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
            this.m_Editor = null;
            this.m_recorderSelector = null;
            var path = AssetDatabase.GetAssetPath(this.m_WindowSettingsAsset);
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_WindowSettingsAsset, true);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            this.m_WindowSettingsAsset = null;
        }

        public void OnDestroy()
        {
            this.StopRecording();
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
            this.m_Editor = null;
        }

        void RecordButtonOnGui()
        {
            if (this.m_Editor == null || this.m_Editor.target == null)
                return;

            switch (this.m_State)
            {
                case EState.Idle:
                {
                    var errors = new List<string>();
                    using (new EditorGUI.DisabledScope(!this.m_Editor.ValidityCheck(errors)))
                    {
                        if (GUILayout.Button("Start Recording")) this.StartRecording();
                    }
                    break;
                }
                case EState.WaitingForPlayModeToStartRecording:
                {
                    using (new EditorGUI.DisabledScope(Time.frameCount - this.m_FrameCount < 5))
                    {
                        if (GUILayout.Button("Stop Recording")) this.StopRecording();
                    }
                    break;
                }

                case EState.Recording:
                {
                    var recorderGO = Unity_Technologies.Recorder.Framework.Core.Engine.SceneHook.FindRecorder((Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings)this.m_Editor.target);
                    if (recorderGO == null)
                    {
                        GUILayout.Button("Start Recording"); // just to keep the ui system happy.
                        this.m_State = EState.Idle;
                        this.m_FrameCount = 0;
                    }
                    else
                    {
                        if (GUILayout.Button("Stop Recording")) this.StopRecording();
                        this.UpdateRecordingProgress(recorderGO);
                    }
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void UpdateRecordingProgress( GameObject go)
        {
            var rect = EditorGUILayout.BeginHorizontal(  );
            rect.height = 20;
            var recComp = go.GetComponent<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderComponent>();
            if (recComp == null || recComp.session == null)
                return;

            var session = recComp.session;
            var settings = recComp.session.m_Recorder.settings;
            switch (settings.m_DurationMode)
            {
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.Manual:
                {
                    var label = string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect, 0, label );

                    break;
                }
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.SingleFrame:
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.FrameInterval:
                {
                    var label = (session.frameIndex < settings.m_StartFrame) ? 
                            string.Format("Skipping first {0} frames...", settings.m_StartFrame-1) : 
                            string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect, (session.frameIndex +1) / (float)(settings.m_EndFrame +1), label );
                    break;
                }
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.TimeInterval:
                {
                    var label = (session.m_CurrentFrameStartTS < settings.m_StartTime) ?
                        string.Format("Skipping first {0} seconds...", settings.m_StartTime) :
                        string.Format("{0} Frames recorded", session.m_Recorder.recordedFramesCount);
                    EditorGUI.ProgressBar(rect,(float)session.m_CurrentFrameStartTS / (settings.m_EndTime == 0f ? 0.0001f : settings.m_EndTime), label );
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        void StartRecording()
        {
            this.m_State = EState.WaitingForPlayModeToStartRecording;
            Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.GameViewSize.DisableMaxOnPlay();
            EditorApplication.isPlaying = true;
            this.m_FrameCount = Time.frameCount;
            return;
        }

        void DelayedStartRecording()
        {
            this.StartRecording(true);
        }

        void StartRecording(bool autoExitPlayMode)
        {
            var settings = (Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings)this.m_Editor.target;
            var go = Unity_Technologies.Recorder.Framework.Core.Engine.SceneHook.HookupRecorder();
            var session = new Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession()
            {
                m_Recorder = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.GenerateNewRecorder(this.m_recorderSelector.selectedRecorder, settings),
                m_RecorderGO = go,
            };

            var component = go.AddComponent<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderComponent>();
            component.session = session;
            component.autoExitPlayMode = autoExitPlayMode;

            if (session.SessionCreated() && session.BeginRecording())
                this.m_State = EState.Recording;
            else
            {
                this.StopRecording();
            }
        }

        void StopRecording()
        {
            if (this.m_Editor != null)
            {
                var settings = (Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings)this.m_Editor.target;
                if (settings != null)
                {
                    var recorderGO = Unity_Technologies.Recorder.Framework.Core.Engine.SceneHook.FindRecorder(settings);
                    if (recorderGO != null)
                    {
                        Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(recorderGO);
                    }
                }
            }

            this.m_FrameCount = 0;
            this.m_State = EState.Idle;
        }

        public void OnRecorderSelected()
        {
            if (this.m_Editor != null)
            {
                Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_Editor);
                this.m_Editor = null;
            }

            if (this.m_recorderSelector.selectedRecorder == null)
                return;

            this.m_Category = this.m_recorderSelector.category;

            if (this.m_WindowSettingsAsset.m_Settings != null 
                && Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.GetRecorderInfo(this.m_recorderSelector.selectedRecorder).settingsClass != this.m_WindowSettingsAsset.m_Settings.GetType())
            {
                this.CleanupSettingsAsset();
            }

            if(this.m_WindowSettingsAsset.m_Settings == null ) this.m_WindowSettingsAsset.m_Settings = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.GenerateRecorderInitialSettings(this.m_WindowSettingsAsset, this.m_recorderSelector.selectedRecorder );
            this.m_Editor = UnityEditor.Editor.CreateEditor(this.m_WindowSettingsAsset.m_Settings ) as RecorderEditor;
            AssetDatabase.Refresh();

        }

        void CleanupSettingsAsset()
        {
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.m_WindowSettingsAsset, true);
            this.m_WindowSettingsAsset = CreateInstance<RecorderWindowSettings>();
            AssetDatabase.CreateAsset(this.m_WindowSettingsAsset, FRPackagerPaths.GetRecorderRootPath() +  "/RecorderWindowSettings.asset");
            AssetDatabase.Refresh();
        }


    }
}
