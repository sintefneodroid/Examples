using UnityEngine;
using UnityEngine.Serialization;

namespace SceneAssets.Experiments
{
  public class RigidbodyArrows: MonoBehaviour
    {
      [SerializeField] Transform arrowPrefab;
      [SerializeField] Rigidbody rb;
      [SerializeField] Transform velArrow;
      [SerializeField] Transform angArrow;
      [SerializeField] float divisor = 1;

      static readonly int Color = Shader.PropertyToID("_Color");
      static readonly int Color2 = Shader.PropertyToID("_Color2");
      static readonly string shader_name = "IgnoreZ";

      void Awake()
      {
        this.rb = this.GetComponent<Rigidbody>();

        var aC = new Color(0, 1, 0, (float) .5);
        var vC = new Color(0, 0, 1, (float) .5);

        var transform1 = this.transform;
        var position = transform1.position;// + Vector3.up+ Vector3.left;
        this.velArrow = Instantiate(this.arrowPrefab, position, Quaternion.identity, transform1);
        var vel_mat = this.velArrow.GetComponent<Renderer>().material;
        vel_mat.shader = Shader.Find(shader_name);
        vel_mat.SetColor(Color, vC);
        vel_mat.SetColor(Color2, vC);

        this.angArrow = Instantiate(this.arrowPrefab, position, Quaternion.identity, transform1);
        var ang_mat =this.angArrow.GetComponent<Renderer>().material;
        ang_mat.shader = Shader.Find(shader_name);
        ang_mat.SetColor(Color, aC);
        ang_mat.SetColor(Color2, aC);
      }

      void FixedUpdate()
      {
        var vel = this.rb.velocity;
        this.velArrow.LookAt(this.velArrow.position + vel.normalized);
        var vScale = this.velArrow.localScale;
        vScale.z = vel.magnitude;
        this.velArrow.localScale = vScale;

        var ang = this.rb.angularVelocity;
        this.angArrow.LookAt(this.angArrow.position + ang.normalized);
        var aScale = this.angArrow.localScale;
        aScale.z = ang.magnitude;
        this.angArrow.localScale = aScale;

      }
    }
  }
