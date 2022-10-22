using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System;

public class NextUpGrid
{

    Texture2D emptyCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    // dimensions of the second grid
    private const int width = 4;

    private const int height = 8;

    // relevant grids
    bool[,] gridMatrix = new bool[height, width];

    Color[,] colorMatrix = new Color[height, width];

    public NextUpGrid()
    {
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = new Vector2(TetrisGame.ScreenSize.X / 2 + 7 * emptyCell.Width, TetrisGame.ScreenSize.Y / 2 - 10 * emptyCell.Height + 10);
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        DrawMatrix(spriteBatch);
        DrawText(spriteBatch, font);
    }

    

    private void DrawMatrix(SpriteBatch spriteBatch)
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

    private void DrawText(SpriteBatch spriteBatch, SpriteFont font)
    {
        string[] controls = { "[A] & [D] = LEFT & RIGHT", "[Q] & [E] = ROTATE", "[S] = INCREASE GRAVITY", "[Space] = HARD DROP ", "[F] = HOLD", "[Esc] PAUSE GAME" };
        float stringHeight = font.MeasureString(controls[0]).Y + 5;
        for (int i = 0; i < controls.Length; i++)
        {
            spriteBatch.DrawString(font, controls[i], new Vector2(position.X + width * emptyCell.Width + 10, position.Y + i * stringHeight), Color.White);
        }

        Vector2 next = font.MeasureString("Next Block");
        Vector2 hold = font.MeasureString("In Holding");
        float midGrid = position.X + (width * emptyCell.Width / 2);
        spriteBatch.DrawString(font, "Next Block", new Vector2(midGrid - next.X / 2, position.Y - next.Y - 10), Color.White);
        spriteBatch.DrawString(font, "In Holding", new Vector2(midGrid - hold.X / 2, position.Y + hold.Y + height * emptyCell.Height + 2), Color.White);
    }

    public void Refresh(TetrisBlock nextblock, TetrisBlock holdBlock)
    {
        // new blank matrix
        gridMatrix = new bool[height, width];

        // populate matrix with new data 
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // set the next block in the queue
                if (y < 4 && x < nextblock.size && y < nextblock.size)
                {
                    if (nextblock.shape[y, x])
                    {
                        gridMatrix[y, x] = true;
                        colorMatrix[y, x] = nextblock.color;
                    }
                }

                // set the block in holding
                if (y > 3 && holdBlock != null && y - 4 < holdBlock.size && x < holdBlock.size)
                {
                    if (holdBlock.shape[y - 4, x])
                    {
                        gridMatrix[y, x] = true;
                        colorMatrix[y, x] = holdBlock.color;
                    }
                }
            }
        }
    }
}
