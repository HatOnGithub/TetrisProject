using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

public class TetrisBlock
{
    Texture2D emptyCell;

    /// <summary>
    /// Shape of the current Tetris block
    /// </summary>
    public bool[,] Shape { get { return shape; } }
    /// <summary>
    /// Current location on the grid
    /// </summary>
    public Point Location { get { return location; } }

    /// <summary>
    /// Color of current Tetris Block
    /// </summary>
    public Color Color { get { return color; } }

    protected Random random = new Random();
    protected Color color;
    protected Color[] possibleColors = {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple };
    protected bool[,] shape;
    protected bool[,] detectionPattern;
    protected Point location;
    protected TetrisGrid targetGrid;
    private bool[] canMoveTo = new bool[4];


    public TetrisBlock(TetrisGrid targetGrid, Point location)
    {
        this.targetGrid = targetGrid;
        this.location = location;
        color = possibleColors[random.Next(possibleColors.Length - 1)];
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
    }

    public void Update()
    {

    }
	
    public void InputHandler(InputHelper inputHelper)
    {
        if (inputHelper.KeyPressed(Keys.A))
        {
            Test();
            if (canMoveTo[3]) location.X -= 1;
        }
        if (inputHelper.KeyPressed(Keys.D))
        {
            Test();
            if (canMoveTo[1]) location.X += 1;
        }

        if (inputHelper.KeyPressed(Keys.S))
        {
            Test();
            if (canMoveTo[2]) location.Y += 1;
        }

        
        if (inputHelper.KeyPressed(Keys.W))
        {
            Test();
            if (canMoveTo[0]) location.Y -= 1;
        }

        if (inputHelper.KeyPressed(Keys.Space))
        {
            
        }


    }

    /// <summary>
    /// Returns a bool list of length 4: 0 = Above, 1 = Right, 2 = Below, 3 = Left
    /// </summary>
    public void Test()
    {
        bool blockIsPresent = false;
        bool[,] matrix = targetGrid.gridMatrix;
        canMoveTo = Enumerable.Repeat<bool>(true, 4).ToArray(); // sets all values to true

        for (int y = 0; y < location.Y + 4; y++)
        {

            for (int x = 0 ; x < location.X + 4; x++)
            {
                //checks if a part of the shape is in the given coordinates
                if (y - location.Y >= 0 && y - location.Y < 4 && x - location.X >= 0 && x - location.X < 4) 
                {
                    if (shape[y - location.Y, x - location.X]) blockIsPresent = true;
                    else
                    {
                        blockIsPresent = false;
                        continue;
                    }
                }

                if (blockIsPresent) // checks around the shape
                {
                    // this part tests for "edge" cases (see what i did there?)
                    if (y == targetGrid.Height - 1) canMoveTo[2] = false;
                    if (x == 0)                     canMoveTo[3] = false;
                    if (x == targetGrid.Width - 1)  canMoveTo[1] = false;

                    // if the shape is not near an edge, this kicks in
                    if (y > 0 && y < targetGrid.Height - 1)
                    {
                        if (matrix[y - 1, x]) canMoveTo[2] = false;
                        if (matrix[y + 1, x]) canMoveTo[0] = false;

                    }
                    if (x > 0 && x < targetGrid.Width - 1)
                    {
                        if (matrix[y, x - 1]) canMoveTo[3] = false;
                        if (matrix[y, x + 1]) canMoveTo[1] = false;
                    }
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
        
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Vector2 gridPosition = targetGrid.Position;
        bool[,] gridMatrix = targetGrid.gridMatrix;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (shape[i, j])
                {
                    spriteBatch.Draw(emptyCell, new Vector2(gridPosition.X + ((j + location.X) * emptyCell.Width), gridPosition.Y + ((i + location.Y) * emptyCell.Height)), color);
                }

            }
        }
    }
}
/// <summary>
///  T Block 
/// </summary>
class T : TetrisBlock
{
    public T(TetrisGrid targetGrid, Point location)
        : base( targetGrid, location)
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
    public L1(TetrisGrid targetGrid, Point location)
        : base(targetGrid, location)
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
    public L2(TetrisGrid targetGrid, Point location)
        : base(targetGrid, location)
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
    public Long(TetrisGrid targetGrid, Point location)
        : base(targetGrid, location)
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
    public SQ(TetrisGrid targetGrid, Point location)
        : base(targetGrid, location)
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
    public D1(TetrisGrid targetGrid, Point location)
        : base(targetGrid, location)
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
    public D2(TetrisGrid targetGrid, Point location)
        : base(targetGrid, location)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, false, true , false},

                {false, true , true , false},

                {false, true , false, false}};

    }
}