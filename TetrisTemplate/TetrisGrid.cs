using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
public class TetrisGrid
{
    /// The sprite of a single empty cell in the grid.
    Texture2D emptyCell;
    /// the sprite of a filled cell on the grid
    Texture2D filledCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    private const int width = 10;

    private const int height = 20;
    
    public bool[,] gridMatrix = new bool[height, width];
    public Color[,] colorMatrix = new Color[height, width];

    /// The number of grid elements in the x-direction.
    public int Width { get { return width; } }
   
    /// The number of grid elements in the y-direction.
    public int Height { get { return height; } }

    /// current position of the grid origin
    public Vector2 Position { get { return position; } }


    /// <summary>
    /// Creates a new TetrisGrid.
    /// </summary>
    /// <param name="b"></param>
    public TetrisGrid()
    {
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("grid");
        filledCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = new Vector2(TetrisGame.ScreenSize.X/2 - 5 * emptyCell.Width, TetrisGame.ScreenSize.Y/2 - 10 * emptyCell.Height);
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
                    spriteBatch.Draw(filledCell, new Vector2(position.X + (j * emptyCell.Width), position.Y + (i * emptyCell.Height)), colorMatrix[i,j]);
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



    /// <summary>
    /// Returns the points gained depending on lines cleared and points gained
    /// Level should start at 0
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public int FullLines(int n, int lowestPointOfBlock) 
    {
        int l = 0;
        int result = 0;
        List<int> linesToClear = new List<int>();
        for (int y = lowestPointOfBlock; y >= 0; y--) 
        {
            for (int x = 0; x < width - 1; x++)
            {
                if (IsFilled(y))
                {
                    l++;
                    linesToClear.Add(y);
                }
                
            }
        }

        if (l > 0) 
        {
            switch (l)
            {
                case 1:
                    result = 40 * (n + 1);
                    break;
                case 2:
                    result = 100 * (n + 1);
                    break;
                case 3:
                    result = 300 * (n + 1);
                    break;
                case 4:
                    result = 1200 * (n + 1);
                    break;
            }
            ClearLines(linesToClear);
        } 

        return result;
    }

    public void ClearLines(IList<int> y)
    {

    }

    protected bool IsFilled(int y)
    {
        for (int x = 0; x < width - 1; x++)
        {
            if (gridMatrix[y, x]) return false;
        }
        return true;
    }


   
}

