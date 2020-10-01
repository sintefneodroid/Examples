﻿using System.IO;
using System.Text;
using SceneAssets.Experiments.ScriptedManipulator.Scripts;
using SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <summary>
  ///
  /// </summary>
  public class DataCollector : MonoBehaviour {
    [SerializeField] Camera[] _cameras = null;
    [SerializeField] int _current_episode_progress = 0;

    [SerializeField] int _episode_length = 100;

    // Sampling rate
    [SerializeField] string _file_path = @"training_data/";
    [SerializeField] string _file_path_gripper = @"gripper_position_rotation.csv";
    [SerializeField] string _file_path_target = @"target_position_rotation.csv";
    [SerializeField] ScriptedGrasping _grasping = null;

    [SerializeField] int _i = 0;
    [SerializeField] Grasp _target = null;

    void Start() {
      if (!this._grasping) {
        this._grasping = FindObjectOfType<ScriptedGrasping>();
      }

      if (!this._target) {
        this._target = FindObjectOfType<Grasp>();
      }

      //print ("GPU supports depth format: " + SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth));
      //print ("GPU supports shadowmap format: " + SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Shadowmap));

      File.WriteAllText(path : this._file_path + this._file_path_gripper, contents : "frame, x, y, z, rot_x, rot_y, rot_z\n");
      File.WriteAllText(path : this._file_path + this._file_path_target, contents : "frame, x, y, z, rot_x, rot_y, rot_z\n");

      /*if (!File.Exists(_file_path + _file_path_pos_rot)) {
      Debug.Log("Created file/path: " + _file_path + _file_path_pos_rot);
      //File.Create(_file_path + _file_path_pos_rot);
    }
    if (_deleteFileContent) {
      File.WriteAllText(_file_path + _file_path_pos_rot, "");
      _deleteFileContent = false;
    }*/
    }

    void LateUpdate() {
      if (this._current_episode_progress == this._episode_length - 1) {
        //Vector3 screenPoint = _depth_camera.WorldToViewportPoint (_target.transform.position);
        //if (screenPoint.z > 0 && screenPoint.x > 0.1 && screenPoint.x < 0.9 && screenPoint.y > 0.1 && screenPoint.y < 0.9) {
        var gripper_position_relative_to_camera =
            this.transform.InverseTransformPoint(position : this._grasping.transform.position);
        var gripper_direction_relative_to_camera =
            this.transform.InverseTransformDirection(direction : this._grasping.transform.eulerAngles);
        var gripper_transform_output =
            this.GetTransformOutput(id : this._i,
                                    pos : gripper_position_relative_to_camera,
                                    dir : gripper_direction_relative_to_camera);
        this.SaveToCsv(file_path : this._file_path + this._file_path_gripper, output : gripper_transform_output);

        var target_position_relative_to_camera =
            this.transform.InverseTransformPoint(position : this._target.transform.position);
        var target_direction_relative_to_camera =
            this.transform.InverseTransformDirection(direction : this._target.transform.eulerAngles);
        var target_transform_output =
            this.GetTransformOutput(id : this._i,
                                    pos : target_position_relative_to_camera,
                                    dir : target_direction_relative_to_camera);
        this.SaveToCsv(file_path : this._file_path + this._file_path_target, output : target_transform_output);

        foreach (var input_camera in this._cameras) {
          this.SaveRenderTextureToImage(id : this._i, input_camera : input_camera, file_name_dd : input_camera.name + "/");
        }

        this._i++;
        //}
        this._current_episode_progress = 0;
      }

      this._current_episode_progress++;
    }

    string[] GetTransformOutput(int id, Vector3 pos, Vector3 dir) {
      return new[] {
                       id.ToString(),
                       pos.x.ToString("0.000000"),
                       pos.y.ToString("0.000000"),
                       pos.z.ToString("0.000000"),
                       dir.x.ToString("0.000000"),
                       dir.y.ToString("0.000000"),
                       dir.z.ToString("0.000000")
                   };
    }

    void SaveBytesToFile(byte[] bytes, string filename) { File.WriteAllBytes(path : filename, bytes : bytes); }

    //Writes to file in the format: pos x, pos y, pos z, rot x, rot y, rot z
    void SaveToCsv(string file_path, string[] output) {
      var delimiter = ", ";

      var sb = new StringBuilder();

      sb.AppendLine(value : string.Join(separator : delimiter, value : output));

      File.AppendAllText(path : file_path, contents : sb.ToString());

      //using UnityEngine; using UnityEditor; using (StreamWriter sw = File.AppendText(filePath)) {
      //  sw.WriteLine(sb.ToString());
      //}
    }

    public void SaveRenderTextureToImage(int id, Camera input_camera, string file_name_dd) {
      var texture2_d = RenderTextureImage(input_camera : input_camera);
      var data = texture2_d.EncodeToPNG();
      var file_name = this._file_path + file_name_dd + id;
      //SaveTextureAsArray (camera, texture2d, file_name+".ssv");
      this.SaveBytesToFile(bytes : data, filename : file_name + ".png");
      //return data;
    }

    public static Texture2D RenderTextureImage(Camera input_camera) {
      // From unity documentation, https://docs.unity3d.com/ScriptReference/Camera.Render.html
      var current_render_texture = RenderTexture.active;
      RenderTexture.active = input_camera.targetTexture;
      input_camera.Render();
      var target_texture = input_camera.targetTexture;
      var image = new Texture2D(width : target_texture.width, height : target_texture.height);
      image.ReadPixels(source : new Rect(0,
                                         0,
                                         width : target_texture.width,
                                         height : target_texture.height),
                       destX : 0,
                       destY : 0);
      image.Apply();
      RenderTexture.active = current_render_texture;
      return image;
    }

    void SaveTextureAsArray(Camera input_camera, Texture2D image, string path) {
      var str_array = new string[image.height];
      var colors = image.GetPixels32();
      for (var iss = 0; iss < image.height; iss++) {
        var str = "";
        for (var jss = 0; jss < image.width; jss++) {
          str = str
                + (input_camera.nearClipPlane + colors[iss * image.width + jss].r * input_camera.farClipPlane)
                + " ";
        }

        str_array[iss] = str;
      }

      File.WriteAllLines(path : path, contents : str_array);
    }
  }
}
