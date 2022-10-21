using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

    // the sound for clearing a line of blocks
    protected SoundEffect lineclear;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    private const int width = 10;

    private const int height = 22;
    
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
        position = new Vector2(TetrisGame.ScreenSize.X/2 - width/2 * emptyCell.Width, TetrisGame.ScreenSize.Y/2 - height/2 * emptyCell.Height);
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
    /// Returns the amount of full lines and clears the lines off the grid
    /// </summary>
    public int FullLines(int lowestPointOfBlock) 
    {
        int l = 0;
        List<int> linesToClear = new List<int>();
        for (int y = lowestPointOfBlock; y >= 0; y--) 
        {
            if (IsFilled(y))
            {
                l++;
                linesToClear.Add(y);
            }
        }
        if (l > 0) ClearLines(linesToClear);
        return l;
    }

    /// <summary>
    /// Clears the lines given as parameter and moves lines above it down
    /// </summary>
    /// <param name="lines"></param>
    public void ClearLines(List<int> lines)
    {
        for (int i = 0; i < lines.Count && lines.Count > 0; i++)
        {
            for (int y = lines[i]; y >= 0; y--)
            {
                if (y == lines[i])
                {
                    for (int x = 0; x < width; x++) 
                    {
                        gridMatrix[y, x] = false;
                        colorMatrix[y, x] = Color.White;
                    }
                    continue;
                }
                for (int x = 0; x < width; x++)
                {
                    
                    gridMatrix[y + 1, x] = gridMatrix[y,x];
                    colorMatrix[y + 1, x] = colorMatrix[y, x];
                    gridMatrix[y, x] = false;
                    colorMatrix[y, x] = Color.White;
                }
            }
            if (i < lines.Count - 1 && i >= 0) lines[i + 1] += i + 1;
        }
    }

    protected bool IsFilled(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (!gridMatrix[y, x]) return false;
        }
        return true;
    }


   
}

