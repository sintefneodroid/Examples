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
    public string _Template;
    public MonoBehaviour _Item;
  }

  public class ListViewItemNestedData<TChildType> : ListViewItemData {
    public bool _Expanded;
    public TChildType[] _Children;
  }

  [Serializable] public class ListViewItemInspectorData : ListViewItemData { }

  public class ListViewItemTemplate {
    public readonly GameObject _Prefab;
    public readonly List<MonoBehaviour> _Pool = new List<MonoBehaviour>();

    public ListViewItemTemplate(GameObject prefab) {
      if (prefab == null) {
        Debug.LogError("Template prefab cannot be null");
      }

      this._Prefab = prefab;
    }
  }
}
