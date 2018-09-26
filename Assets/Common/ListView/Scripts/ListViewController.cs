using System.Collections.Generic;
using UnityEngine;

namespace Common.ListView.Scripts {
  public class ListViewController : ListViewController<ListViewItemInspectorData, ListViewItem> { }

  public abstract class ListViewControllerBase : MonoBehaviour {
    //Public variables
    [Tooltip("Distance (in meters) we have scrolled from initial position")]
    public float _ScrollOffset;

    [Tooltip("Padding (in meters) between items")]
    public float _Padding = 0.01f;

    [Tooltip("Width (in meters) of visible region")]
    public float _Range = 1;

    [Tooltip("Item temlate prefabs (at least one is required)")]
    public GameObject[] _Templates;

    //Protected variables
    protected int _M_DataOffset;
    protected int _M_NumItems;
    protected Vector3 _M_LeftSide;
    protected Vector3 _M_ItemSize;

    protected readonly Dictionary<string, ListViewItemTemplate> _M_Templates =
        new Dictionary<string, ListViewItemTemplate>();

    //Public properties
    public Vector3 ItemSize { get { return this._M_ItemSize; } }

    void Start() { this.Setup(); }

    void Update() { this.ViewUpdate(); }

    protected virtual void Setup() {
      if (this._Templates.Length < 1) {
        Debug.LogError("No templates!");
      }

      foreach (var template in this._Templates) {
        if (this._M_Templates.ContainsKey(template.name)) {
          Debug.LogError("Two templates cannot have the same name");
        }

        this._M_Templates[template.name] = new ListViewItemTemplate(template);
      }
    }

    protected virtual void ViewUpdate() {
      this.ComputeConditions();
      this.UpdateItems();
    }

    protected virtual void ComputeConditions() {
      if (this._Templates.Length > 0) {
        //Use first template to get item size
        this._M_ItemSize = this.GetObjectSize(this._Templates[0]);
      }

      //Resize range to nearest multiple of item width
      this._M_NumItems = Mathf.RoundToInt(this._Range / this._M_ItemSize.x); //Number of cards that will fit
      this._Range = this._M_NumItems * this._M_ItemSize.x;

      //Get initial conditions. This procedure is done every frame in case the collider bounds change at runtime
      this._M_LeftSide = this.transform.position + Vector3.left * this._Range * 0.5f;

      this._M_DataOffset = (int)(this._ScrollOffset / this.ItemSize.x);
      if (this._ScrollOffset < 0) {
        this._M_DataOffset--;
      }
    }

    protected abstract void UpdateItems();

    public virtual void ScrollNext() { this._ScrollOffset += this._M_ItemSize.x; }

    public virtual void ScrollPrev() { this._ScrollOffset -= this._M_ItemSize.x; }

    public virtual void ScrollTo(int index) { this._ScrollOffset = index * this.ItemSize.x; }

    protected virtual void Positioning(Transform t, int offset) {
      t.position = this._M_LeftSide + (offset * this._M_ItemSize.x + this._ScrollOffset) * Vector3.right;
    }

    protected virtual Vector3 GetObjectSize(GameObject g) {
      var item_size = Vector3.one;
      //TODO: Better method for finding object size
      var rend = g.GetComponentInChildren<Renderer>();
      if (rend) {
        item_size.x = Vector3.Scale(g.transform.lossyScale, rend.bounds.extents).x * 2 + this._Padding;
        item_size.y = Vector3.Scale(g.transform.lossyScale, rend.bounds.extents).y * 2 + this._Padding;
        item_size.z = Vector3.Scale(g.transform.lossyScale, rend.bounds.extents).z * 2 + this._Padding;
      }

      return item_size;
    }

    protected virtual void RecycleItem(string template, MonoBehaviour item) {
      if (item == null || template == null) {
        return;
      }

      this._M_Templates[template]._Pool.Add(item);
      item.gameObject.SetActive(false);
    }
  }

  public abstract class ListViewController<TDataType, TItemType> : ListViewControllerBase
      where TDataType : ListViewItemData where TItemType : ListViewItem<TDataType> {
    [Tooltip("Source Data")] public TDataType[] _Data;

    protected override void UpdateItems() {
      for (var i = 0; i < this._Data.Length; i++) {
        if (i + this._M_DataOffset < 0) {
          this.ExtremeLeft(this._Data[i]);
        } else if (i + this._M_DataOffset > this._M_NumItems) {
          this.ExtremeRight(this._Data[i]);
        } else {
          this.ListMiddle(this._Data[i], i);
        }
      }
    }

    protected virtual void ExtremeLeft(TDataType data) {
      this.RecycleItem(data._Template, data._Item);
      data._Item = null;
    }

    protected virtual void ExtremeRight(TDataType data) {
      this.RecycleItem(data._Template, data._Item);
      data._Item = null;
    }

    protected virtual void ListMiddle(TDataType data, int offset) {
      if (data._Item == null) {
        data._Item = this.GetItem(data);
      }

      this.Positioning(data._Item.transform, offset);
    }

    protected virtual TItemType GetItem(TDataType data) {
      if (data == null) {
        Debug.LogWarning("Tried to get item with null data");
        return null;
      }

      if (!this._M_Templates.ContainsKey(data._Template)) {
        Debug.LogWarning("Cannot get item, template " + data._Template + " doesn't exist");
        return null;
      }

      TItemType item = null;
      if (this._M_Templates[data._Template]._Pool.Count > 0) {
        item = (TItemType)this._M_Templates[data._Template]._Pool[0];
        this._M_Templates[data._Template]._Pool.RemoveAt(0);

        item.gameObject.SetActive(true);
        item.Setup(data);
      } else {
        item = Instantiate(this._M_Templates[data._Template]._Prefab).GetComponent<TItemType>();
        item.transform.parent = this.transform;
        item.Setup(data);
      }

      return item;
    }
  }
}
