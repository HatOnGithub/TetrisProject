using Microsoft.Xna.Framework.Input;
using System;
using System.Text;

public class TetrisBlock
{
    public bool[,] shape;

    
    public TetrisBlock(bool[,] shape)
    {

    }

    
	
    public static void InputHandler(InputHelper inputHelper)
    {
        if (inputHelper.KeyPressed(Keys.A))
        {

        }
    }

    public void RotateCW()
    {
        bool[,] a = shape;
        bool[,] rotated = new bool[4, 4] {
            {a[3, 0], a[2, 0], a[1, 0], a[0, 0] },
            {a[3, 1], a[2, 1], a[1, 1], a[0, 1] },
            {a[3, 2], a[2, 2], a[1, 2], a[0, 2] },
            {a[3, 3], a[2, 3], a[1, 3], a[0, 3] }};
        this.shape = rotated;

    }
    public void RotateCCW()
    {
        bool[,] a = shape;
        bool[,] rotated = new bool[4, 4] {
            {a[0, 3], a[1, 3], a[2, 3], a[3, 3] },
            {a[0, 2], a[1, 2], a[2, 2], a[3, 2] },
            {a[0, 1], a[1, 1], a[2, 1], a[3, 1] },
            {a[0, 0], a[1, 0], a[2, 0], a[3, 0] }};
        this.shape = rotated;
        
    }
}
/// <summary>
///  T Block 
/// </summary>
class T : TetrisBlock
{
    public T(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , false, false},

                {false, true , true , false},

                {false, true , false, false}};
    }
}
/// <summary>
/// L Block
/// </summary>
class L1 : TetrisBlock
{
    public L1(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, true , false, false},

                {false, true , false, false}};
    }
}
/// <summary>
/// Flipped L Block
/// </summary>
class L2 : TetrisBlock
{
    public L2(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, false, true , false},

                {false, false, true , false}};
    }
}
/// <summary>
/// Long BLock
/// </summary>
class Long : TetrisBlock
{
    public Long(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, true , false, false},

                {false, true , false, false},

                {false, true , false, false},

                {false, true , false, false}};
    }
}
/// <summary>
/// Square Block
/// </summary>
class SQ : TetrisBlock
{
    public SQ(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, true , true , false},

                {false, false, false, false}};
    }
}

/// <summary>
/// Diagonal Block with left high
/// </summary>
class D1 : TetrisBlock
{
    public D1(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , false, false},

                {false, true , true , false},

                {false, false, true , false}};
    }
}
/// <summary>
/// Diagonal Block with right high
/// </summary>
class D2 : TetrisBlock
{
    public D2(bool[,] shape)
        : base(shape)
    {
        base.shape = new bool[4, 4]{
                {false, false, false, false},

                {false, false, true , false},

                {false, true , true , false},

                {false, true , false, false}};

    }
}