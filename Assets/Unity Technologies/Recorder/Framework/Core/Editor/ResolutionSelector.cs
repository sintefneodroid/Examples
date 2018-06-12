using UnityEditor;

namespace Unity_Technologies.Recorder.Framework.Core.Editor
{
    public class ResolutionSelector
    {
        string[] m_MaskedNames;
        Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension m_MaxRes = Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window;

        public void OnInspectorGUI(Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension max, SerializedProperty size )
        {
            if (this.m_MaskedNames == null || max != this.m_MaxRes)
            {
                this.m_MaskedNames = EnumHelper.ClipOutEnumNames<Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension>((int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window, (int)max);
                this.m_MaxRes = max;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var index = EnumHelper.GetClippedIndexFromEnumValue<Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension>(size.intValue, (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window, (int)this.m_MaxRes);
                index = EditorGUILayout.Popup("Output Resolution", index, this.m_MaskedNames);

                if (check.changed)
                    size.intValue = EnumHelper.GetEnumValueFromClippedIndex<Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension>(index, (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window, (int)this.m_MaxRes);

                if (size.intValue > (int)this.m_MaxRes)
                    size.intValue = (int)this.m_MaxRes;
            }
        }
    }
}