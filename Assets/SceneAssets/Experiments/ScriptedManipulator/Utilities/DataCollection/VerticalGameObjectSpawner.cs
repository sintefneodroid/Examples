namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <summary>
  /// </summary>
  public class VerticalGameObjectSpawner : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] UnityEngine.GameObject _game_object = null;
    [UnityEngine.SerializeField] int _spawn_count = 10;

    void Start() {
      SpawnGameObjectsVertically(game_object : this._game_object,
                                 at_transform : this.transform,
                                 count : this._spawn_count);
    }

    public static void SpawnGameObjectsVertically(UnityEngine.GameObject game_object,
                                                  UnityEngine.Transform at_transform,
                                                  int count,
                                                  float spacing = 0.5f) {
      var y = at_transform.position.y;
      var new_position =
          new UnityEngine.Vector3(x : at_transform.position.x, y : y, z : at_transform.position.z);
      for (var i = 0; i < count; i++) {
        new_position.y = y;
        var new_game_object = Instantiate(original : game_object,
                                          position : new_position,
                                          rotation : at_transform.rotation);
        new_game_object.name = new_game_object.name + i;
        y += spacing;
      }
    }
  }
}