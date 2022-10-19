using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

public class TetrisBlock
{
    /// <summary>
    /// Shape of the current Tetris block as a 2D bool array
    /// </summary>
    public bool[,] Shape { get { return shape; } }
    
    /// <summary>
    /// Size of the shape
    /// </summary>
    public int Size { get { return size; } }
    
    /// <summary>
    /// Current location on the grid as a Point
    /// </summary>
    public Point Location { get { return location; } }

    /// <summary>
    /// Returns the color of the Tetris Block
    /// </summary>
    public Color Color { get { return color; } }

    /// <summary>
    /// If the shapeToGrid() method has been called [0] shows true
    /// If the shape is outside of bounds when it cannot move down, [1] shows true (loss condition)
    /// </summary>
    public bool HasCommitedToGrid { get { return hasCommitedToGrid; } }
    private bool hasCommitedToGrid = false;


    public int lowestPoint;

    protected float baseFallSpeed;
    protected bool[,] shape;
    protected int size;
    bool[,] gridMatrix;
    protected Color color;
    protected Point location;


    // stores which grid the block is bound to
    protected TetrisGrid targetGrid;

    // random num generator
    protected Random random = new Random();

    // texture of the block
    protected Texture2D sprite;

    // timer values
    protected int lastTime = 0;
    protected int currentTime;
    bool timerActive = false;


    /// <summary>
    /// sets starting values
    /// </summary>
    /// <param name="targetGrid"></param>
    /// <param name="location"></param>
    /// <param name="baseFallSpeed"></param>
    public TetrisBlock(TetrisGrid targetGrid, float baseFallSpeed)
    {
        // sets the target grid and starting values
        this.targetGrid = targetGrid;
        gridMatrix = targetGrid.gridMatrix;
        this.baseFallSpeed = baseFallSpeed;
        sprite = TetrisGame.ContentManager.Load<Texture2D>("block");
    }

    
	
    /// <summary>
    /// Controls for moving the Block
    /// </summary>
    /// <param name="inputHelper"></param>
    public void InputHandler(InputHelper inputHelper)
    {
        // movement
        if (inputHelper.KeyPressed(Keys.A))
        {
            if (MoveTest(shape)[3]) location.X -= 1;
            if (!MoveTest(shape)[2]) timerActive = false;
        }
        if (inputHelper.KeyPressed(Keys.D))
        {
            if (MoveTest(shape)[1]) location.X += 1;
            if (!MoveTest(shape)[2]) timerActive = false;
        }

        if (inputHelper.KeyPressed(Keys.S))
        {
            baseFallSpeed *= 2;
        }
        if (inputHelper.KeyReleased(Keys.S))
        {
            baseFallSpeed /= 2;
        }

        // for debugging
        if (inputHelper.KeyPressed(Keys.W))
        {
            if (MoveTest(shape)[0]) location.Y -= 1;
            timerActive = false;
        }


        // non-movement actions
        if (inputHelper.KeyPressed(Keys.Space))
        {
            HardDrop();
        }

        if (inputHelper.KeyPressed(Keys.Right))
        {
            RotateCW();
            timerActive = false;
        }

        if (inputHelper.KeyPressed(Keys.Left))
        {
            RotateCCW();
            timerActive = false;
        }

    }
    /// <summary>
    /// Moves the block to the lowest possible position
    /// </summary>
    protected void HardDrop()
    {
        for (int y = location.Y; y < targetGrid.Height; y++)
        {
            location.Y = y;
            if (!MoveTest(shape)[2])
            {
                CommitToGrid();
                break;
            }
        }
    }

    /// <summary>
    /// Returns a bool array of length 4 defined as such: 
    /// [0] = Up, [1] = Right, [2] = Down, [3] = Left
    /// </summary>
    protected bool[] MoveTest(bool[,] testshape)
    {
        // sets all values to true
        bool[] canMoveTo = Enumerable.Repeat<bool>(true, 4).ToArray(); 

        // if the entry point for a block is filled, can't move down
        if (gridMatrix[0, targetGrid.Width / 2] || gridMatrix[0, targetGrid.Width / 2 + 1]) canMoveTo[2] = false; 

        // goes through the shape
        for (int y = location.Y; y < location.Y + size; y++)
        {
            if (y < 0) continue;
            for (int x = location.X ; x < location.X + size; x++) // iterates through possible locations
            {
                //checks if the 4x4 block is in the given coordinates
                if (y - location.Y >= 0 && y - location.Y < size && x - location.X >= 0 && x - location.X < 4) 
                {
                    // This part has to be nested else you get problems with indexes being out of range
                    // It checks if a part of the shape is present within the given coordinates
                    if (testshape[y - location.Y, x - location.X]) 
                    {
                        // this part tests for "edge" cases (see what i did there?)
                        if (y == targetGrid.Height - 1) canMoveTo[2] = false;
                        if (x == 0)                     canMoveTo[3] = false;
                        if (x == targetGrid.Width - 1)  canMoveTo[1] = false;

                        // if the part of the shape is not near an edge, this kicks in
                        if (y > 0 && y < targetGrid.Height - 1) // if not near top or bottom
                        {
                            if (gridMatrix[y - 1, x])   canMoveTo[0] = false;
                            if (gridMatrix[y + 1, x])   canMoveTo[2] = false;

                        }
                        if (x > 0 && x < targetGrid.Width - 1) // if not near sides
                        {
                            if (gridMatrix[y, x - 1])   canMoveTo[3] = false;
                            if (gridMatrix[y, x + 1])   canMoveTo[1] = false;
                        }
                        lowestPoint = y;
                    }
                }
            }
        }
        return canMoveTo;
    }


    /// <summary>
    /// Rotates shape ClockWise, Test() built in, no need to call it seperately
    /// </summary>
    protected void RotateCW()
    {
        if (size != 2) // because rotationg a square doesn't do anything
        {
            bool[,] rotated = new bool[0,0];
            switch (size)
            {
                case 3: rotated = new bool[3, 3]{
                    {shape[2,0], shape[1,0], shape[0,0] },
                    {shape[2,1], shape[1,1], shape[0,1] },
                    {shape[2,2], shape[1,2], shape[0,2] }};
                    break;

                case 4: rotated = new bool[4, 4] {
                    {shape[3, 0], shape[2, 0], shape[1, 0], shape[0, 0] },
                    {shape[3, 1], shape[2, 1], shape[1, 1], shape[0, 1] },
                    {shape[3, 2], shape[2, 2], shape[1, 2], shape[0, 2] },
                    {shape[3, 3], shape[2, 3], shape[1, 3], shape[0, 3] }};
                    break;
            }
            shape = rotated;
        }

    }

    /// <summary>
    /// Rotates shape Counter ClockWise, Test() built in, no need to call it seperately
    /// </summary>
    protected void RotateCCW()
    {
        if (size != 2) // because rotating a square doesn't do anything
        {
            bool[,] rotated = new bool[0, 0];

            switch (size)
            {
                case 3:
                    rotated = new bool[3, 3] {
                    {shape[0, 2], shape[1, 2], shape[2, 2]},
                    {shape[0, 1], shape[1, 1], shape[2, 1]},
                    {shape[0, 2], shape[1, 0], shape[2, 0]}};
                    break;

                case 4:
                    rotated = new bool[4, 4] {
                    {shape[0, 3], shape[1, 3], shape[2, 3], shape[3, 3] },
                    {shape[0, 2], shape[1, 2], shape[2, 2], shape[3, 2] },
                    {shape[0, 1], shape[1, 1], shape[2, 1], shape[3, 1] },
                    {shape[0, 0], shape[1, 0], shape[2, 0], shape[3, 0] }};
                    break;
            }
            shape = rotated;
        }
    }

    /// <summary>
    /// If the block has reached it's lowest point, transfer the data of the block to the target grid
    /// </summary>
    protected void CommitToGrid()
    {
        for (int y = 0; y < targetGrid.Height; y++)
        {
            for (int x = 0; x < targetGrid.Width; x++)
            {
                if (y - location.Y >= 0 && y - location.Y < size && x - location.X >= 0 && x - location.X < size)
                {
                    if (shape[y - location.Y, x - location.X])
                    {
                        targetGrid.gridMatrix[y, x] = true;
                        targetGrid.colorMatrix[y, x] = color;
                    }
                }
            }
        }
        hasCommitedToGrid = true;
    }

    /// <summary>
    /// Does Physics stuff
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        
        switch (timerActive)
        {
            case false:
                lastTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                timerActive = true;
                break;
            case true:
                currentTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                if (currentTime >= lastTime + (1000 / baseFallSpeed))
                {
                    if (MoveTest(shape)[2])
                    {
                        location.Y += 1;
                        timerActive = false;
                    }
                    else if (currentTime >= lastTime + 500)
                    {
                        CommitToGrid();
                        timerActive = false;
                    }
                }
                break;
        }

    }


    // draws the shape by testing the shape matrix and then drawing it on screen with the location as offset compared to the targeted grid
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        
        Vector2 gridPosition = targetGrid.Position;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (shape[i, j] && location.Y + i >= 0)
                {
                    spriteBatch.Draw(sprite, new Vector2(gridPosition.X + ((j + location.X) * sprite.Width), gridPosition.Y + ((i + location.Y) * sprite.Height)), color);
                }
            }
        }
        
    }
}

// Subclasses for each possible shape

/// <summary>
///  T Block 
/// </summary>
class T : TetrisBlock
{
    public T(TetrisGrid targetGrid, float baseFallSpeed)
        : base( targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 2, 0);
        shape = new bool[3, 3]{

                {false, true , false},

                {true , true , true },

                {false, false, false } };
        color = Color.HotPink;
        size = 3;
    }
}
/// <summary>
/// L Block
/// </summary>
class L : TetrisBlock
{
    public L(TetrisGrid targetGrid, float baseFallSpeed)
        : base(targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 2, 0);
        shape = new bool[3, 3]{

                {true , false, false},

                {true , true , true },

                {false, false, false}};
        color = Color.Orange;
        size = 3;
    }
}
/// <summary>
/// Flipped L Block
/// </summary>
class J : TetrisBlock
{
    public J(TetrisGrid targetGrid, float baseFallSpeed)
        : base(targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 2, 0);
        shape = new bool[3, 3]{

                {false , false , true },

                {true, true , true},

                {false, false , false}};
        color = Color.Blue;
        size = 3;
    }
}
/// <summary>
/// Long BLock
/// </summary>
class I : TetrisBlock
{
    public I(TetrisGrid targetGrid, float baseFallSpeed)
        : base(targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 2, -1);
        shape = new bool[4, 4]{
                {false, false, false, false},

                {true , true , true , true },

                {false, false, false, false},

                {false, false, false, false}};
        color = Color.Cyan;
        size = 4;
    }
}
/// <summary>
/// Square Block
/// </summary>
class O : TetrisBlock
{
    public O(TetrisGrid targetGrid, float baseFallSpeed)
        : base(targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 1, 0);
        shape = new bool[2, 2]{
                {true , true },

                {true , true}};
        color = Color.Gold;
        size = 2;
    }
}

/// <summary>
/// The S shaped Tetris Block
/// </summary>
class S : TetrisBlock
{
    public S(TetrisGrid targetGrid, float baseFallSpeed)
        : base(targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 2, 0);
        shape = new bool[3, 3]{

                {false, true , true},

                {true, true , false },

                {false, false, false }};
        color = Color.Green;
        size = 3;
    }
}
/// <summary>
/// The Z Shaped Tetris block
/// </summary>
class Z : TetrisBlock
{
    public Z(TetrisGrid targetGrid, float baseFallSpeed)
        : base(targetGrid, baseFallSpeed)
    {
        location = new(targetGrid.Width / 2 - 2, 0);
        shape = new bool[3, 3]{
                {true, true, false },

                {false, true , true},

                {false, false , false}};
        color = Color.Red;
        size = 3;
    }
}