using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    const int basefallspeed = 4;

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
    /// Smaller grid for displaying the next 
    /// </summary>
    NextUpGrid nextUpGrid;

    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;

        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        grid = new TetrisGrid();

        nextUpGrid = new NextUpGrid();

        nextblock = NewRandomBlock();

        Reset();
    }

    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        if (started) block.InputHandler(inputHelper);
        if (inputHelper.KeyPressed(Keys.Space) && !started)
        {
            CycleBlock();
        }
    }

    public void Update(GameTime gameTime)
    {
        switch (gameState)
        {
            case GameState.Playing:
                if (started)
                {
                    if (block.HasCommitedToGrid)
                    {
                        HandleScore();
                        HandleLevels();
                    }
                    if (cannotSpawn) 
                    {
                        started = false;
                        block = null;
                        cannotSpawn = false;
                        score = 0;
                        level = 0;
                        break;
                    } 
                    block.Update(gameTime);
                }
                break;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        switch (gameState)
        {
            case GameState.Playing:
                grid.Draw(gameTime, spriteBatch);
                nextUpGrid.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(font, "Score: " + score.ToString(), Vector2.Zero, Color.White);
                if (started) block.Draw(gameTime, spriteBatch);

                break;
            case GameState.GameOver:
                Vector2 midScreen = new Vector2((TetrisGame.ScreenSize.X / 2), (TetrisGame.ScreenSize.Y / 2));
                Vector2 stringSizeGameOver = font.MeasureString("Game Over");
                Vector2 stringSizeLevel = font.MeasureString("Level: " + level.ToString());
                Vector2 stringSizeScore = font.MeasureString("Score: " + score.ToString());
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
        nextUpGrid.Refresh(nextblock);
    }

    public TetrisBlock NewRandomBlock()
    {
        TetrisBlock result = null;
        int i = random.Next(1, 8);
        switch (i)
        {
            case 1: result = new T(grid, basefallspeed);
                break;
            case 2: result = new L(grid, basefallspeed);
                break;
            case 3: result = new J(grid, basefallspeed);
                break;
            case 4: result = new I(grid, basefallspeed);
                break;
            case 5: result = new O(grid, basefallspeed);
                break;
            case 6: result = new S(grid, basefallspeed);
                break;
            case 7: result = new Z(grid, basefallspeed);
                break;
        }
        return result;
    }

    public void HandleScore()
    {
        int l = grid.FullLines(block.lowestPoint);
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

        }
        totalLinesCleared += l;
        if (block.HardDropped) score += block.CellsDroppedBonus * 2;
        else score += block.CellsDroppedBonus;
        CycleBlock();
    }

    public void HandleLevels()
    {

    }

    public void Reset()
    {
        nextUpGrid.Refresh(nextblock);
        grid.Clear();
    }

}
