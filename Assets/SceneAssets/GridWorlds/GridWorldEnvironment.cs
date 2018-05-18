using System.Collections.Generic;
using System.Linq;
using droid.Neodroid.Environments;
using droid.Neodroid.Prototyping.Evaluation;
using droid.Neodroid.Prototyping.Observers;
using droid.Neodroid.Utilities.Structs;
using droid.Neodroid.Utilities.Unsorted;
using UnityEngine;

namespace SceneAssets.GridWorlds {
  /// <summary>
  /// 
  /// </summary>
  public enum MazeDirection {
    /// <summary>
    /// 
    /// </summary>
    North_,

    /// <summary>
    /// 
    /// </summary>
    East_,

    /// <summary>
    /// 
    /// </summary>
    South_,

    /// <summary>
    /// 
    /// </summary>
    West_,

    /// <summary>
    /// 
    /// </summary>
    Up_,

    /// <summary>
    /// 
    /// </summary>
    Down_
  }

  /// <summary>
  /// 
  /// </summary>
  public static class MazeDirections {
    /// <summary>
    /// 
    /// </summary>
    public const int _Count = 6;

    /// <summary>
    /// 
    /// </summary>
    static IntVector3[] _vectors = {
        new IntVector3(0, 0, 1),
        new IntVector3(1, 0, 0),
        new IntVector3(0, 0, -1),
        new IntVector3(-1, 0, 0),
        new IntVector3(0, 1, 0),
        new IntVector3(0, -1, 0)
    };

    /// <summary>
    /// 
    /// </summary>
    public static MazeDirection RandomValue { get { return (MazeDirection)Random.Range(0, _Count); } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static IntVector3 ToIntVector3(this MazeDirection direction) { return _vectors[(int)direction]; }
  }

  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [RequireComponent(typeof(GoalCellObserver))]
  public class GridWorldEnvironment : PrototypingEnvironment {
    [Range(0.0f, 0.999f)] [SerializeField] float _min_empty_cells_percentage = 0.5f;
    [SerializeField] Camera _camera;
    [SerializeField] Material _empty_cell_material;
    [SerializeField] Material _filled_cell_material;

    [SerializeField] Material _goal_cell_material;

    [SerializeField] GoalCellObserver _goal_cell_observer;
    [SerializeField] GridCell[,,] _grid;
    [SerializeField] IntVector3 _grid_size = new IntVector3(Vector3.one * 20);

    /// <summary>
    /// 
    /// </summary>
    IntVector3 RandomCoordinates {
      get {
        return new IntVector3(
            Random.Range(0, this._grid_size.x),
            Random.Range(0, this._grid_size.y),
            Random.Range(0, this._grid_size.z));
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xs"></param>
    /// <param name="ys"></param>
    /// <param name="zs"></param>
    /// <returns></returns>
    GridCell[,,] GenerateFullGrid(int xs, int ys, int zs) {
      var new_grid = new GridCell[xs, ys, zs];
      for (var i = 0; i < xs; i++) {
        for (var j = 0; j < ys; j++) {
          for (var k = 0; k < zs; k++) {
            new_grid[i, j, k] = this.CreateEmptyCell(i, j, k, xs, ys, zs);
          }
        }
      }

      return new_grid;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xs"></param>
    /// <param name="ys"></param>
    /// <param name="zs"></param>
    /// <param name="min_empty_cells_percentage"></param>
    /// <returns></returns>
    GridCell[,,] GenerateRandomGrid(int xs, int ys, int zs, float min_empty_cells_percentage = 0.4f) {
      var empty_cells_num = 0;
      var new_grid = new GridCell[xs, ys, zs];
      var total_cells = (float)(xs * ys * zs);
      var percentage_empty_cells = 0f;
      while (percentage_empty_cells <= min_empty_cells_percentage) {
        var c = this.RandomCoordinates;
        var active_cells = new List<GridCell>();
        this.DoFirstGenerationStep(
            ref empty_cells_num,
            ref new_grid,
            ref active_cells,
            c,
            xs,
            ys,
            zs); // does not count
        while (active_cells.Count > 0) {
          this.DoNextGenerationStep(ref empty_cells_num, ref new_grid, ref active_cells, xs, ys, zs);
        }

        percentage_empty_cells = empty_cells_num / total_cells;
      }

      for (var i = 0; i < xs; i++) {
        for (var j = 0; j < ys; j++) {
          for (var k = 0; k < zs; k++) {
            if (new_grid[i, j, k] == null) {
              var new_cell = this.CreateFilledCell(i, j, k, xs, ys, zs);
              new_grid[i, j, k] = new_cell;
            }
          }
        }
      }

      return new_grid;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="empty_cells_num"></param>
    /// <param name="grid"></param>
    /// <param name="active_cells"></param>
    /// <param name="c"></param>
    /// <param name="xs"></param>
    /// <param name="ys"></param>
    /// <param name="zs"></param>
    void DoFirstGenerationStep(
        ref int empty_cells_num,
        ref GridCell[,,] grid,
        ref List<GridCell> active_cells,
        IntVector3 c,
        int xs,
        int ys,
        int zs) {
      if (grid[c.x, c.y, c.z] == null) {
        grid[c.x, c.y, c.z] = this.CreateEmptyCell(c, xs, ys, zs);
        empty_cells_num += 1;
      }

      active_cells.Add(grid[c.x, c.y, c.z]);
    }

    void DoNextGenerationStep(
        ref int empty_cells_num,
        ref GridCell[,,] grid,
        ref List<GridCell> active_cells,
        int xs,
        int ys,
        int zs) {
      var current_index = active_cells.Count - 1;
      var current_cell = active_cells[current_index];
      var direction = MazeDirections.RandomValue;
      var c = current_cell.GridCoordinates + direction.ToIntVector3();

      if (this.ContainsCoordinates(c) && grid[c.x, c.y, c.z] == null) {
        grid[c.x, c.y, c.z] = this.CreateEmptyCell(c, xs, ys, zs);
        active_cells.Add(grid[c.x, c.y, c.z]);
        empty_cells_num += 1;
      } else {
        active_cells.RemoveAt(current_index);
      }
    }

    bool ContainsCoordinates(IntVector3 coordinate) {
      return coordinate.x >= 0
             && coordinate.x < this._grid_size.x
             && coordinate.y >= 0
             && coordinate.y < this._grid_size.y
             && coordinate.z >= 0
             && coordinate.z < this._grid_size.z;
    }

    GridCell CreateEmptyCell(IntVector3 c, IntVector3 size) {
      return this.CreateEmptyCell(c.x, c.y, c.z, size.x, size.y, size.z);
    }

    GridCell CreateEmptyCell(IntVector3 c, int xs, int ys, int zs) {
      return this.CreateEmptyCell(c.x, c.y, c.z, xs, ys, zs);
    }

    GridCell CreateEmptyCell(int x, int y, int z, int xs, int ys, int zs) {
      var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube.transform.parent = this.transform;
      cube.transform.localPosition = new Vector3(
          x - xs * 0.5f + 0.5f,
          y - ys * 0.5f + 0.5f,
          z - zs * 0.5f + 0.5f);
      var new_cell = cube.AddComponent<EmptyCell>();
      var n = $"EmptyCell{x}{y}{z}";
      new_cell.Setup(n, this._empty_cell_material);
      new_cell.GridCoordinates = new IntVector3(x, y, z);
      return new_cell;
    }

    GridCell CreateFilledCell(int x, int y, int z, int xs, int ys, int zs) {
      var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

      cube.transform.parent = this.transform;
      cube.transform.localPosition = new Vector3(
          x - xs * 0.5f + 0.5f,
          y - ys * 0.5f + 0.5f,
          z - zs * 0.5f + 0.5f);
      var new_cell = cube.AddComponent<FilledCell>();
      var n = $"FilledCell{x}{y}{z}";
      new_cell.Setup(n, this._filled_cell_material);
      new_cell.GridCoordinates = new IntVector3(x, y, z);
      return new_cell;
    }

    protected override void PreSetup() {
      base.PreSetup();

      var xs = this._grid_size.x;
      var ys = this._grid_size.y;
      var zs = this._grid_size.z;
      this._grid = this.GenerateRandomGrid(xs, ys, zs, this._min_empty_cells_percentage);

      this._goal_cell_observer = this.gameObject.GetComponent<GoalCellObserver>();

      this.Setup();

      var dominant_dimension = Mathf.Max(xs, ys, zs);
      this._camera.orthographicSize = dominant_dimension / 2f + 1f;
      this._camera.transform.position = new Vector3(0, ys / 2f + 1f, 0);
    }

    void NewGridWorld() {
      var xs = this._grid_size.x;
      var ys = this._grid_size.y;
      var zs = this._grid_size.z;

      for (var i = 0; i < xs; i++) {
        for (var j = 0; j < ys; j++) {
          for (var k = 0; k < zs; k++) {
            if (this._grid[i, j, k] != null) {
              DestroyImmediate(this._grid[i, j, k].gameObject);
            }
          }
        }
      }

      this._grid = this.GenerateRandomGrid(xs, ys, zs, this._min_empty_cells_percentage);
    }

    protected override void Setup() {
      var empty_cells = FindObjectsOfType<EmptyCell>().ToList();

      var objective_function = this.ObjectiveFunction as ReachGoal;

      foreach (var a in this.Actors) {
        var idx = Random.Range(0, empty_cells.Count);
        var empty_cell = empty_cells[idx];
        a.Value.transform.position = empty_cell.transform.position;
        empty_cells.RemoveAt(idx);
      }

      if (objective_function) {
        var idx = Random.Range(0, empty_cells.Count);
        var empty_cell = empty_cells[idx];
        empty_cell.SetAsGoal("Goal", this._goal_cell_material);
        this._goal_cell_observer.CurrentGoal = empty_cell;
        objective_function.SetGoal(empty_cell);
      }
    }

    public override void PostStep() {
      if (this._Terminated) {
        this._Terminated = false;

        this.NewGridWorld();

        this.Reset();

        this.Setup();

        if (this._Configure) {
          this._Configure = false;
          this.Configure();
        }
      }

      this.UpdateConfigurableValues();
      this.UpdateObserversData();
    }
  }
}
