using System;
using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._3._Custom_Item {
  public class CubeItem : ListViewItem<CubeItemData> {
    public TextMesh _Label;

    public override void Setup(CubeItemData data) {
      base.Setup(data);
      this._Label.text = data._Text;
    }
  }

  [Serializable]
  public class CubeItemData : ListViewItemData {
    public string _Text;
  }
}
