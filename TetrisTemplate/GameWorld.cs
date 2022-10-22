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

    int basefallspeed = 48;

    int framesPerCell;

    bool started = false;

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

        framesPerCell = basefallspeed;

        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        lineclear = TetrisGame.ContentManager.Load<SoundEffect>("lineclear");

        locksound = TetrisGame.ContentManager.Load<SoundEffect>("lockblock");

        backgroundmusic = TetrisGame.ContentManager.Load<Song>("backgroundmusic");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(backgroundmusic);

        grid = new TetrisGrid(new Vector2(TetrisGame.ScreenSize.X / 2 - 5 * 30, TetrisGame.ScreenSize.Y / 2 - 11 * 30));

        nextUpGrid = new NextUpGrid();

        nextblock = NewRandomBlock();

        Reset();
    }

    /// <summary>
    /// Does Input Stuff
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="inputHelper"></param>
    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        if (started) block.InputHandler(inputHelper);
        if (inputHelper.KeyPressed(Keys.Space) && !started)
        {
            if (gameState == GameState.GameOver)
            {
                gameState = GameState.Playing;
            }
            else CycleBlock();
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
                if (started)
                {
                    if (block.ToHold)
                    {
                        if (holdblock != null)
                        {
                            TetrisBlock temp;
                            temp = holdblock;
                            temp.ToHold = false;
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
                    if (block.HasCommitedToGrid)
                    {
                        locksound.Play();
                        HandleScore();
                        HandleLevels();
                    }

                    if (cannotSpawn) 
                    {
                        gameState = GameState.GameOver;
                        block = null;
                        cannotSpawn = false;
                        score = 0;
                        level = 0;
                        break;
                    } 

                    block.Update(gameTime);
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    /// <summary>
    /// Does Drawing Stuff
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        Vector2 midScreen = new Vector2((TetrisGame.ScreenSize.X / 2), (TetrisGame.ScreenSize.Y / 2));
        Vector2 stringSizeGameOver = font.MeasureString("Game Over");
        Vector2 stringSizeLevel = font.MeasureString("Level: " + level.ToString());
        Vector2 stringSizeScore = font.MeasureString("Score: " + score.ToString());
        Vector2 stringSizeStart = font.MeasureString("Press <Space> to Start!");

        switch (gameState)
        {
            case GameState.Playing:
                grid.Draw(gameTime, spriteBatch);
                nextUpGrid.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(font, "Score: " + score.ToString(), Vector2.Zero, Color.White);
                spriteBatch.DrawString(font, "Level: " + level.ToString(), new Vector2(0, stringSizeLevel.Y), Color.White);
                if (!started) spriteBatch.DrawString(font, "Press <Space> to Start!", new Vector2(midScreen.X - stringSizeStart.X / 2, midScreen.Y), Color.White);
                if (started) block.Draw(gameTime, spriteBatch);
                break;
            case GameState.GameOver:
                
                spriteBatch.DrawString(font, "Game Over", new Vector2(midScreen.X - stringSizeGameOver.X/2, midScreen.Y - stringSizeGameOver.Y), Color.White);
                spriteBatch.DrawString(font, "Level: " + level.ToString(), new Vector2(midScreen.X - stringSizeLevel.X / 2 , midScreen.Y), Color.White);
                spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(midScreen.X - stringSizeScore.X / 2 , midScreen.Y + stringSizeScore.Y), Color.White);
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
            Reset();
            started = false;
            block = null;
            cannotSpawn = true;
        }
        // normal cycling of blocks
        else
        {
            block = nextblock;
            started = true;
        }
        nextblock = NewRandomBlock();
        nextUpGrid.Refresh(nextblock, holdblock);
    }

    /// <summary>
    /// Returns a new Random block
    /// </summary>
    /// <returns></returns>
    public TetrisBlock NewRandomBlock()
    {
        TetrisBlock result = null;
        int i = random.Next(1, 8);
        switch (i)
        {
            case 1: result = new T(grid, framesPerCell);
                break;
            case 2: result = new L(grid, framesPerCell);
                break;
            case 3: result = new J(grid, framesPerCell);
                break;
            case 4: result = new I(grid, framesPerCell);
                break;
            case 5: result = new O(grid, framesPerCell);
                break;
            case 6: result = new S(grid, framesPerCell);
                break;
            case 7: result = new Z(grid, framesPerCell);
                break;
        }
        return result;
    }

    /// <summary>
    /// Calculates score based on level, lines cleared and other bonuses
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

        CycleBlock();
    }

    /// <summary>
    /// Keeps a record of lines cleared, lines per level and current level
    /// </summary>
    public void HandleLevels()
    {
        int[] LinesBeforeIncrease = new int[30] 
        {10, 20, 30, 40, 50, 60, 70, 80, 90, 100,
         100, 100, 100, 100, 100, 100, 110, 120, 130, 140,
         150, 160, 170, 180, 190, 200, 200, 200, 200, -1};

        int[] speedPerLevel = new int[30]
        {48, 43, 38, 33, 28, 23, 18, 13, 8, 6,
         5, 5, 4, 4, 4, 3, 3, 3, 3, 2,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2};
        if (totalLinesCleared >= LinesBeforeIncrease[level] && LinesBeforeIncrease[level] != -1)
        {
            totalLinesCleared -= LinesBeforeIncrease[level];
            level += 1;
            framesPerCell = speedPerLevel[level];
        }
    }

    public void SaveHighScore()
    {

    }

    /// <summary>
    /// Clears the board
    /// </summary>
    public void Reset()
    {
        nextUpGrid.Refresh(nextblock, holdblock);
        grid.Clear();
    }

}
