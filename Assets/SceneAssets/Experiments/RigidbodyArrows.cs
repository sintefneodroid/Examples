using UnityEngine;
using UnityEngine.Serialization;

namespace SceneAssets.Experiments {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class RigidbodyArrows : MonoBehaviour {
    [SerializeField] Transform arrowPrefab=null;
    [SerializeField] Rigidbody rb=null;
    [SerializeField] Transform velArrow=null;
    [SerializeField] Transform angArrow=null;

    static readonly int _color = Shader.PropertyToID("_Color");
    static readonly int _color2 = Shader.PropertyToID("_Color2");
    static readonly string _shader_name = "IgnoreZ";

    void Awake() {
      this.rb = this.GetComponent<Rigidbody>();

      var a_c = new Color(0, 1, 0, (float).5);
      var v_c = new Color(0, 0, 1, (float).5);

      var transform1 = this.transform;
      var position = transform1.position; // + Vector3.up+ Vector3.right;
      this.velArrow = Instantiate(this.arrowPrefab, position, Quaternion.identity, transform1);
      var vel_mat = this.velArrow.GetComponent<Renderer>().material;
      vel_mat.shader = Shader.Find(_shader_name);
      vel_mat.SetColor(_color, v_c);
      vel_mat.SetColor(_color2, v_c);

      this.angArrow = Instantiate(this.arrowPrefab, position, Quaternion.identity, transform1);
      var ang_mat = this.angArrow.GetComponent<Renderer>().material;
      ang_mat.shader = Shader.Find(_shader_name);
      ang_mat.SetColor(_color, a_c);
      ang_mat.SetColor(_color2, a_c);
    }

    void FixedUpdate() {
      var vel = this.rb.velocity;
      this.velArrow.LookAt(this.velArrow.position + vel.normalized);
      var v_scale = this.velArrow.localScale;
      v_scale.z = vel.magnitude;
      this.velArrow.localScale = v_scale;

      var ang = this.rb.angularVelocity;
      this.angArrow.LookAt(this.angArrow.position + ang.normalized);
      var a_scale = this.angArrow.localScale;
      a_scale.z = ang.magnitude;
      this.angArrow.localScale = a_scale;
    }
  }
}
