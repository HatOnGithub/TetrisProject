using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

public class NextUpGrid
{
	int a;
	int b;
    bool[,] shape1;
    bool[,] shape2;

    Texture2D emptyCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    private const int width = 4;

    private const int height = 9;

    public bool[,] gridMatrix = new bool[height, width];
    Color[,] colorMatrix = new Color[height, width];

    public NextUpGrid()
	{
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = new Vector2(TetrisGame.ScreenSize.X / 2 + 7 * emptyCell.Width, TetrisGame.ScreenSize.Y / 2 - 10 * emptyCell.Height);
        Reset();
    }
    public void Update(GameTime gameTime)
    {
        switch (a)
        {
            case 1:
                break;
            case 2:
                break;
        }
    }
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (gridMatrix[i, j])
                {
                    spriteBatch.Draw(emptyCell, new Vector2(position.X + (j * emptyCell.Width), position.Y + (i * emptyCell.Height)), colorMatrix[i, j]);
                }
                

            }
        }
    }
    public void Reset()
    {
        gridMatrix = new bool[height, width];
        var random = new Random();
        if (a == 0)
        {
            a = random.Next(1, 7);
        }
        a = b;
        b = random.Next(1, 7);
    }

}
