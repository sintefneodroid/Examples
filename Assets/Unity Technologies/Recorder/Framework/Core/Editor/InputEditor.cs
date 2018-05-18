using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.Recorder
{
    public abstract class InputEditor : Editor
    {
        protected List<string> m_SettingsErrors = new List<string>();

        public delegate EFieldDisplayState IsFieldAvailableDelegate(SerializedProperty property);

        public IsFieldAvailableDelegate isFieldAvailableForHost { get; set; }

        protected virtual void AddProperty(SerializedProperty prop, Action action)
        {
            var state = this.isFieldAvailableForHost == null ? EFieldDisplayState.Disabled : this.isFieldAvailableForHost(prop);

            if (state == EFieldDisplayState.Enabled)
                state = this.IsFieldAvailable(prop);
            if (state != EFieldDisplayState.Hidden)
            {
                using (new EditorGUI.DisabledScope(state == EFieldDisplayState.Disabled))
                    action();
            }
        }

        protected virtual EFieldDisplayState IsFieldAvailable(SerializedProperty property)
        {
            return EFieldDisplayState.Enabled;
        }

        public virtual void OnValidateSettingsGUI()
        {
            this.m_SettingsErrors.Clear();
            if (!(this.target as Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting).ValidityCheck(this.m_SettingsErrors))
            {
                for (int i = 0; i < this.m_SettingsErrors.Count; i++)
                {
                    EditorGUILayout.HelpBox(this.m_SettingsErrors[i], MessageType.Warning);
                }
            }
        }
    }
}
