using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.Recorder
{
    public class ResolutionSelector
    {
        string[] m_MaskedNames;
        Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension m_MaxRes = Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window;

        public void OnInspectorGUI(Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension max, SerializedProperty size )
        {
            if (m_MaskedNames == null || max != m_MaxRes)
            {
                m_MaskedNames = EnumHelper.ClipOutEnumNames<Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension>((int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window, (int)max);
                m_MaxRes = max;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var index = EnumHelper.GetClippedIndexFromEnumValue<Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension>(size.intValue, (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window, (int)m_MaxRes);
                index = EditorGUILayout.Popup("Output Resolution", index, m_MaskedNames);

                if (check.changed)
                    size.intValue = EnumHelper.GetEnumValueFromClippedIndex<Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension>(index, (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window, (int)m_MaxRes);

                if (size.intValue > (int)m_MaxRes)
                    size.intValue = (int)m_MaxRes;
            }
        }
    }
}