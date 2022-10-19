using Microsoft.Xna.Framework;
using System;
using System.Data.Common;



/// <summary>
/// This Rotation System is Insipired by the original SRS from the original NES Tetris
/// </summary>


public class SuperRotationSystem
{
	public enum Direction { Clockwise, CounterClockwise }
	
	TetrisGrid targetGrid;
	TetrisBlock targetBlock;
	Point location;
	bool[,] gridMatrix;

	public SuperRotationSystem(TetrisBlock targetBlock, TetrisGrid targetGrid)
	{
		
		this.targetGrid = targetGrid;
		this.targetBlock = targetBlock;
	}
	/// <summary>
	/// Returns True if the shape will collide with the wall or other shapes given the offset
	/// </summary>
	/// <param name="rotatedShape"></param>
	/// <param name="testOffset"></param>
	/// <returns></returns>
	protected bool WallKickValid(bool[,] rotatedShape, int size, Point testOffset)
	{
		for (int y = 0; y < size; y++)
		{
			for(int x = 0; x < size; x++)
			{
				// checks if out of bounds
				if ((location.X + x + testOffset.X < 0 || location.X + x + testOffset.X >= targetGrid.Width || location.Y + y + testOffset.Y >= targetGrid.Height) && 
					rotatedShape[y, x])
					return false;
				// checks if overlapping with shapes already on grid
				else if (location.Y + y + testOffset.Y >= 0 && location.Y + y + testOffset.Y < targetGrid.Height &&
                    location.X + x + testOffset.X >= 0 && location.X + x + testOffset.X < targetGrid.Width)
				{
                    if (rotatedShape[y, x] && gridMatrix[location.Y + y + testOffset.Y, location.X + x + testOffset.X]) return false;
                }
			}
		}
		return true;
	}

	/// <summary>
	/// Offsets the shape by checking around the shape
	/// </summary>
	public void PerformRotate(Direction dir, bool[,] currentGrid, bool[,] shape, Point location, int size, int rotation)
	{
		// updates the variables
		gridMatrix = currentGrid;
		this.location = location;

		// Returns list of offsets to attempt
        Point[] testList = TestList(size, rotation, dir);

        // Returns a rotated shape
        bool[,] rotated = RotateShape(dir, shape, size);
		
        
		// checks wallkick offsets
		for (int i = 0; i < testList.Length; i++)
		{
			if (WallKickValid(rotated, size, testList[i]))
			{
				if (dir == Direction.Clockwise && rotation < 3) targetBlock.Rotation += 1;
				else if (dir == Direction.Clockwise) targetBlock.Rotation = 0;

				if (dir == Direction.CounterClockwise && rotation > 0) targetBlock.Rotation -= 1;
				else if (dir == Direction.CounterClockwise) targetBlock.Rotation = 3;
                targetBlock.Location += testList[i];
                targetBlock.Shape = rotated;
				break;
			}
		}
    }

	public bool[,] RotateShape(Direction dir, bool[,] shape, int size)
	{
		bool[,] result = new bool[2, 2] { { true, true },
                                          { true, true } };
        if (dir == Direction.Clockwise && size != 2)
		{
            switch (size)
            {
                case 3:
                    result = new bool[3, 3]{
                {shape[2,0], shape[1,0], shape[0,0] },
                {shape[2,1], shape[1,1], shape[0,1] },
                {shape[2,2], shape[1,2], shape[0,2] }};
                    break;

                case 4:
                    result = new bool[4, 4] {
                {shape[3, 0], shape[2, 0], shape[1, 0], shape[0, 0] },
                {shape[3, 1], shape[2, 1], shape[1, 1], shape[0, 1] },
                {shape[3, 2], shape[2, 2], shape[1, 2], shape[0, 2] },
                {shape[3, 3], shape[2, 3], shape[1, 3], shape[0, 3] }};
                    break;
			}
		}

        if (dir == Direction.CounterClockwise && size != 2)
		{
            switch (size)
            {
                case 3:
                    result = new bool[3, 3] {
                {shape[0, 2], shape[1, 2], shape[2, 2]},
                {shape[0, 1], shape[1, 1], shape[2, 1]},
                {shape[0, 0], shape[1, 0], shape[2, 0]}};
                    break;

                case 4:
                    result = new bool[4, 4] {
                {shape[0, 3], shape[1, 3], shape[2, 3], shape[3, 3] },
                {shape[0, 2], shape[1, 2], shape[2, 2], shape[3, 2] },
                {shape[0, 1], shape[1, 1], shape[2, 1], shape[3, 1] },
                {shape[0, 0], shape[1, 0], shape[2, 0], shape[3, 0] }};
                    break;
            }
        }
		return result;
    }

	/// <summary>
	/// Returns list of tests to perform,
	/// The test series originates from the Original NES Tetris Super Rotation System
	/// </summary>
	/// <param name="size"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	protected Point[] TestList(int size, int rotation, Direction dir)
	{
		Point[] list;
		if (size == 2) list = new Point[1];
		else list = new Point[5];
        list[0] = new Point(0, 0);
        switch (size)
        {
            case 3:
				switch (rotation)
				{
					case 0:
						list[1] = new(-1, 0); list[2] = new(-1, -1); list[3] = new(0, 2); list[4] = new(-1, 2);
						break;
					case 1:
						list[1] = new(1, 0); list[2] = new(1, 1); list[3] = new(0, -2); list[4] = new(1, -2);
						break;
					case 2:
						list[1] = new(1, 0); list[2] = new(1, -1); list[3] = new(0, 2); list[4] = new(1, 2);
						break;
					case 3:
						list[1] = new(-1, 0); list[2] = new(-1, 1); list[3] = new(0, -2); list[4] = new(-1, -2);
						break;
				}
                break;
            case 4:
                switch (rotation)
                {
					case 0:
						list[1] = new(-2, 0); list[2] = new(1, 0); list[3] = new(-2, 1); list[4] = new(1, -2);
						break;
					case 1:
						list[1] = new(-1, 0); list[2] = new(2, 0); list[3] = new(-1, -2); list[4] = new(2, 1);
						break;
					case 2:
						list[1] = new(2, 0); list[2] = new(-1, 0); list[3] = new(2, -1); list[4] = new(-1, 2);
						break;
					case 3:
						list[1] = new(1, 0); list[2] = new(-2, 0); list[3] = new(1, 2); list[4] = new(-2, -1);
                        break;
                }
                break;
        }
		if (dir == Direction.CounterClockwise)
		{
			for (int i = 1; i < list.Length; i++)
			{
				list[i].X *= -1; list[i].Y *= -1;
			}
		}
		return list;
    }
}
