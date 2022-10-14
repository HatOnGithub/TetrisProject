using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

public class TetrisBlock
{
    /// <summary>
    /// Shape of the current Tetris block as a 2D bool array
    /// </summary>
    public bool[,] Shape { get { return shape; } }
    /// <summary>
    /// Current location on the grid as a Point
    /// </summary>
    public Point Location { get { return location; } }

    /// <summary>
    /// Returns the color of the Tetris Block
    /// </summary>
    public Color Color { get { return color; } }

    /// <summary>
    /// If the shapeToGrid() method has been called it shows true
    /// </summary>
    public bool HasCommitedToGrid { get { return hasCommitedToGrid; } }
    private bool hasCommitedToGrid = false;


    protected float baseFallSpeed;
    protected int rotation = 0;
    protected bool[,] shape;
    protected Color color;
    protected Color[] possibleColors = {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple };
    protected Point location;
    protected TetrisGrid targetGrid;
    protected Random random = new Random();
    protected Texture2D emptyCell;

    protected int lastTime = 0;
    protected int currentTime;
    bool timer = false;


    /// <summary>
    /// sets starting values
    /// </summary>
    /// <param name="targetGrid"></param>
    /// <param name="location"></param>
    /// <param name="baseFallSpeed"></param>
    public TetrisBlock(TetrisGrid targetGrid, Point location, float baseFallSpeed)
    {
        this.targetGrid = targetGrid;
        this.location = location;
        this.baseFallSpeed = baseFallSpeed;
        color = possibleColors[random.Next(possibleColors.Length - 1)];
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
    }

    /// <summary>
    /// Does Physics stuff
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        currentTime = (int) gameTime.TotalGameTime.TotalMilliseconds;
        switch (timer)
        {
            case false:
                lastTime = currentTime;
                timer = true;
                break;
            case true:
                if (currentTime >= lastTime + (1000 / baseFallSpeed))
                {
                    bool[] canMoveTo = MoveTest(shape);
                    if (canMoveTo[2])
                    {
                        location.Y += 1;
                        timer = false;
                    }
                    else if (currentTime >= lastTime + 500)
                    {
                        shapeToGrid();
                        timer = false;
                    }
                }
                break;
        }

    }
	
    /// <summary>
    /// Controls for moving the Block
    /// </summary>
    /// <param name="inputHelper"></param>
    public void InputHandler(InputHelper inputHelper)
    {
        if (inputHelper.KeyPressed(Keys.A))
        {
            bool[] canMoveTo = MoveTest(shape);
            if (canMoveTo[3]) location.X -= 1;
            timer = false;
        }
        if (inputHelper.KeyPressed(Keys.D))
        {
            bool[] canMoveTo = MoveTest(shape);
            if (canMoveTo[1]) location.X += 1;
            timer = false;
        }

        if (inputHelper.KeyPressed(Keys.S))
        {
            bool[] canMoveTo = MoveTest(shape);
            if (canMoveTo[2]) location.Y += 1;
            timer = false;
        }

        // for debugging
        if (inputHelper.KeyPressed(Keys.W))
        {
            bool[] canMoveTo = MoveTest(shape);
            if (canMoveTo[0]) location.Y -= 1;
            timer = false;
        }
        
        if (inputHelper.KeyPressed(Keys.Space))
        {
            
        }

        if (inputHelper.KeyPressed(Keys.Right))
        {
            RotateCW();
            timer = false;
        }

        if (inputHelper.KeyPressed(Keys.Left))
        {
            RotateCCW();
            timer = false;
        }

    }

    /// <summary>
    /// Returns a bool array of length 4 defined as such: 
    /// [0] = Up, [1] = Right, [2] = Down, [3] = Left
    /// </summary>
    protected bool[] MoveTest(bool[,] testshape)
    {
        bool[,] gridMatrix = targetGrid.gridMatrix;
        bool[] canMoveTo = Enumerable.Repeat<bool>(true, 4).ToArray(); // sets all values to true

        for (int y = location.Y; y < location.Y + 4; y++)
        {
            if (y < 0) continue;
            for (int x = location.X ; x < location.X + 4; x++) // iterates through possible locations
            {
                //checks if the 4x4 block is in the given coordinates
                if (y - location.Y >= 0 && y - location.Y < 4 && x - location.X >= 0 && x - location.X < 4) 
                {
                    // This part has to be nested else you get problems with indexes being out of range
                    // It checks if a part of the shape is present within the given coordinates
                    if (testshape[y - location.Y, x - location.X]) 
                    {
                        // this part tests for "edge" cases (see what i did there?)
                        if (y == targetGrid.Height - 1) canMoveTo[2] = false;
                        if (x == 0)                     canMoveTo[3] = false;
                        if (x == targetGrid.Width - 1)  canMoveTo[1] = false;

                        // if the shape is not near an edge, this kicks in
                        if (y > 0 && y < targetGrid.Height - 1)
                        {
                            if (gridMatrix[y - 1, x])   canMoveTo[0] = false;
                            if (gridMatrix[y + 1, x])   canMoveTo[2] = false;

                        }
                        if (x > 0 && x < targetGrid.Width - 1)
                        {
                            if (gridMatrix[y, x - 1])   canMoveTo[3] = false;
                            if (gridMatrix[y, x + 1])   canMoveTo[1] = false;
                        }
                    }
                }
            }
        }
        return canMoveTo;
    }

    /// <summary>
    /// Returns a bool if the requested shape fits in the location with the given offset
    /// </summary>
    /// <param name="rotatedShape"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    protected bool RotateTest(bool[,] rotatedShape, int offsetX, int offsetY) // doesn't fuckin work
    {
        bool[,] gridMatrix = targetGrid.gridMatrix;
        Point offsetLocation = new Point(offsetY + location.Y, offsetX + location.X);
        for (int y = offsetLocation.Y; y < offsetLocation.Y + 4; y++)
        {
            for (int x = offsetLocation.X; x < offsetLocation.X + 4; x++) // iterates through possible offsetLocations
            {
                if (x < 0)
        {
                    if (rotatedShape[0, -1 + (offsetLocation.X + x) * -1] || 
                        rotatedShape[1, -1 + (offsetLocation.X + x) * -1] || 
                        rotatedShape[2, -1 + (offsetLocation.X + x) * -1] || 
                        rotatedShape[3, -1 + (offsetLocation.X + x) * -1]) 
                        
                        return false;
                }
                if (x > targetGrid.Width - 1)
            {
                    if (rotatedShape[0, 4 - (x - targetGrid.Width - 1)] ||
                        rotatedShape[1, 4 - (x - targetGrid.Width - 1)] ||
                        rotatedShape[2, 4 - (x - targetGrid.Width - 1)] ||
                        rotatedShape[3, 4 - (x - targetGrid.Width - 1)]) 

                        return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Rotates shape ClockWise, Test() built in, no need to call it seperately
    /// </summary>
    public void RotateCW()
    {
        bool[,] a = shape; 
        bool[,] rotated = new bool[4, 4] {
            {a[3, 0], a[2, 0], a[1, 0], a[0, 0] },
            {a[3, 1], a[2, 1], a[1, 1], a[0, 1] },
            {a[3, 2], a[2, 2], a[1, 2], a[0, 2] },
            {a[3, 3], a[2, 3], a[1, 3], a[0, 3] }};
        for (int x = -1; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
        {
                if (RotateTest(rotated, x, y)) shape = rotated;
            }
        }
    }

    /// <summary>
    /// Rotates shape Counter ClockWise, Test() built in, no need to call it seperately
    /// </summary>
    public void RotateCCW()
    {
        bool[,] a = shape;
        bool[,] rotated = new bool[4, 4] {
        {a[0, 3], a[1, 3], a[2, 3], a[3, 3] },
        {a[0, 2], a[1, 2], a[2, 2], a[3, 2] },
        {a[0, 1], a[1, 1], a[2, 1], a[3, 1] },
        {a[0, 0], a[1, 0], a[2, 0], a[3, 0] }};
        
        for (int x = -1; x < 2; x++)
        {
            for(int y = 0; y < 2; y++)
            {
                if (RotateTest(rotated, x, y)) shape = rotated;
            }
        }
    }

    // draws the shape by testing the shape matrix and then drawing it on screen with the location as offset compared to the targeted grid
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 gridPosition = targetGrid.Position;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (shape[i, j] && location.Y + i >= 0)
                {
                    spriteBatch.Draw(emptyCell, new Vector2(gridPosition.X + ((j + location.X) * emptyCell.Width), gridPosition.Y + ((i + location.Y) * emptyCell.Height)), color);
                }
            }
        }
    }

    public void shapeToGrid()
    {
        bool[,] newMatrix = new bool[targetGrid.Height, targetGrid.Width];
        Color[,] newColorMatrix = new Color[targetGrid.Height, targetGrid.Width];
        for (int y = 0; y < targetGrid.Height; y++)
        {
            for (int x = 0; x < targetGrid.Width; x++)
            {
                if (y - location.Y >= 0 && y - location.Y < 4 && x - location.X >= 0 && x - location.X < 4)
                {
                    if (shape[y - location.Y, x - location.X])
                    {
                        newMatrix[y, x] = true;
                        newColorMatrix[y, x] = color;
                    }
                    else
                    {
                        newMatrix[y, x] = targetGrid.gridMatrix[y, x];
                        newColorMatrix[y, x] = targetGrid.colorMatrix[y, x];
                    }
                }
                else
                {
                    newMatrix[y, x] = targetGrid.gridMatrix[y, x];
                    newColorMatrix[y, x] = targetGrid.colorMatrix[y, x];
                }
            }
        }
        targetGrid.gridMatrix = newMatrix;
        targetGrid.colorMatrix = newColorMatrix;
        hasCommitedToGrid = true;
    }
}

// Subcla

/// <summary>
///  T Block 
/// </summary>
class T : TetrisBlock
{
    public T(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base( targetGrid, location, baseFallSpeed)
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
    public L1(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base(targetGrid, location, baseFallSpeed)
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
    public L2(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base(targetGrid, location, baseFallSpeed)
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
    public Long(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base(targetGrid, location, baseFallSpeed)
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
    public SQ(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base(targetGrid, location, baseFallSpeed)
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
    public D1(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base(targetGrid, location, baseFallSpeed)
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
    public D2(TetrisGrid targetGrid, Point location, float baseFallSpeed)
        : base(targetGrid, location, baseFallSpeed)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, false, true , false},

                {false, true , true , false},

                {false, true , false, false}};

    }
}