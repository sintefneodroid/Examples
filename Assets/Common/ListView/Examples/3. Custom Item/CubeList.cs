using Common.ListView.Scripts;

namespace Common.ListView.Examples._3._Custom_Item {
  public class CubeList : ListViewController<CubeItemData, CubeItem> {
    protected override void Setup() {
      base.Setup();
      for (var i = 0; i < this._Data.Length; i++) {
        this._Data[i]._Text = i + "";
      }
    }
  }
}
