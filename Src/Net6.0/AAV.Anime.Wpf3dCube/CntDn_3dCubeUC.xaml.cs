using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace AAV.Anime.Wpf3dCube
{
  public partial class CntDn_3dCubeUC : UserControl
  {
    class CubeRotation // A class which represents the quaternion rotation needed to display a given cube side based on the initially displayed side being the 'front'.
    {
      public CubeSide CubeSide { get; set; }
      public Quaternion Quaternion { get; set; }
    }

    static readonly CubeRotation _cubeFrontSideFacing, _cubeBack_SideFacing, _cubeLeft_SideFacing, _cubeRightSideFacing, _cubeTop__SideFacing, _cubeBottmSideFacing;
    static readonly Dictionary<CubeSide, Dictionary<Direction, CubeRotation>> _possibleRotationMatrix;

    CubeRotation _currentCubeRotation;
    bool _isRotating;

    enum CubeSide { Front, Back, Left, Right, Top, Bottom }
    enum Direction { None, Nort, East, Sout, West }

    static CntDn_3dCubeUC() // Create a class for each side of the cube which contains the quaternion rotation needed to display the side based on the intially displayed side being the 'front'.
    {
      _cubeFrontSideFacing = new CubeRotation() { CubeSide = CubeSide.Front  /**/, Quaternion = new Quaternion(new Vector3D(0, 0, 1), 0) };
      _cubeBack_SideFacing = new CubeRotation() { CubeSide = CubeSide.Back   /**/, Quaternion = new Quaternion(new Vector3D(0, 1, 0), 180) };
      _cubeLeft_SideFacing = new CubeRotation() { CubeSide = CubeSide.Left   /**/, Quaternion = new Quaternion(new Vector3D(0, 1, 0), -90) };
      _cubeRightSideFacing = new CubeRotation() { CubeSide = CubeSide.Right  /**/, Quaternion = new Quaternion(new Vector3D(0, 1, 0), 90) };
      _cubeTop__SideFacing = new CubeRotation() { CubeSide = CubeSide.Top    /**/, Quaternion = new Quaternion(new Vector3D(1, 0, 0), -90) };
      _cubeBottmSideFacing = new CubeRotation() { CubeSide = CubeSide.Bottom /**/, Quaternion = new Quaternion(new Vector3D(1, 0, 0), 90) };

      _possibleRotationMatrix = new Dictionary<CubeSide, Dictionary<Direction, CubeRotation>>(); // For each cube side that could be facing there are 4 possible directions (based on user dragging a mouse) to rotate the cube. Store this as a collection for easy use later on.

      var possibleRotations = new Dictionary<Direction, CubeRotation>(5)
      {

        // 1. FRONT - First store all possible rotations from the cube front side facing
        [Direction.None] = _cubeFrontSideFacing,
        [Direction.West] = _cubeRightSideFacing,
        [Direction.East] = _cubeLeft_SideFacing,
        [Direction.Nort] = _cubeBottmSideFacing,
        [Direction.Sout] = _cubeTop__SideFacing
      };

      _possibleRotationMatrix[CubeSide.Front] = possibleRotations;

      // 2. BACK
      possibleRotations = new Dictionary<Direction, CubeRotation>(5)
      {
        [Direction.None] = _cubeBack_SideFacing,
        [Direction.West] = _cubeLeft_SideFacing,
        [Direction.East] = _cubeRightSideFacing,
        [Direction.Nort] = _cubeBottmSideFacing,
        [Direction.Sout] = _cubeTop__SideFacing
      };

      _possibleRotationMatrix[CubeSide.Back] = possibleRotations;

      // 3. LEFT
      possibleRotations = new Dictionary<Direction, CubeRotation>(5)
      {
        [Direction.None] = _cubeLeft_SideFacing,
        [Direction.West] = _cubeFrontSideFacing,
        [Direction.East] = _cubeBack_SideFacing,
        [Direction.Nort] = _cubeBottmSideFacing,
        [Direction.Sout] = _cubeTop__SideFacing
      };

      _possibleRotationMatrix[CubeSide.Left] = possibleRotations;

      // 4. RIGHT
      possibleRotations = new Dictionary<Direction, CubeRotation>(5)
      {
        [Direction.None] = _cubeRightSideFacing,
        [Direction.West] = _cubeBack_SideFacing,
        [Direction.East] = _cubeFrontSideFacing,
        [Direction.Nort] = _cubeBottmSideFacing,
        [Direction.Sout] = _cubeTop__SideFacing
      };

      _possibleRotationMatrix[CubeSide.Right] = possibleRotations;

      // 5. TOP
      possibleRotations = new Dictionary<Direction, CubeRotation>(5)
      {
        [Direction.None] = _cubeTop__SideFacing,
        [Direction.West] = _cubeRightSideFacing,
        [Direction.East] = _cubeLeft_SideFacing,
        [Direction.Nort] = _cubeFrontSideFacing,
        [Direction.Sout] = _cubeBack_SideFacing
      };

      _possibleRotationMatrix[CubeSide.Top] = possibleRotations;

      // 6. BOTTOM
      possibleRotations = new Dictionary<Direction, CubeRotation>(5)
      {
        [Direction.None] = _cubeBottmSideFacing,
        [Direction.West] = _cubeRightSideFacing,
        [Direction.East] = _cubeLeft_SideFacing,
        [Direction.Nort] = _cubeBack_SideFacing,
        [Direction.Sout] = _cubeFrontSideFacing
      };

      _possibleRotationMatrix[CubeSide.Bottom] = possibleRotations;
    }

    public CntDn_3dCubeUC()
    {
      InitializeComponent();
      _currentCubeRotation = _cubeFrontSideFacing;


      _d = new Direction[10];
      for (var i = 0; i < _d.Length; i++) { _d[i] = (Direction)_rnd.Next(1, 4); }

      _d[0] = Direction.West;
      _d[1] = Direction.Sout;
      _d[2] = Direction.East;
      _d[3] = Direction.Nort;
      _d[4] = Direction.West;
      _d[5] = Direction.Sout;
      _d[6] = Direction.East;
      _d[7] = Direction.Nort;
      _d[8] = Direction.West;

      _dt = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, new EventHandler(onTimerTick), Dispatcher.CurrentDispatcher);
    }

    readonly DispatcherTimer _dt;
    readonly Random _rnd = new Random(DateTime.Now.Second);
    readonly Direction[] _d;
    int _step = 0;
    void onTimerTick(object sender, EventArgs e)
    {
      var d = (_step++) % 9;
      RotateCube(_d[d]); // RotateCube(Direction.East); tbr.Text = "  8";

      if (d == 3)
        tbr.Text = "  4";
      if (d == 4)
      {
        tbl.Text = "  2";
        tbu.Text = "  3";
      }
      if (d == 5)
      {
        tbf.Text = "  5";
        tbd.Text = "  1";
      }
      if (d == 7)
        tbr.Text = "  0";

      if (d == 8)
        _dt.Stop();
    }

    void RotateCube(Direction direction) // Rotates the cube (gives the illusion by rotating the camera).
    {
      if (!_isRotating)
      {
        var animation = new QuaternionAnimation
        {
          From = _possibleRotationMatrix[_currentCubeRotation.CubeSide][Direction.None].Quaternion,   // The From quaternion is the one required to display the current cube side based on the original side being the 'front'  
          To = _possibleRotationMatrix[_currentCubeRotation.CubeSide][direction].Quaternion,          // The  To  quaternion is the one required to display the next cube side based on the original side being the 'front'
          Duration = new Duration(new TimeSpan(0, 0, 0, 0, 650)),
          EasingFunction =
        //  new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations=1, Springiness=3 }; // 
        new BounceEase { EasingMode = EasingMode.EaseOut, Bounciness = 11 }
        }; // Let the quaternion animation figure out how to transform between 2 quaternions... // A quaternion is way of representing a rotation around an axis

        _isRotating = true;

        animation.Completed += (o, e) =>
        {
          _isRotating = false;
          IsHitTestVisible = (true);
          _currentCubeRotation = _possibleRotationMatrix[_currentCubeRotation.CubeSide][direction];
        };

        IsHitTestVisible = (false); // Temporarily remove hit testing to make things a but smoother

        CameraRotation.BeginAnimation(QuaternionRotation3D.QuaternionProperty, animation);
      }
    }
  }
}
