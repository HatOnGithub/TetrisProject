using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Threading;

public class TetrisBlock
{
    /// <summary>
    /// Shape of the current Tetris block as a 2D bool array
    /// </summary>
    public bool[,] Shape { get { return shape; } set { shape = value; } }
    protected bool[,] shape;
    
    /// <summary>
    /// Size of the shape
    /// </summary>
    public int Size { get { return size; } }
    protected int size;
    
    /// <summary>
    /// Rotation
    /// </summary>
    public int Rotation { get { return rotation; } set { rotation = value; } }
    protected int rotation;
    
    /// <summary>
    /// Current location on the grid as a Point
    /// </summary>
    public Point Location { get { return location; } set { location = value; } }
    protected Point location;
    protected Point startlocation;

    /// <summary>
    /// Returns the color of the Tetris Block
    /// </summary>
    public Color Color { get { return color; } }
    protected Color color;
    
    /// <summary>
    /// If the shapeToGrid() method has been called [0] shows true
    /// If the shape is outside of bounds when it cannot move down, [1] shows true (loss condition)
    /// </summary>
    public bool HasCommitedToGrid { get { return hasCommitedToGrid; } }
    private bool hasCommitedToGrid = false;

    /// <summary>
    /// Put the shape in Hold
    /// </summary>
    public bool ToHold { get { return toHold; } set { toHold = value; } }
    protected bool toHold = false;

    // Score Modifiers (HardDropped and CellsDroppedBonus)
    public bool HardDropped { get { return hardDropped; } }
    protected bool hardDropped = false;

    public int CellsDroppedBonus { get { return cellsDroppedBonus; } }
    protected int cellsDroppedBonus = 0;
    
    // Lowest point of the block after commiting
    public int LowestPoint { get { return lowestPoint; } }
    protected int lowestPoint = 0;

    // Bool to turn on softdropping bonus counting
    protected bool IsSoftDropping = false;
    
    // What speed the ball should fall
    protected float baseFallSpeed;

    // stores which grid the block is bound to
    protected TetrisGrid targetGrid;
    
    // Matrix of the target grid
    bool[,] gridMatrix;

    // random num generator
    protected Random random = new Random();

    // texture of the block
    protected Texture2D sprite;

    // Rotation System
    protected SuperRotationSystem rotationSystem;

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
        rotation = 0;
    }

    
	
    
    /// <summary>
    /// Moves the block to the lowest possible position
    /// </summary>
    protected void HardDrop()
    {
        int start = location.Y;
        for (int y = start; y < targetGrid.Height; y++)
        {
            location.Y = y;
            if (!MoveTest(shape)[2])
            {
                cellsDroppedBonus = y - start;
                hardDropped = true;
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
                    if (x < 0) continue;
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
    /// Does the stuff to make sure when returning from hold it resets properly
    /// </summary>
    public void ReturnFromHold()
    {
        timerActive = false;
        location = startlocation;
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
                        if (IsSoftDropping) cellsDroppedBonus += 1; 
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

    /// <summary>
    /// Controls for moving the Block
    /// </summary>
    /// <param name="inputHelper"></param>
    public void InputHandler(InputHelper inputHelper)
    {
        // move left
        if (inputHelper.KeyPressed(Keys.A) && (!inputHelper.KeyPressed(Keys.Right) || inputHelper.KeyPressed(Keys.Left)))
        {
            if (MoveTest(shape)[3]) location.X -= 1;
            if (!MoveTest(shape)[2]) timerActive = false;
        }
        // move right
        if (inputHelper.KeyPressed(Keys.D) && (!inputHelper.KeyPressed(Keys.Right) || inputHelper.KeyPressed(Keys.Left)))
        {
            if (MoveTest(shape)[1]) location.X += 1;
            if (!MoveTest(shape)[2]) timerActive = false;
        }


        // Increase Gravity when S is held
        if (inputHelper.KeyPressed(Keys.S))
        {
            IsSoftDropping = true;
            baseFallSpeed *= 2;
        }

        if (inputHelper.KeyReleased(Keys.S))
        {
            IsSoftDropping = false;
            baseFallSpeed /= 2;
        }

        // non-movement actions

        // Move Block to the lowest possible position, gives bonus points
        if (inputHelper.KeyPressed(Keys.Space))
        {
            HardDrop();
            timerActive = false;
        }

        // Rotate Clockwise
        if (inputHelper.KeyPressed(Keys.E) && !(inputHelper.KeyPressed(Keys.A) || inputHelper.KeyPressed(Keys.D)))
        {
            rotationSystem.PerformRotate(SuperRotationSystem.Direction.Clockwise, gridMatrix, shape, location, size, rotation);
            if (!MoveTest(shape)[2]) timerActive = false;
        }

        // Rotate CounterClockwise
        if (inputHelper.KeyPressed(Keys.Q) && !(inputHelper.KeyPressed(Keys.A) || inputHelper.KeyPressed(Keys.D)))
        {
            rotationSystem.PerformRotate(SuperRotationSystem.Direction.CounterClockwise, gridMatrix, shape, location, size, rotation);
            if (!MoveTest(shape)[2]) timerActive = false;
        }

        // Put the piece in Hold
        if (inputHelper.KeyPressed(Keys.F))
        {
            toHold = true;
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
        startlocation = location;
        shape = new bool[3, 3]{

                {false, true , false},

                {true , true , true },

                {false, false, false } };
        color = Color.HotPink;
        size = 3;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
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
        startlocation = location;
        shape = new bool[3, 3]{

                {true , false, false},

                {true , true , true },

                {false, false, false}};
        color = Color.Orange;
        size = 3;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
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
        startlocation = location;
        shape = new bool[3, 3]{

                {false , false , true },

                {true, true , true},

                {false, false , false}};
        color = Color.Blue;
        size = 3;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
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
        startlocation = location;
        shape = new bool[4, 4]{
                {false, false, false, false},

                {true , true , true , true },

                {false, false, false, false},

                {false, false, false, false}};
        color = Color.Cyan;
        size = 4;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
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
        startlocation = location;
        shape = new bool[2, 2]{
                {true , true },

                {true , true}};
        color = Color.Gold;
        size = 2;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
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
        startlocation = location;
        shape = new bool[3, 3]{

                {false, true , true},

                {true, true , false },

                {false, false, false }};
        color = Color.Green;
        size = 3;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
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
        startlocation = location;
        shape = new bool[3, 3]{
                {true, true, false },

                {false, true , true},

                {false, false , false}};
        color = Color.Red;
        size = 3;
        rotationSystem = new SuperRotationSystem(this, targetGrid);
    }
}