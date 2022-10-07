using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;
using System.Text;

public class TetrisBlock
{
    /// <summary>
    /// Shape of the current Tetris block
    /// </summary>
    public bool[,] Shape { get { return shape; } }
    /// <summary>
    /// Current location on the grid
    /// </summary>
    public Point Location { get { return location; } }


    protected bool[,] shape;
    protected bool[,] detectionPattern;
    protected Point location;
    protected TetrisGrid targetGrid;
    private bool[] canMoveTo = new bool[4];
    public TetrisBlock(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
    {
        this.shape = shape;
        this.detectionPattern = detectionPattern;
        this.targetGrid = targetGrid;
        this.location = location;
    }

    
	
    public static void InputHandler(InputHelper inputHelper)
    {
        if (inputHelper.KeyPressed(Keys.A))
        {
            
        }

        if (inputHelper.KeyPressed(Keys.D))
        {

        }

        if (inputHelper.KeyPressed(Keys.S))
        {

        }

        if (inputHelper.KeyDown(Keys.S))
        {

        }

        if (inputHelper.KeyPressed(Keys.Space))
        {
            
        }


    }

    /// <summary>
    /// Returns a bool list of length 4: 0 = Above, 1 = Right, 2 = Below, 3 = Left
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="detectionPattern"></param>
    /// <returns></returns>
    public void Test()
    {
        bool[,] matrix = targetGrid.gridMatrix;
        for (int i = 0; i < detectionPattern.Length; i++)
        {
            for (int j = 0 ; j < detectionPattern.Length; j++)
            {
                if (location.X + j < 0)
                {
                    if (detectionPattern[i,j]) canMoveTo[3] = false;
                    break;
                }
                if (location.X + j > targetGrid.gridMatrix.GetLength(0))
                {
                    if (detectionPattern[i, j]) canMoveTo[1] = false;
                    break;
                }
                if (location.Y + j > targetGrid.gridMatrix.GetLength(1))
                {
                    if (detectionPattern[i, j]) canMoveTo[2] = false;
                    break;
                }
                if (location.Y < 0) break; // prevents it from assigning values outside the array
                if (matrix[i + location.Y, j + location.X] == detectionPattern[i, j] && detectionPattern[i, j])
                {
                    if (shape[i - 1, j]) canMoveTo[2] = false; // check if space below is free
                    else canMoveTo[2] = true;
                    if (shape[i + 1, j]) canMoveTo[0] = false; // check if space above is free
                    else canMoveTo[0] = true;
                    if (shape[i, j + 1]) canMoveTo[3] = false; // check if space to the left is free
                    else canMoveTo[3] = true;
                    if (shape[i, j - 1]) canMoveTo[1] = false; // check if space to the right is free
                    else canMoveTo[1] = true;
                }
            }
        }
        
        // check which side got triggered and update result
    }
    /// <summary>
    /// Rotates shape ClockWise
    /// </summary>
    public void RotateCW()
    {
        bool[,] a = shape;
        bool[,] rotatedA = new bool[4, 4] {
            {a[3, 0], a[2, 0], a[1, 0], a[0, 0] },
            {a[3, 1], a[2, 1], a[1, 1], a[0, 1] },
            {a[3, 2], a[2, 2], a[1, 2], a[0, 2] },
            {a[3, 3], a[2, 3], a[1, 3], a[0, 3] }};

        bool[,] b = detectionPattern;
        bool[,] rotatedB = new bool[4, 4] {
            {b[3, 0], b[2, 0], b[1, 0], b[0, 0] },
            {b[3, 1], b[2, 1], b[1, 1], b[0, 1] },
            {b[3, 2], b[2, 2], b[1, 2], b[0, 2] },
            {b[3, 3], b[2, 3], b[1, 3], b[0, 3] }};
        this.detectionPattern = rotatedB;

    }

    /// <summary>
    /// Rotates shape Counter ClockWise
    /// </summary>
    public void RotateCCW()
    {
        
        bool[,] a = shape;
        bool[,] rotated = new bool[4, 4] {
            {a[0, 3], a[1, 3], a[2, 3], a[3, 3] },
            {a[0, 2], a[1, 2], a[2, 2], a[3, 2] },
            {a[0, 1], a[1, 1], a[2, 1], a[3, 1] },
            {a[0, 0], a[1, 0], a[2, 0], a[3, 0] }};
        this.shape = rotated;

        bool[,] b = detectionPattern;
        bool[,] rotatedB = new bool[4, 4] {
            {b[0, 3], b[1, 3], b[2, 3], b[3, 3] },
            {b[0, 2], b[1, 2], b[2, 2], b[3, 2] },
            {b[0, 1], b[1, 1], b[2, 1], b[3, 1] },
            {b[0, 0], b[1, 0], b[2, 0], b[3, 0] }};
        this.detectionPattern = rotatedB;
    }
}
/// <summary>
///  T Block 
/// </summary>
class T : TetrisBlock
{
    public T(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , false, false},

                {false, true , true , false},

                {false, true , false, false}};
    }
}
/// <summary>
/// L Block
/// </summary>
class L1 : TetrisBlock
{
    public L1(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, true , false, false},

                {false, true , false, false}};
    }
}
/// <summary>
/// Flipped L Block
/// </summary>
class L2 : TetrisBlock
{
    public L2(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, false, true , false},

                {false, false, true , false}};
    }
}
/// <summary>
/// Long BLock
/// </summary>
class Long : TetrisBlock
{
    public Long(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, true , false, false},

                {false, true , false, false},

                {false, true , false, false},

                {false, true , false, false}};
    }
}
/// <summary>
/// Square Block
/// </summary>
class SQ : TetrisBlock
{
    public SQ(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, true , true , false},

                {false, false, false, false}};
    }
}

/// <summary>
/// Diagonal Block with left high
/// </summary>
class D1 : TetrisBlock
{
    public D1(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , false, false},

                {false, true , true , false},

                {false, false, true , false}};
    }
}
/// <summary>
/// Diagonal Block with right high
/// </summary>
class D2 : TetrisBlock
{
    public D2(bool[,] shape, bool[,] detectionPattern, TetrisGrid targetGrid, Point location)
        : base(shape, detectionPattern, targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, false, true , false},

                {false, true , true , false},

                {false, true , false, false}};

    }
}