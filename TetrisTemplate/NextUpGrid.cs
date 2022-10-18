using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class NextUpGrid
{

    Texture2D emptyCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    private const int width = 4;

    private const int height = 4;

    public bool[,] gridMatrix = new bool[height, width];
    Color[,] colorMatrix = new Color[height, width];

    public NextUpGrid()
    {
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = new Vector2(TetrisGame.ScreenSize.X / 2 + 7 * emptyCell.Width, TetrisGame.ScreenSize.Y / 2 - 10 * emptyCell.Height);
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
    public void Refresh(TetrisBlock nextblock)
    {
        gridMatrix = new bool[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x < nextblock.Size && y < nextblock.Size)
                {
                    if (nextblock.Shape[y, x])
                    {
                        gridMatrix[y, x] = true;
                        colorMatrix[y, x] = nextblock.Color;
                    }
                }
            }
        }
    }
}
