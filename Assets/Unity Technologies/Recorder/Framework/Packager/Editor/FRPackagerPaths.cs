using UnityEditor;
using UnityEngine;

namespace Unity_Technologies.Recorder.Framework.Packager.Editor
{
    class FRPackagerPaths : ScriptableObject
    {
        public static string GetRecorderRootPath()
        {
            var path = GetFrameRecorderPath();
            path = path.Substring(path.IndexOf("Assets"));
            return path;
        }

        public static string GetRecorderVersionFilePath()
        {
            var dummy = CreateInstance<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderVersion>();
            var path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(dummy);
            return path;
        }

        public static string GetFrameRecorderPath()
        {
            var dummy = CreateInstance<FRPackagerPaths>();
            var path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(dummy);

            path= path.Replace("/Packager/Editor/FRPackagerPaths.cs", "");
            return path.Substring(0, path.LastIndexOf("/"));
        }

    }
}