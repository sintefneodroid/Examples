namespace SceneAssets.Experiments {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class RigidbodyArrows : UnityEngine.MonoBehaviour {
    static readonly int _color = UnityEngine.Shader.PropertyToID("_Color");
    static readonly int _color2 = UnityEngine.Shader.PropertyToID("_Color2");
    static readonly string _shader_name = "IgnoreZ";
    [UnityEngine.SerializeField] UnityEngine.Transform arrowPrefab = null;
    [UnityEngine.SerializeField] UnityEngine.Rigidbody rb = null;
    [UnityEngine.SerializeField] UnityEngine.Transform velArrow = null;
    [UnityEngine.SerializeField] UnityEngine.Transform angArrow = null;

    void Awake() {
      this.rb = this.GetComponent<UnityEngine.Rigidbody>();

      var a_c = new UnityEngine.Color(0,
                                      1,
                                      0,
                                      a : (float).5);
      var v_c = new UnityEngine.Color(0,
                                      0,
                                      1,
                                      a : (float).5);

      var transform1 = this.transform;
      var position = transform1.position; // + Vector3.up+ Vector3.right;
      this.velArrow = Instantiate(original : this.arrowPrefab,
                                  position : position,
                                  rotation : UnityEngine.Quaternion.identity,
                                  parent : transform1);
      var vel_mat = this.velArrow.GetComponent<UnityEngine.Renderer>().material;
      vel_mat.shader = UnityEngine.Shader.Find(name : _shader_name);
      vel_mat.SetColor(nameID : _color, value : v_c);
      vel_mat.SetColor(nameID : _color2, value : v_c);

      this.angArrow = Instantiate(original : this.arrowPrefab,
                                  position : position,
                                  rotation : UnityEngine.Quaternion.identity,
                                  parent : transform1);
      var ang_mat = this.angArrow.GetComponent<UnityEngine.Renderer>().material;
      ang_mat.shader = UnityEngine.Shader.Find(name : _shader_name);
      ang_mat.SetColor(nameID : _color, value : a_c);
      ang_mat.SetColor(nameID : _color2, value : a_c);
    }

    void FixedUpdate() {
      var vel = this.rb.velocity;
      this.velArrow.LookAt(worldPosition : this.velArrow.position + vel.normalized);
      var v_scale = this.velArrow.localScale;
      v_scale.z = vel.magnitude;
      this.velArrow.localScale = v_scale;

      var ang = this.rb.angularVelocity;
      this.angArrow.LookAt(worldPosition : this.angArrow.position + ang.normalized);
      var a_scale = this.angArrow.localScale;
      a_scale.z = ang.magnitude;
      this.angArrow.localScale = a_scale;
    }
  }
}