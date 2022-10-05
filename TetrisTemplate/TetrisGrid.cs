using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
class TetrisGrid
{
    /// The sprite of a single empty cell in the grid.
    Texture2D emptyCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    private static int width = 10;

    private static int height = 20;
    
    bool[,] gridMatrix = new bool[height, width];

    /// The number of grid elements in the x-direction.
    public int Width { get { return width; } }
   
    /// The number of grid elements in the y-direction.
    public int Height { get { return height; } }

    /// <summary>
    /// Creates a new TetrisGrid.
    /// </summary>
    /// <param name="b"></param>
    public TetrisGrid()
    {
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = Vector2.Zero;
        Clear();
    }

    

    /// <summary>
    /// Draws the grid on the screen.
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (gridMatrix[i,j])
                {
                    spriteBatch.Draw(emptyCell, new Vector2(position.X + (j * emptyCell.Width), position.Y + (i * emptyCell.Height)), Color.Red);
                }
                else
                {
                    spriteBatch.Draw(emptyCell, new Vector2(position.X + (j * emptyCell.Width), position.Y + (i * emptyCell.Height)), Color.White);
                }
                
            }
        }
    }

    /// <summary>
    /// Clears the grid.
    /// </summary>
    public void Clear()
    {
        gridMatrix = new bool[height, width];
    }
}

