using System.IO;
using UnityEngine;

namespace UnityEditor.Recorder
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
            var dummy = ScriptableObject.CreateInstance<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderVersion>();
            var path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(dummy);
            return path;
        }

        public static string GetFrameRecorderPath()
        {
            var dummy = ScriptableObject.CreateInstance<FRPackagerPaths>();
            var path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            Unity_Technologies.Recorder.Framework.Core.Engine.UnityHelpers.Destroy(dummy);

            path= path.Replace("/Packager/Editor/FRPackagerPaths.cs", "");
            return path.Substring(0, path.LastIndexOf("/"));
        }

    }
}