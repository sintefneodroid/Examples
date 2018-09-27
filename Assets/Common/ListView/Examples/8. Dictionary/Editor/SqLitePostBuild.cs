using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/*
namespace Excluded.ListView.Examples._8._Dictionary.Editor {
  public static class SqLitePostBuild {
    [PostProcessBuild(0)]
    public static void OnPostprocessBuild(BuildTarget target, string path_to_build_project) {
      switch (target) {
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
          path_to_build_project = Path.Combine(
              Path.GetDirectoryName(path_to_build_project),
              Path.GetFileNameWithoutExtension(path_to_build_project) + "_Data");
          Debug.Log(
              Path.Combine(Application.dataPath, DictionaryList._EditorDatabasePath)
              + ", "
              + Path.Combine(path_to_build_project, DictionaryList._DatabasePath));
          File.Copy(
              Path.Combine(Application.dataPath, DictionaryList._EditorDatabasePath),
              Path.Combine(path_to_build_project, DictionaryList._DatabasePath));
          break;
        case BuildTarget.StandaloneOSX:
          path_to_build_project = Path.Combine(
              Path.Combine(
                  Path.GetDirectoryName(path_to_build_project),
                  Path.GetFileNameWithoutExtension(path_to_build_project) + ".app"),
              "Contents");
          Debug.Log(
              Path.Combine(Application.dataPath, DictionaryList._EditorDatabasePath)
              + ", "
              + Path.Combine(path_to_build_project, DictionaryList._DatabasePath));
          File.Copy(
              Path.Combine(Application.dataPath, DictionaryList._EditorDatabasePath),
              Path.Combine(path_to_build_project, DictionaryList._DatabasePath));
          break;
      }
    }
  }
}*/
