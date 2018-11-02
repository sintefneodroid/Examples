using UnityEditor;
using UnityEngine;

namespace Common.ReferenceChecker.Editor {
  public class SettingsWindow : EditorWindow {
    const string _check_after_compilation_info = "Checks all build scenes whenever Unity finishes compiling.";
    bool _check_on_compilation;
    bool _colorful_logs;
    LogType _log_severity;

    [MenuItem("Window/UnityRefChecker")]
    public static void ShowWindow() { GetWindow(typeof(SettingsWindow)); }

    public void Awake() {
      this._check_on_compilation = Settings.GetCheckOnCompilation();
      this._log_severity = Settings.GetLogSeverity();
      this._colorful_logs = Settings.GetColorfulLogs();
    }

    void OnGUI() {
      GUILayout.Label("UnityRefChecker", EditorStyles.boldLabel);
      DrawDocumentationButton();

      GUILayout.Label("Commands", EditorStyles.boldLabel);
      DrawCommandButtons();

      GUILayout.Label("Settings", EditorStyles.boldLabel);
      this.DrawSettings();
    }

    static void DrawDocumentationButton() {
      if (GUILayout.Button("Documentation")) {
        Application.OpenURL("https://github.com/haydenjameslee/unityrefchecker");
      }
    }

    static void DrawCommandButtons() {
      if (GUILayout.Button("Check All Build Scenes")) {
        Commands.CheckBuildScenes();
      }

      if (GUILayout.Button("Check Open Scene")) {
        Commands.CheckOpenScene();
      }
    }

    void DrawSettings() {
      this.DrawCheckOnCompilationToggle();
      this.DrawLogSeverityPopup();
      this.DrawColorfulLogsToggle();
      this.DrawResetSettingsButton();
      this.DrawClearConsoleButton();
    }

    void DrawCheckOnCompilationToggle() {
      var toggle_value = EditorGUILayout.Toggle("Check after compilation", this._check_on_compilation);
      if (toggle_value) {
        EditorGUILayout.HelpBox(_check_after_compilation_info, MessageType.Info);
      }

      if (toggle_value != this._check_on_compilation) {
        Settings.SetCheckOnCompilation(toggle_value);
        this._check_on_compilation = toggle_value;
      }
    }

    void DrawLogSeverityPopup() {
      var selected_log_severity = (LogType)EditorGUILayout.EnumPopup("Log type", this._log_severity);
      if (selected_log_severity != this._log_severity) {
        Settings.SetLogSeverity(selected_log_severity);
        this._log_severity = selected_log_severity;
      }
    }

    void DrawColorfulLogsToggle() {
      var toggle_value = EditorGUILayout.Toggle("Colorful logs", this._colorful_logs);
      if (toggle_value != this._colorful_logs) {
        Settings.SetColorfulLogs(toggle_value);
        this._colorful_logs = toggle_value;
      }
    }

    void DrawResetSettingsButton() {
      if (GUILayout.Button("Reset Settings")) {
        Settings.ClearSettings();
        this.Close();
      }
    }

    void DrawClearConsoleButton() {
      if (GUILayout.Button("Clear Console")) {
        Commands.ClearConsole();
      }
    }
  }
}