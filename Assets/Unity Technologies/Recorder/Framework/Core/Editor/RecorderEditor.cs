using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Recorder
{
    public enum EFieldDisplayState
    {
        Enabled,
        Disabled,
        Hidden
    }

    public abstract class RecorderEditor : Editor
    {

        protected class InputEditorState
        {
            InputEditor.IsFieldAvailableDelegate m_Validator;
            public bool visible;
            public InputEditor editor { get; private set; }

            Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting m_SettingsObj;

            public Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting settingsObj
            {
                get { return this.m_SettingsObj; }
                set
                {
                    this.m_SettingsObj = value;
                    if (this.editor != null)
                        Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(this.editor);

                    this.editor = CreateEditor(this.m_SettingsObj) as InputEditor;
                    if (this.editor is InputEditor)
                        (this.editor as InputEditor).isFieldAvailableForHost = this.m_Validator;
                }
            }

            public InputEditorState(InputEditor.IsFieldAvailableDelegate validator, Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting settings)
            {
                this.m_Validator = validator;
                this.settingsObj = settings;
            }
        }

        protected List<InputEditorState> m_InputEditors;
        protected List<string> m_SettingsErrors = new List<string>();
        RTInputSelector m_RTInputSelector;

        SerializedProperty m_FrameRateMode;
        SerializedProperty m_FrameRate;
        SerializedProperty m_DurationMode;
        SerializedProperty m_StartFrame;
        SerializedProperty m_EndFrame;
        SerializedProperty m_StartTime;
        SerializedProperty m_EndTime;
        SerializedProperty m_SynchFrameRate;
        SerializedProperty m_CaptureEveryNthFrame;
        SerializedProperty m_FrameRateExact;
        SerializedProperty m_DestinationPath;
        SerializedProperty m_BaseFileName;


        string[] m_FrameRateLabels;

        protected virtual void OnEnable()
        {
            if (this.target != null)
            {
                this.m_InputEditors = new List<InputEditorState>();
                this.m_FrameRateLabels = EnumHelper.MaskOutEnumNames<Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate>(0xFFFF, (x) => Unity_Technologies.Recorder.Framework.Core.Engine.FrameRateHelper.ToLable((Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate)x));

                var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings>(this.serializedObject);
                this.m_FrameRateMode = pf.Find(x => x.m_FrameRateMode);
                this.m_FrameRate = pf.Find(x => x.m_FrameRate);
                this.m_DurationMode = pf.Find(x => x.m_DurationMode);
                this.m_StartFrame = pf.Find(x => x.m_StartFrame);
                this.m_EndFrame = pf.Find(x => x.m_EndFrame);
                this.m_StartTime = pf.Find(x => x.m_StartTime);
                this.m_EndTime = pf.Find(x => x.m_EndTime);
                this.m_SynchFrameRate = pf.Find(x => x.m_SynchFrameRate);
                this.m_CaptureEveryNthFrame = pf.Find(x => x.m_CaptureEveryNthFrame);
                this.m_FrameRateExact = pf.Find(x => x.m_FrameRateExact);
                this.m_DestinationPath = pf.Find(w => w.m_DestinationPath);
                this.m_BaseFileName = pf.Find(w => w.m_BaseFileName);

                this.m_RTInputSelector = new RTInputSelector(this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings);

                this.BuildInputEditors();
            }
        }

        void BuildInputEditors()
        {
            var rs = this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings;
            if (!rs.inputsSettings.hasBrokenBindings && rs.inputsSettings.Count == this.m_InputEditors.Count)
                return;

            if (rs.inputsSettings.hasBrokenBindings)
                rs.BindSceneInputSettings();

            foreach (var editor in this.m_InputEditors)
                Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(editor.editor);
            this.m_InputEditors.Clear();

            foreach (var input in rs.inputsSettings) this.m_InputEditors.Add(new InputEditorState(this.GetFieldDisplayState, input) { visible = true });
        }

        protected virtual void OnDisable() {}

        protected virtual void Awake() {}

        public bool ValidityCheck(List<string> errors)
        {
            return (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).ValidityCheck(errors)
                && (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).isPlatformSupported;
        }

        public bool showBounds { get; set; }

        bool m_FoldoutInput = true;
        bool m_FoldoutEncoder = true;
        bool m_FoldoutTime = true;
        bool m_FoldoutBounds = true;
        bool m_FoldoutOutput = true;

        protected virtual void OnGroupGui()
        {
            this.OnInputGroupGui();
            this.OnOutputGroupGui();
            this.OnEncodingGroupGui();
            this.OnFrameRateGroupGui();
            this.OnBoundsGroupGui();
            this.OnExtraGroupsGui();
        }

        public override void OnInspectorGUI()
        {
            if (this.target == null)
                return;

            this.BuildInputEditors();

            EditorGUI.BeginChangeCheck();
            this.serializedObject.Update();

            this.OnGroupGui();

            this.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndChangeCheck();

            (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).SelfAdjustSettings();

            this.OnValidateSettingsGUI();
        }

        public virtual void OnValidateSettingsGUI()
        {
            this.m_SettingsErrors.Clear();
            if (!(this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).ValidityCheck(this.m_SettingsErrors))
            {
                for (int i = 0; i < this.m_SettingsErrors.Count; i++)
                {
                    EditorGUILayout.HelpBox(this.m_SettingsErrors[i], MessageType.Warning);
                }
            }
        }

        protected void AddInputSettings(Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting inputSettings)
        {
            var inputs = (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).inputsSettings;
            inputs.Add(inputSettings);
            this.m_InputEditors.Add(new InputEditorState(this.GetFieldDisplayState, inputSettings) { visible = true });
        }

        public void ChangeInputSettings(int atIndex, Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting newSettings)
        {
            if (newSettings != null)
            {
                var inputs = (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).inputsSettings;
                inputs.ReplaceAt(atIndex, newSettings);
                this.m_InputEditors[atIndex].settingsObj = newSettings;
            }
            else if (this.m_InputEditors.Count == 0)
            {
                throw new Exception("Source removal not implemented");
            }
        }

        protected virtual void OnInputGui()
        {
            var inputs = (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).inputsSettings;

            bool multiInputs = inputs.Count > 1;
            for (int i = 0; i < inputs.Count; i++)
            {
                if (multiInputs)
                {
                    EditorGUI.indentLevel++;
                    this.m_InputEditors[i].visible = EditorGUILayout.Foldout(this.m_InputEditors[i].visible, this.m_InputEditors[i].settingsObj.m_DisplayName ?? "Input " + (i + 1));
                }

                if (this.m_InputEditors[i].visible) this.OnInputGui(i);

                if (multiInputs)
                    EditorGUI.indentLevel--;
            }
        }

        protected virtual void OnInputGui(int inputIndex)
        {
            var inputs = (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings).inputsSettings;
            var input = inputs[inputIndex];
            if (this.m_RTInputSelector.OnInputGui(inputIndex, ref input)) this.ChangeInputSettings(inputIndex, input);

            this.m_InputEditors[inputIndex].editor.OnInspectorGUI();
            this.m_InputEditors[inputIndex].editor.OnValidateSettingsGUI();
        }

        protected virtual void OnOutputGui()
        {
            this.AddProperty(this.m_DestinationPath, () => { EditorGUILayout.PropertyField(this.m_DestinationPath, new GUIContent("Output path")); });
            this.AddProperty(this.m_BaseFileName, () => { EditorGUILayout.PropertyField(this.m_BaseFileName, new GUIContent("File name")); });
            this.AddProperty(this.m_CaptureEveryNthFrame, () => EditorGUILayout.PropertyField(this.m_CaptureEveryNthFrame, new GUIContent("Every n'th frame")));
        }

        protected virtual void OnEncodingGui()
        {
            // place holder
        }

        protected virtual void OnFrameRateGui()
        {
            this.AddProperty(this.m_FrameRateMode, () => EditorGUILayout.PropertyField(this.m_FrameRateMode, new GUIContent("Constraint Type")));

            this.AddProperty(
                    this.m_FrameRateExact, () =>
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var label = this.m_FrameRateMode.intValue == (int)Unity_Technologies.Recorder.Framework.Core.Engine.FrameRateMode.Constant ? "Target fps" : "Max fps";
                    var index = EnumHelper.GetMaskedIndexFromEnumValue<Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate>(this.m_FrameRateExact.intValue, 0xFFFF);
                    index = EditorGUILayout.Popup(label, index, this.m_FrameRateLabels);

                    if (check.changed)
                    {
                        this.m_FrameRateExact.intValue = EnumHelper.GetEnumValueFromMaskedIndex<Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate>(index, 0xFFFF);
                        if (this.m_FrameRateExact.intValue != (int)Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate.FR_CUSTOM) this.m_FrameRate.floatValue = Unity_Technologies.Recorder.Framework.Core.Engine.FrameRateHelper.ToFloat((Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate)this.m_FrameRateExact.intValue, this.m_FrameRate.floatValue);
                    }
                }
            });

            this.AddProperty(
                    this.m_FrameRate, () =>
            {
                if (this.m_FrameRateExact.intValue == (int)Unity_Technologies.Recorder.Framework.Core.Engine.EFrameRate.FR_CUSTOM)
                {
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.PropertyField(this.m_FrameRate, new GUIContent("Value"));
                    --EditorGUI.indentLevel;
                }
            });

            this.AddProperty(
                    this.m_FrameRateMode, () =>
            {
                if (this.m_FrameRateMode.intValue == (int)Unity_Technologies.Recorder.Framework.Core.Engine.FrameRateMode.Constant)
                    EditorGUILayout.PropertyField(this.m_SynchFrameRate, new GUIContent("Sync. framerate"));
            });
        }

        protected virtual void OnBoundsGui()
        {
            EditorGUILayout.PropertyField(this.m_DurationMode, new GUIContent("Mode"));

            ++EditorGUI.indentLevel;
            switch ((Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode)this.m_DurationMode.intValue)
            {
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.Manual:
                {
                    break;
                }
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.SingleFrame:
                {
                    this.AddProperty(
                            this.m_StartFrame, () =>
                    {
                        EditorGUILayout.PropertyField(this.m_StartFrame, new GUIContent("Frame #"));
                        this.m_EndFrame.intValue = this.m_StartFrame.intValue;
                    });
                    break;
                }
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.FrameInterval:
                {
                    this.AddProperty(this.m_StartFrame, () => EditorGUILayout.PropertyField(this.m_StartFrame, new GUIContent("First frame")));
                    this.AddProperty(this.m_EndFrame, () => EditorGUILayout.PropertyField(this.m_EndFrame, new GUIContent("Last frame")));
                    break;
                }
                case Unity_Technologies.Recorder.Framework.Core.Engine.DurationMode.TimeInterval:
                {
                    this.AddProperty(this.m_StartTime, () => EditorGUILayout.PropertyField(this.m_StartTime, new GUIContent("Start (sec)")));
                    this.AddProperty(this.m_EndFrame, () => EditorGUILayout.PropertyField(this.m_EndTime, new GUIContent("End (sec)")));
                    break;
                }
            }
            --EditorGUI.indentLevel;
        }

        protected virtual void OnInputGroupGui()
        {
            this.m_FoldoutInput = EditorGUILayout.Foldout(this.m_FoldoutInput, "Input(s)");
            if (this.m_FoldoutInput)
            {
                ++EditorGUI.indentLevel;
                this.OnInputGui();
                --EditorGUI.indentLevel;
            }
        }

        protected virtual void OnOutputGroupGui()
        {
            this.m_FoldoutOutput = EditorGUILayout.Foldout(this.m_FoldoutOutput, "Output(s)");
            if (this.m_FoldoutOutput)
            {
                ++EditorGUI.indentLevel;
                this.OnOutputGui();
                --EditorGUI.indentLevel;
            }
        }

        protected virtual void OnEncodingGroupGui()
        {
            this.m_FoldoutEncoder = EditorGUILayout.Foldout(this.m_FoldoutEncoder, "Encoding");
            if (this.m_FoldoutEncoder)
            {
                ++EditorGUI.indentLevel;
                this.OnEncodingGui();
                --EditorGUI.indentLevel;
            }
        }

        protected virtual void OnFrameRateGroupGui()
        {
            this.m_FoldoutTime = EditorGUILayout.Foldout(this.m_FoldoutTime, "Frame rate");
            if (this.m_FoldoutTime)
            {
                ++EditorGUI.indentLevel;
                this.OnFrameRateGui();
                --EditorGUI.indentLevel;
            }
        }

        protected virtual void OnBoundsGroupGui()
        {
            if (this.showBounds)
            {
                this.m_FoldoutBounds = EditorGUILayout.Foldout(this.m_FoldoutBounds, "Bounds / Limits");
                if (this.m_FoldoutBounds)
                {
                    ++EditorGUI.indentLevel;
                    this.OnBoundsGui();
                    --EditorGUI.indentLevel;
                }
            }
        }

        protected virtual void OnExtraGroupsGui()
        {
            // nothing. this is for sub classes...
        }

        protected virtual EFieldDisplayState GetFieldDisplayState(SerializedProperty property)
        {
            return EFieldDisplayState.Enabled;
        }

        protected void AddProperty(SerializedProperty prop, Action action)
        {
            var state = this.GetFieldDisplayState(prop);
            if (state != EFieldDisplayState.Hidden)
            {
                using (new EditorGUI.DisabledScope(state == EFieldDisplayState.Disabled))
                    action();
            }
        }


    }
}

