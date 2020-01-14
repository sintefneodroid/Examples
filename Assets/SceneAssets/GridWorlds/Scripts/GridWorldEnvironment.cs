using System.Collections.Generic;
using System.Linq;
using droid.Runtime.Environments;
using droid.Runtime.Environments.Prototyping;
using droid.Runtime.Prototyping.ObjectiveFunctions.Spatial;
using droid.Runtime.Prototyping.Sensors.Spatial.Grid;
using droid.Runtime.Structs.Vectors;
using droid.Runtime.Utilities.Grid;
using UnityEngine;

namespace SceneAssets.GridWorlds.Scripts {
  /// <summary>
  /// </summary>
  public enum MazeDirection {
    /// <summary>
    /// </summary>
    North_,

    /// <summary>
    /// </summary>
    East_,

    /// <summary>
    /// </summary>
    South_,

    /// <summary>
    /// </summary>
    West_,

    /// <summary>
    /// </summary>
    Up_,

    /// <summary>
    /// </summary>
    Down_
  }

  /// <summary>
  /// </summary>
  public static class MazeDirections {
    /// <summary>
    /// </summary>
    public const int _Count = 6;

    /// <summary>
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
    /// </summary>

    public static MazeDirection RandomValue { get { return (MazeDirection)Random.Range(0, max : _Count); } }

    /// <summary>
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static IntVector3 ToIntVector3(this MazeDirection direction) { return _vectors[(int)direction]; }
  }

  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [RequireComponent(typeof(GoalCellSensor))]
  public class GridWorldEnvironment : ActorisedPrototypingEnvironment {
    [SerializeField] Camera _camera = null;
    [SerializeField] Material _empty_cell_material = null;
    [SerializeField] Material _filled_cell_material = null;

    [SerializeField] Material _goal_cell_material = null;

    [SerializeField] GoalCellSensor _goal_cell_observer = null;
    [SerializeField] GridCell[,,] _grid = null;
    [SerializeField] IntVector3 _grid_size = new IntVector3(Vector3.one * 20);
    [Range(0.0f, 0.999f)] [SerializeField] float _min_empty_cells_percentage = 0.5f;

    /// <summary>
    /// </summary>
    IntVector3 RandomCoordinates {
      get {
        return new IntVector3(Random.Range(0, max : this._grid_size.X),
                              Random.Range(0, max : this._grid_size.Y),
                              Random.Range(0, max : this._grid_size.Z));
      }
    }

    /// <summary>
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
            new_grid[i, j, k] = this.CreateEmptyCell(x : i,
                                                     y : j,
                                                     z : k,
                                                     xs : xs,
                                                     ys : ys,
                                                     zs : zs);
          }
        }
      }

      return new_grid;
    }

    /// <summary>
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
        this.DoFirstGenerationStep(empty_cells_num : ref empty_cells_num,
                                   grid : ref new_grid,
                                   active_cells : ref active_cells,
                                   c : c,
                                   xs : xs,
                                   ys : ys,
                                   zs : zs); // does not count
        while (active_cells.Count > 0) {
          this.DoNextGenerationStep(empty_cells_num : ref empty_cells_num,
                                    grid : ref new_grid,
                                    active_cells : ref active_cells,
                                    xs : xs,
                                    ys : ys,
                                    zs : zs);
        }

        percentage_empty_cells = empty_cells_num / total_cells;
      }

      for (var i = 0; i < xs; i++) {
        for (var j = 0; j < ys; j++) {
          for (var k = 0; k < zs; k++) {
            if (new_grid[i, j, k] == null) {
              var new_cell = this.CreateFilledCell(x : i,
                                                   y : j,
                                                   z : k,
                                                   xs : xs,
                                                   ys : ys,
                                                   zs : zs);
              new_grid[i, j, k] = new_cell;
            }
          }
        }
      }

      return new_grid;
    }

    /// <summary>
    /// </summary>
    /// <param name="empty_cells_num"></param>
    /// <param name="grid"></param>
    /// <param name="active_cells"></param>
    /// <param name="c"></param>
    /// <param name="xs"></param>
    /// <param name="ys"></param>
    /// <param name="zs"></param>
    void DoFirstGenerationStep(ref int empty_cells_num,
                               ref GridCell[,,] grid,
                               ref List<GridCell> active_cells,
                               IntVector3 c,
                               int xs,
                               int ys,
                               int zs) {
      if (grid[c.X, c.Y, c.Z] == null) {
        grid[c.X, c.Y, c.Z] = this.CreateEmptyCell(c : c,
                                                   xs : xs,
                                                   ys : ys,
                                                   zs : zs);
        empty_cells_num += 1;
      }

      active_cells.Add(grid[c.X, c.Y, c.Z]);
    }

    void DoNextGenerationStep(ref int empty_cells_num,
                              ref GridCell[,,] grid,
                              ref List<GridCell> active_cells,
                              int xs,
                              int ys,
                              int zs) {
      var current_index = active_cells.Count - 1;
      var current_cell = active_cells[index : current_index];
      var direction = MazeDirections.RandomValue;
      var c = current_cell.GridCoordinates + direction.ToIntVector3();

      if (this.ContainsCoordinates(coordinate : c) && grid[c.X, c.Y, c.Z] == null) {
        grid[c.X, c.Y, c.Z] = this.CreateEmptyCell(c : c,
                                                   xs : xs,
                                                   ys : ys,
                                                   zs : zs);
        active_cells.Add(grid[c.X, c.Y, c.Z]);
        empty_cells_num += 1;
      } else {
        active_cells.RemoveAt(index : current_index);
      }
    }

    bool ContainsCoordinates(IntVector3 coordinate) {
      return coordinate.X >= 0
             && coordinate.X < this._grid_size.X
             && coordinate.Y >= 0
             && coordinate.Y < this._grid_size.Y
             && coordinate.Z >= 0
             && coordinate.Z < this._grid_size.Z;
    }

    GridCell CreateEmptyCell(IntVector3 c, IntVector3 size) {
      return this.CreateEmptyCell(x : c.X,
                                  y : c.Y,
                                  z : c.Z,
                                  xs : size.X,
                                  ys : size.Y,
                                  zs : size.Z);
    }

    GridCell CreateEmptyCell(IntVector3 c, int xs, int ys, int zs) {
      return this.CreateEmptyCell(x : c.X,
                                  y : c.Y,
                                  z : c.Z,
                                  xs : xs,
                                  ys : ys,
                                  zs : zs);
    }

    GridCell CreateEmptyCell(int x, int y, int z, int xs, int ys, int zs) {
      var cube = GameObject.CreatePrimitive(type : PrimitiveType.Cube);
      cube.transform.parent = this.transform;
      cube.transform.localPosition =
          new Vector3(x - xs * 0.5f + 0.5f, y - ys * 0.5f + 0.5f, z - zs * 0.5f + 0.5f);
      var new_cell = cube.AddComponent<EmptyCell>();
      var n = $"EmptyCell{x}{y}{z}";
      new_cell.Setup(n : n, mat : this._empty_cell_material);
      new_cell.GridCoordinates = new IntVector3(x : x, y : y, z : z);
      return new_cell;
    }

    GridCell CreateFilledCell(int x, int y, int z, int xs, int ys, int zs) {
      var cube = GameObject.CreatePrimitive(type : PrimitiveType.Cube);

      cube.transform.parent = this.transform;
      cube.transform.localPosition =
          new Vector3(x - xs * 0.5f + 0.5f, y - ys * 0.5f + 0.5f, z - zs * 0.5f + 0.5f);
      var new_cell = cube.AddComponent<FilledCell>();
      var n = $"FilledCell{x}{y}{z}";
      new_cell.Setup(n : n, mat : this._filled_cell_material);
      new_cell.GridCoordinates = new IntVector3(x : x, y : y, z : z);
      return new_cell;
    }

    /// <summary>
    ///
    /// </summary>
    public override void PreSetup() {
      var xs = this._grid_size.X;
      var ys = this._grid_size.Y;
      var zs = this._grid_size.Z;
      this._grid = this.GenerateRandomGrid(xs : xs,
                                           ys : ys,
                                           zs : zs,
                                           min_empty_cells_percentage : this._min_empty_cells_percentage);

      this._goal_cell_observer = this.gameObject.GetComponent<GoalCellSensor>();

      //this.Setup();

      var dominant_dimension = Mathf.Max(xs, ys, zs);
      this._camera.orthographicSize = dominant_dimension / 2f + 1f;
      this._camera.transform.position = new Vector3(0, ys / 2f + 1f, 0);
    }

    void NewGridWorld() {
      var xs = this._grid_size.X;
      var ys = this._grid_size.Y;
      var zs = this._grid_size.Z;

      for (var i = 0; i < xs; i++) {
        for (var j = 0; j < ys; j++) {
          for (var k = 0; k < zs; k++) {
            if (this._grid[i, j, k] != null) {
              DestroyImmediate(obj : this._grid[i, j, k].gameObject);
            }
          }
        }
      }

      this._grid = this.GenerateRandomGrid(xs : xs,
                                           ys : ys,
                                           zs : zs,
                                           min_empty_cells_percentage : this._min_empty_cells_percentage);
    }

    /// <summary>
    ///
    /// </summary>
    public override void Setup() {
      var empty_cells = FindObjectsOfType<EmptyCell>().ToList();

      var objective_function = this.ObjectiveFunction as ReachGoalObjective;

      if (empty_cells != null && empty_cells.Count > 0) {
        foreach (var a in this.Actors) {
          var idx = Random.Range(0, max : empty_cells.Count);
          var empty_cell = empty_cells[index : idx];
          a.Value.Transform.position = empty_cell.transform.position;
          empty_cells.RemoveAt(index : idx);
        }

        if (objective_function != null) {
          var idx = Random.Range(0, max : empty_cells.Count);
          var empty_cell = empty_cells[index : idx];
          empty_cell.SetAsGoal("Goal", mat : this._goal_cell_material);
          this._goal_cell_observer.CurrentGoal = empty_cell;
          objective_function.SetGoal(goal : empty_cell);
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public override void PostStep() {
      if (this.Terminated) {
        this.Terminated = false;

        this.NewGridWorld();

        this.PrototypingReset();

        //this.Setup();

        if (this.Configure) {
          this.Configure = false;
          this.Reconfigure();
        }
      }

      this.LoopConfigurables();
      this.LoopSensors();
    }
  }
}
