using UnityEditor;
using Unity_Technologies.Recorder.Framework.Core.Editor;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.Editor
{
    public class RecorderEditorBase: RecorderEditor
    {
        public string m_BaseFileName;
        public string m_DestinationPath;

        [MenuItem("Tools/Recorder/Video")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }
    }
}
