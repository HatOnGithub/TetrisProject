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
    public bool[,] shape;
    
    /// <summary>
    /// Size of the shape
    /// </summary>
    public int size;
    
    /// <summary>
    /// Rotation
    /// </summary>
    public int rotation;
    
    /// <summary>
    /// Current location on the grid as a Point
    /// </summary>
    public Point location;
    protected Point startlocation;

    /// <summary>
    /// Returns the color of the Tetris Block
    /// </summary>
    public Color color;
    
    /// <summary>
    /// If the shapeToGrid() method has been called: shows true
    /// </summary>
    public bool HasCommitedToGrid { get { return hasCommitedToGrid; } }
    private bool hasCommitedToGrid = false;

    /// <summary>
    /// Put the shape in Hold
    /// </summary>
    public bool toHold = false;
    protected bool returnedFromHold = false;

    // Score Modifiers (HardDropped and CellsDroppedBonus)
    public bool HardDropped { get { return hardDropped; } }
    protected bool hardDropped = false;
    
    // Bool to turn on softdropping bonus counting
    protected bool IsSoftDropping = false;
    public int CellsDroppedBonus { get { return cellsDroppedBonus; } }
    protected int cellsDroppedBonus = 0;

    // Highest and Lowest point of the block after commiting
    public int HighestPoint { get { return highestPoint; } }
    public int LowestPoint { get { return lowestPoint; } }
    protected int highestPoint = 0;
    protected int lowestPoint = 0;

    
    // What speed the ball should fall
    protected int framesPerCell;

    // How long it takes for the block to commit to grid
    int lockDelay = 100; // Milliseconds

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

    // Timing system
    protected TimeHelper timeHelper;

    /// <summary>
    /// Set Targeted Grid and Speed
    /// </summary>
    /// <param name="targetGrid"></param>
    /// <param name="location"></param>
    /// <param name="framesPerCell"></param>
    public TetrisBlock(TetrisGrid targetGrid, int framesPerCell)
    {
        // sets the target grid and starting values
        this.targetGrid = targetGrid;
        gridMatrix = targetGrid.gridMatrix;
        this.framesPerCell = framesPerCell;
        sprite = TetrisGame.ContentManager.Load<Texture2D>("block");
        rotation = 0;
        timeHelper = new TimeHelper();
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
            if (!MoveTest(shape, location)[2])
            {
                cellsDroppedBonus = y - start;
                hardDropped = true;
                lockDelay = 50;
                break;
            }
        }
    }

    /// <summary>
    /// Returns a bool array of length 4 defined as such: 
    /// [0] = Up, [1] = Right, [2] = Down, [3] = Left
    /// True if it can move in that direction, false if not.
    /// </summary>
    protected bool[] MoveTest(bool[,] testshape, Point location)
    {
        // sets all values to true
        bool[] canMoveTo = Enumerable.Repeat<bool>(true, 4).ToArray(); 

        // goes through the shape at it's given location
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
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (shape[y , x])
                {
                    targetGrid.gridMatrix[y + location.Y, x + location.X] = true;
                    targetGrid.colorMatrix[y + location.Y, x + location.X] = color;
                    lowestPoint = location.Y + y;
                }
            }
        }
        hasCommitedToGrid = true;
    }

    /// <summary>
    /// Does the stuff to make sure when returning from hold the block resets properly
    /// </summary>
    public void ReturnFromHold()
    {
        timeHelper.TimerReset();
        location = startlocation;
        returnedFromHold = true;
    }

    /// <summary>
    /// does the gravity
    /// </summary>
    protected void Gravity()
    {
        if (timeHelper.TimerHasReachedFrames(framesPerCell))
        {
            if (MoveTest(shape, location)[2])
            {
                location.Y += 1;
                if (IsSoftDropping) cellsDroppedBonus += 1;
                timeHelper.TimerReset();
            }
            else if (timeHelper.TimerHasReached(lockDelay))
            {
                CommitToGrid();
            }
        }
    }

    /// <summary>
    /// Resets rotation
    /// </summary>
    /// <param name="rotation"></param>
    public void ResetRotation()
    {
        while (rotation > 0)
        {
            shape = SuperRotationSystem.RotateShape(SuperRotationSystem.Direction.CounterClockwise, shape, size);
            rotation--;
        }
    }

    /// <summary>
    /// Controls for moving the Block
    /// </summary>
    /// <param name="inputHelper"></param>
    public void InputHandler(InputHelper inputHelper, bool started)
    {
        if (!timeHelper.paused)
        {
            // Movement

            // move left
            if (inputHelper.KeyPressed(Keys.A))
            {
                if (MoveTest(shape, location)[3]) location.X -= 1;
                if (!MoveTest(shape, location)[2] && (MoveTest(shape, location)[1] || MoveTest(shape, location)[3])) timeHelper.TimerReset();
            }

            // move right
            if (inputHelper.KeyPressed(Keys.D))
            {
                if (MoveTest(shape, location)[1]) location.X += 1;
                if (!MoveTest(shape, location)[2] && (MoveTest(shape, location)[1] || MoveTest(shape, location)[3])) timeHelper.TimerReset();
            }

            // Double Gravity when S is held
            if (inputHelper.KeyPressed(Keys.S))
            {
                IsSoftDropping = true;
                framesPerCell /= 2;
            }

            if (inputHelper.KeyReleased(Keys.S))
            {
                IsSoftDropping = false;
                framesPerCell *= 2;
            }



            // Non-movement actions

            // Move Block to the lowest possible position, gives bonus points for each cell dropped
            if (inputHelper.KeyPressed(Keys.Space) && !hardDropped)
            {
                HardDrop();
                timeHelper.TimerReset();
            }

            // Rotate Clockwise
            if (inputHelper.KeyPressed(Keys.E))
            {
                // perform rotation
                rotationSystem.PerformRotate(SuperRotationSystem.Direction.Clockwise, gridMatrix, shape, location, size, rotation);

                // keep track of rotation state
                if (rotation == 3) rotation = 0;
                else rotation++;

                // if it can't move down, give grace time
                if (!MoveTest(shape, location)[2]) timeHelper.TimerReset();
            }

            // Rotate CounterClockwise
            if (inputHelper.KeyPressed(Keys.Q))
            {
                //perform rotation
                rotationSystem.PerformRotate(SuperRotationSystem.Direction.CounterClockwise, gridMatrix, shape, location, size, rotation);

                // keep track of rotation state
                if (rotation == 0) rotation = 3;
                else rotation--;

                // if it can't move down, give grace time
                if (!MoveTest(shape, location)[2]) timeHelper.TimerReset();
            }

            // Put the piece in Hold
            if (inputHelper.KeyPressed(Keys.F) && !returnedFromHold)
            {
                // reset rotation so it can fit into the spawn area
                ResetRotation();
                toHold = true;
            }
        }

        if (inputHelper.KeyPressed(Keys.Escape) && started)
        {
            timeHelper.Pause();
        }
    }

    /// <summary>
    /// Does Physics stuff
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        timeHelper.Update(gameTime);
        Gravity();
    }
    
    /// <summary>
    ///  Draws ghost block and block
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        Vector2 midScreen = new Vector2((TetrisGame.ScreenSize.X / 2), (TetrisGame.ScreenSize.Y / 2));
        Vector2 gridPosition = targetGrid.Position;
        Vector2 stringSizePaused = font.MeasureString("Paused");

        // Calculate lowest position for the ghostblock
        int ghostblockY = 0;
        for (int y = 0; y < targetGrid.Height; y++)
        {
            if (!MoveTest(shape, new Point(location.X, location.Y + y))[2]) 
            {
                ghostblockY = location.Y + y;
                break;
            } 
        }

        // Actual drawing
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (shape[y, x] && location.Y + y >= 0)
                {
                    // draw ghost block
                    spriteBatch.Draw(sprite, new Vector2(gridPosition.X + ((x + location.X) * sprite.Width), gridPosition.Y + ((y + ghostblockY) * sprite.Height)), new Color(0, 0, 0, 100));
                    // draw block
                    spriteBatch.Draw(sprite, new Vector2(gridPosition.X + ((x + location.X) * sprite.Width), gridPosition.Y + ((y + location.Y) * sprite.Height)), color);
                    // if paused, say so
                    if (timeHelper.paused) spriteBatch.DrawString(font, "Paused", new Vector2(midScreen.X - stringSizePaused.X / 2, midScreen.Y), Color.White);
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
    public T(TetrisGrid targetGrid, int framesPerCell)
        : base( targetGrid, framesPerCell)
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
    public L(TetrisGrid targetGrid, int baseFallSpeed)
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
    public J(TetrisGrid targetGrid, int baseFallSpeed)
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
    public I(TetrisGrid targetGrid, int baseFallSpeed)
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
    public O(TetrisGrid targetGrid, int baseFallSpeed)
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
    public S(TetrisGrid targetGrid, int baseFallSpeed)
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
    public Z(TetrisGrid targetGrid, int framesPerCell)
        : base(targetGrid, framesPerCell)
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