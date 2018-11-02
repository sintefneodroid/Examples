using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.ReferenceChecker.Editor {
  public static class Commands {
    static bool _was_error_in_check;
    static bool _running_after_compilation;

    [DidReloadScripts]
    static void RunAfterCompilation() {
      if (Settings.GetCheckOnCompilation()) {
        _running_after_compilation = true;
        CheckBuildScenes();
        _running_after_compilation = false;
      }
    }

    public static void CheckBuildScenes() {
      EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
      var previously_open_scene_path = SceneManager.GetActiveScene().path;

      var build_settings_scenes = EditorBuildSettings.scenes;
      for (var i = 0; i < build_settings_scenes.Length; i++) {
        var settings_scene = build_settings_scenes[i];
        var scene = GetSceneFromSettingsScene(settings_scene);
        CheckScene(scene);
      }

      EditorSceneManager.OpenScene(previously_open_scene_path);

      if (!_was_error_in_check && !_running_after_compilation) {
        Debug.Log("UnityRefChecker: All good!");
      }

      _was_error_in_check = false;
    }

    public static void CheckOpenScene() {
      EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
      var scene = SceneManager.GetActiveScene();
      CheckScene(scene);

      if (!_was_error_in_check && !_running_after_compilation) {
        Debug.Log("UnityRefChecker: All good!");
      }

      _was_error_in_check = false;
    }

    public static void ClearConsole() {
      var assembly = Assembly.GetAssembly(typeof(SceneView));
      var log_entries = assembly.GetType("UnityEditorInternal.LogEntries");
      var clear_console_method = log_entries.GetMethod("Clear");
      clear_console_method?.Invoke(new object(), null);
    }

    static Scene GetSceneFromSettingsScene(EditorBuildSettingsScene settings_scene) {
      var scene_path = settings_scene.path;
      EditorSceneManager.OpenScene(scene_path);
      var scene = SceneManager.GetSceneByPath(scene_path);
      return scene;
    }

    static void CheckScene(Scene scene) {
      var roots = scene.GetRootGameObjects();
      for (var i = 0; i < roots.Length; i++) {
        CheckRootGameObject(roots[i]);
      }
    }

    static void CheckRootGameObject(GameObject go) {
      var components = go.GetComponents<Component>();
      for (var i = 0; i < components.Length; i++) {
        CheckComponent(components[i]);
      }
    }

    static void CheckComponent(Component c) {
      // Ignore non-MonoBehaviours like Transform, Camera etc
      bool is_behaviour = c as MonoBehaviour;
      if (!is_behaviour) {
        return;
      }

      var comp_type = c.GetType();
      var field_types = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
      var fields = comp_type.GetFields(field_types);

      for (var i = 0; i < fields.Length; i++) {
        var info = fields[i];

        //Debug.Log("Field=" + info.Name + " type=" + info.MemberType);
        var should_print_log = ShouldPrintLogForComponent(c, info);

        if (should_print_log) {
          BuildAndPrintLog(c, info);

          if (!_was_error_in_check) {
            _was_error_in_check = true;
          }
        }
      }
    }

    static bool ShouldPrintLogForComponent(Component c, FieldInfo info) {
      var value = info.GetValue(c);
      var is_assigned = value != null;

      var has_ignore_attribute = FieldHasAttribute(info, typeof(IgnoreRefCheckerAttribute));

      var is_serializeable = info.IsPublic || FieldHasAttribute(info, typeof(SerializeField));
      var hidden_in_inspector = FieldHasAttribute(info, typeof(HideInInspector));

      var should_print_log =
          !is_assigned && !has_ignore_attribute && is_serializeable && !hidden_in_inspector;
      return should_print_log;
    }

    static bool FieldHasAttribute(FieldInfo info, Type attribute_type) {
      return info.GetCustomAttributes(attribute_type, true).Length > 0;
    }

    static string BuildLog(Component c, FieldInfo info) {
      var log = new ColorfulLogBuilder();
      var use_color = Settings.GetColorfulLogs();
      log.SetColorful(use_color);
      log.Append("UnityRefChecker: Component ");
      log.StartColor();
      log.Append(c.GetType().Name);
      log.EndColor();
      log.Append(" has a null reference for field ");
      log.StartColor();
      log.Append(info.Name);
      log.EndColor();
      log.Append(" on GameObject ");
      log.StartColor();
      log.Append(c.gameObject.name);
      log.EndColor();
      log.Append(" in Scene ");
      log.StartColor();
      log.Append(c.gameObject.scene.name);
      log.EndColor();
      return log.ToString();
    }

    static void BuildAndPrintLog(Component c, FieldInfo info) {
      var log = BuildLog(c, info);
      Debug.unityLogger.LogFormat(Settings.GetLogSeverity(), log);
    }
  }
}