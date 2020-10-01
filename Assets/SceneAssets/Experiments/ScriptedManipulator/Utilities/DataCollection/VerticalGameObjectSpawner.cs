using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <summary>
  /// 
  /// </summary>
  public class VerticalGameObjectSpawner : MonoBehaviour {
    [SerializeField] GameObject _game_object = null;
    [SerializeField] int _spawn_count = 10;

    public static void SpawnGameObjectsVertically(GameObject game_object,
                                                  Transform at_transform,
                                                  int count,
                                                  float spacing = 0.5f) {
      var y = at_transform.position.y;
      var new_position = new Vector3(x : at_transform.position.x, y : y, z : at_transform.position.z);
      for (var i = 0; i < count; i++) {
        new_position.y = y;
        var new_game_object = Instantiate(original : game_object, position : new_position, rotation : at_transform.rotation);
        new_game_object.name = new_game_object.name + i;
        y += spacing;
      }
    }

    void Start() { SpawnGameObjectsVertically(game_object : this._game_object, at_transform : this.transform, count : this._spawn_count); }
  }
}
