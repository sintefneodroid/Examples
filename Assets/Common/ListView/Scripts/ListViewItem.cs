using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.ListView.Scripts {
  public class ListViewItem : ListViewItem<ListViewItemInspectorData> { }

  [RequireComponent(typeof(Collider))] public class ListViewItemBase : MonoBehaviour { }

  public class ListViewItem<TDataType> : ListViewItemBase where TDataType : ListViewItemData {
    public TDataType _Data;

    public virtual void Setup(TDataType data) {
      this._Data = data;
      data._Item = this;
    }
  }

  public class ListViewItemData {
    public MonoBehaviour _Item;
    public string _Template;
  }

  public class ListViewItemNestedData<TChildType> : ListViewItemData {
    public TChildType[] _Children;
    public bool _Expanded;
  }

  [Serializable] public class ListViewItemInspectorData : ListViewItemData { }

  public class ListViewItemTemplate {
    public readonly List<MonoBehaviour> _Pool = new List<MonoBehaviour>();
    public readonly GameObject _Prefab;

    public ListViewItemTemplate(GameObject prefab) {
      if (prefab == null) {
        Debug.LogError("Template prefab cannot be null");
      }

      this._Prefab = prefab;
    }
  }
}
