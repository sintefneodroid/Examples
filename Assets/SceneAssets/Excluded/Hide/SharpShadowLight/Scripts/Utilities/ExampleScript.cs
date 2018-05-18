using UnityEngine;

namespace SceneAssets.Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <summary>
  /// 
  /// </summary>
  public class ExampleScript : MonoBehaviour {
    [HideInInspector] public static int _Vertex_Count;
    int _layer;
    Lights.SharpShadowLight _2_ddl;
    bool _ctrl_down;

    bool _mouse_click;
    Camera _cam;

    Lights.SharpShadowLight _c_light;
    GameObject _cube_l;

    int _light_count = 1;

    void Start() {
      this._cam = FindObjectOfType<Camera>();

      this._c_light = FindObjectOfType<Lights.SharpShadowLight>();
    }

    void Update() {
      this._mouse_click = Input.GetMouseButtonDown(0);
      this._ctrl_down = Input.GetKey(KeyCode.LeftControl);
      
      var pos = this._c_light.transform.position;
      pos.x += Input.GetAxis("Horizontal") * 30f * Time.deltaTime;
      pos.y += Input.GetAxis("Vertical") * 30f * Time.deltaTime;

      if (this._mouse_click) {
        Vector2 p = this._cam.ScreenToWorldPoint(Input.mousePosition);

        if (this._ctrl_down) {
          this._2_ddl = this._c_light.GetComponent<Lights.SharpShadowLight>();
          this._layer = this._2_ddl._Layer;
          var m = new Material(this._2_ddl._Light_Material);

          var n_light = new GameObject();
          n_light.transform.parent = this._c_light.transform;

          this._2_ddl = n_light.AddComponent<Lights.SharpShadowLight>();
          //m.color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
          this._2_ddl._Light_Material = m;
          n_light.transform.position = p;
          this._2_ddl._Light_Radius = 40;
          this._2_ddl._Layer = this._layer;

          var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
          quad.transform.parent = n_light.transform;
          quad.transform.localPosition = Vector3.zero;
          this._light_count++;
        }
      }

      this._c_light.transform.position = pos;
    }

 
  }
}
