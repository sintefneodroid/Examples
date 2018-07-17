using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  public class VerticalGameObjectSpawner : MonoBehaviour {
    [SerializeField] int _spawn_count = 10;
    [SerializeField] GameObject _game_object;

    public void SpawnGameObjectsVertically(
        GameObject game_object,
        Transform at_tranform,
        int count,
        float spacing = 0.5f) {
      var y = at_tranform.position.y;
      var new_position = new Vector3(at_tranform.position.x, y, at_tranform.position.z);
      for (var i = 0; i < count; i++) {
        new_position.y = y;
        var new_game_object = Instantiate(game_object, new_position, at_tranform.rotation);
        new_game_object.name = new_game_object.name + i;
        y += spacing;
      }
    }

    void Start() { this.SpawnGameObjectsVertically(this._game_object, this.transform, this._spawn_count); }
  }
}
