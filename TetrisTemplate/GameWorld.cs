using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
    int score = 0;

    int level = 0;

    int totalLinesCleared = 0;

    int framesPerCell;

    public bool started = false;

    bool cannotSpawn = false;

    /// <summary>
    /// An enum for the different game states that the game can have.
    /// </summary>
    enum GameState
    {
        Playing,
        GameOver
    }

    /// <summary>
    /// The random-number generator of the game.
    /// </summary>
    public static Random Random { get { return random; } }
    static Random random;

    /// <summary>
    /// The main font of the game.
    /// </summary>
    SpriteFont font;

    /// <summary>
    /// The current game state.
    /// </summary>
    GameState gameState;

    /// <summary>
    /// The main grid of the game.
    /// </summary>
    TetrisGrid grid;

    /// <summary>
    /// Current block
    /// </summary>
    TetrisBlock block;

    /// <summary>
    /// Next block in queue
    /// </summary>
    TetrisBlock nextblock;

    /// <summary>
    /// Block being held
    /// </summary>
    TetrisBlock holdblock;
    /// <summary>
    /// Smaller grid for displaying the next 
    /// </summary>
    NextUpGrid nextUpGrid;

    // play the sound for clearing a full line on blocks
    protected SoundEffect lineclear;

    // sound for locking shape to grid
    protected SoundEffect locksound;

    // background music
    protected Song backgroundmusic;

    /// <summary>
    /// Hey, you called me?
    /// </summary>
    public GameWorld()
    {
        random = new Random();

        gameState = GameState.Playing;

        grid = new TetrisGrid(new Vector2(TetrisGame.ScreenSize.X / 2 - 5 * 30, TetrisGame.ScreenSize.Y / 2 - 11 * 30));

        nextUpGrid = new NextUpGrid();
        
        HandleLevels();

        nextblock = NewRandomBlock();

        Reset(false);
    }

    public void LoadContent()
    {
        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        lineclear = TetrisGame.ContentManager.Load<SoundEffect>("lineclear");

        locksound = TetrisGame.ContentManager.Load<SoundEffect>("lockblock");

        backgroundmusic = TetrisGame.ContentManager.Load<Song>("backgroundmusic");

        MediaPlayer.IsRepeating = true;

        MediaPlayer.Play(backgroundmusic);

        MediaPlayer.Volume = 0.5f;
    }

    /// <summary>
    /// Does Input Stuff
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="inputHelper"></param>
    public void HandleInput(InputHelper inputHelper)
    {
        if (started) block.InputHandler(inputHelper, started);
        if (inputHelper.KeyPressed(Keys.Space) && !started)
        {
            if (gameState == GameState.GameOver)
            {
                gameState = GameState.Playing;
                Reset(true);
            }
            else
            {
                started = true;
                CycleBlock();
            }
        }
    }


    /// <summary>
    /// Does Checks and stuff
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        switch (gameState)
        {
            case GameState.Playing:
                if (started && block != null)
                {
                    if (block.toHold) HoldSequence();

                    if (block.HasCommitedToGrid)
                    {
                        locksound.Play();
                        HandleScore();
                        HandleLevels();
                        CycleBlock();
                    }

                    if (cannotSpawn) 
                    {
                        gameState = GameState.GameOver;
                        cannotSpawn = false;
                        started = false;
                        holdblock = null;
                        block = null;
                        nextblock = null;
                        break;
                    } 

                    block.Update(gameTime);
                }
                break;
        }
    }


    /// <summary>
    /// Does Drawing Stuff
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        Vector2 midScreen = new Vector2((TetrisGame.ScreenSize.X / 2), (TetrisGame.ScreenSize.Y / 2));
        Vector2 stringSizeLevel = font.MeasureString("Level: " + level.ToString());
        Vector2 stringSizeStart = font.MeasureString("Press <Space> to Start!");
        string[] gameText = new string[] { "Score: " + score.ToString(), "Level: " + level.ToString()};
        string[] gameOverText = new string[] {"Game over", "Level: " + level.ToString(), "Score: " + score.ToString()};

        switch (gameState)
        {
            case GameState.Playing:
                grid.Draw(spriteBatch);
                nextUpGrid.Draw(spriteBatch, font);

                spriteBatch.DrawString(font, "Score: " + score.ToString(), Vector2.Zero, Color.White);
                spriteBatch.DrawString(font, "Level: " + level.ToString(), new Vector2(0, stringSizeLevel.Y), Color.White);
                if (!started) spriteBatch.DrawString(font, "Press <Space> to Start!", new Vector2(midScreen.X - stringSizeStart.X / 2, midScreen.Y), Color.White);
                if (started) block.Draw(spriteBatch, font);
                break;
            case GameState.GameOver:
                for (int i = 0; i < gameOverText.Length; i++)
                {
                    Vector2 l = new Vector2(midScreen.X - font.MeasureString(gameOverText[i]).X / 2, midScreen.Y + i * font.MeasureString(gameOverText[i]).Y);
                    spriteBatch.DrawString(font, gameOverText[i], l, Color.White);
                }
                break;
        }
        spriteBatch.End();
    }

    // ------------------------------------------------------------------------------------------------


    /// <summary>
    /// Swaps the current block with the next block in queue after the current block has commited to grid
    /// Also updates the score depending on the amount of lines cleared, if any
    /// </summary>
    public void CycleBlock()
    {
        // checks if block can spawn, if it can't, it resets the board
        if (started && (grid.gridMatrix[0, 3] || grid.gridMatrix[0, 4] || grid.gridMatrix[0, 5] ||
            grid.gridMatrix[1, 3] || grid.gridMatrix[1, 4] || grid.gridMatrix[1, 5] || grid.gridMatrix[1, 6]))
        {
            cannotSpawn = true;
        }

        // normal cycling of blocks
        else
        {
            block = nextblock;
            nextblock = NewRandomBlock();
            nextUpGrid.Refresh(nextblock, holdblock);
        }
    }

    /// <summary>
    /// Returns a new Random block
    /// </summary>
    /// <returns></returns>
    public TetrisBlock NewRandomBlock()
    {
        int i = random.Next(1, 8);
        switch (i)
        {
            case 1: return new T(grid, framesPerCell);
            case 2: return new L(grid, framesPerCell);
            case 3: return new J(grid, framesPerCell);
            case 4: return new I(grid, framesPerCell);
            case 5: return new O(grid, framesPerCell);
            case 6: return new S(grid, framesPerCell);
            case 7: return new Z(grid, framesPerCell);
            default: return null;
        }
    }

    /// <summary>
    /// Calculates score based on level, lines cleared and other bonuses like soft and hard drops
    /// </summary>
    public void HandleScore()
    {
        int l = grid.FullLines(block.LowestPoint, block.HighestPoint);
        if (l > 0)
        {
            switch (l)
            {
                case 1:
                    score += 40 * (level + 1);
                    break;
                case 2:
                    score += 100 * (level + 1);
                    break;
                case 3:
                    score += 300 * (level + 1);
                    break;
                case 4:
                    score += 1200 * (level + 1);
                    break;
            }
            lineclear.Play();
        }
        totalLinesCleared += l;
        if (block.HardDropped) score += block.CellsDroppedBonus * 2;
        else score += block.CellsDroppedBonus;
    }

    /// <summary>
    /// Keeps a record of lines cleared, lines per level and current level
    /// </summary>
    public void HandleLevels()
    {
        int[] LinesBeforeIncrease = new int[30] 
        {5, 10, 15, 20, 25, 30, 35, 40, 45, 50,
         50, 50, 50, 50, 50, 50, 55, 60, 65, 70,
         75, 80, 85, 90, 95, 100, 100, 150, 200, -1};

        int[] speedPerLevel = new int[30]
        {48, 43, 38, 33, 28, 23, 18, 13, 8, 6,
         5, 5, 4, 4, 4, 3, 3, 3, 3, 2,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2};

        if (totalLinesCleared >= LinesBeforeIncrease[level] && LinesBeforeIncrease[level] != -1)
        {
            totalLinesCleared -= LinesBeforeIncrease[level];
            level += 1;
        }
        framesPerCell = speedPerLevel[level];
    }

    /// <summary>
    /// Does the Hold Piece stuff
    /// </summary>
    public void HoldSequence()
    {
        if (holdblock != null)
        {
            TetrisBlock temp;
            temp = holdblock;
            temp.toHold = false;
            temp.ReturnFromHold();
            holdblock = block;
            block = temp;
            nextUpGrid.Refresh(nextblock, holdblock);
        }
        else
        {
            holdblock = block;
            block = null;
            CycleBlock();
        }
    }

    /// <summary>
    /// Clears the board
    /// </summary>
    public void Reset(bool restarting)
    {

        grid.Clear();
        if (restarting)
        {
            started = false;
            cannotSpawn = false;
            score = 0;
            level = 0;
            block = null;
            holdblock = null;   
            nextblock = null;
        }
        nextblock = NewRandomBlock();
        nextUpGrid.Refresh(nextblock, holdblock);
    }
}
