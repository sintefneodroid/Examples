using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <summary>
  /// 
  /// </summary>
  public class VerticalGameObjectSpawner : MonoBehaviour {
    [SerializeField] GameObject _game_object;
    [SerializeField] int _spawn_count = 10;

    public static void SpawnGameObjectsVertically(GameObject game_object,
                                                  Transform at_transform,
                                                  int count,
                                                  float spacing = 0.5f) {
      var y = at_transform.position.y;
      var new_position = new Vector3(at_transform.position.x, y, at_transform.position.z);
      for (var i = 0; i < count; i++) {
        new_position.y = y;
        var new_game_object = Instantiate(game_object, new_position, at_transform.rotation);
        new_game_object.name = new_game_object.name + i;
        y += spacing;
      }
    }

    void Start() { SpawnGameObjectsVertically(this._game_object, this.transform, this._spawn_count); }
  }
}
