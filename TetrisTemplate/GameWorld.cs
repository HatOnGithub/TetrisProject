using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;

/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
    private int a = 0;
    private int b = 0;

    const int basefallspeed = 4;

    Point startspawn;

    int shapeSize = 4;
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
    /// Tetris blocks
    /// </summary>
    TetrisBlock block;

    NextUpGrid nextUpGrid;
    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;

        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");

        grid = new TetrisGrid();

        nextUpGrid = new NextUpGrid();

        startspawn = new Point((grid.Width - shapeSize) / 2 , shapeSize * -1);

        NewRandomBlock();

        Reset();
    }

    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        block.InputHandler(inputHelper);
    }

    public void Update(GameTime gameTime)
    {
        block.Update(gameTime);
        if (block.HasCommitedToGrid) NewRandomBlock();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        grid.Draw(gameTime, spriteBatch);
        nextUpGrid.Draw(gameTime, spriteBatch);
        block.Draw(gameTime, spriteBatch);
        spriteBatch.End();
    }

    public void NewRandomBlock()
    {
        int i = random.Next(1, 7);
        switch (i)
        {
            case 1: block = new T(grid, startspawn, basefallspeed);
                break;
            case 2: block = new L1(grid, startspawn, basefallspeed);
                break;
            case 3: block = new L2(grid, startspawn, basefallspeed);
                break;
            case 4: block = new Long(grid, startspawn, basefallspeed);
                break;
            case 5: block = new SQ(grid, startspawn, basefallspeed);
                break;
            case 6: block = new D1(grid, startspawn, basefallspeed);
                break;
            case 7: block = new D2(grid, startspawn, basefallspeed);
                break;
        }

    }

    public void Reset()
    {
        nextUpGrid.Reset();
        grid.Clear();
    }

}
