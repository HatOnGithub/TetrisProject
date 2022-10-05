using System;
using System.Text;

public class TetrisBlock
{
    public bool[,] Shape { get { return shape; } set { shape = value; } }
    bool[,] shape;
    public TetrisBlock()
    {

    }

    /// <summary>
    ///  T Block 
    /// </summary>
	class T
    {
        public T(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , false, false},

                {false, true , true , false},

                {false, true , false, false}};
        }
    }
    /// <summary>
    /// L Block
    /// </summary>
	class L1
    {
        public L1(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, true , false, false},

                {false, true , false, false}};
        }
    }
    /// <summary>
    /// Flipped L Block
    /// </summary>
	class L2
    {
        public L2(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, false, true , false},

                {false, false, true , false}};
        }
    }
    /// <summary>
    /// Long BLock
    /// </summary>
    class Long
    {
        public Long(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, true , false, false},

                {false, true , false, false},

                {false, true , false, false},

                {false, true , false, false}};
        }
    }
    /// <summary>
    /// Square Block
    /// </summary>
    class SQ
    {
        public SQ(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , true , false},

                {false, true , true , false},

                {false, false, false, false}};
        }
    }

    /// <summary>
    /// Diagonal Block with left high
    /// </summary>
    class D1
    {
        public D1(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, false, false, false},

                {false, true , false, false},

                {false, true , true , false},

                {false, false, true , false}};
        }
    }
    /// <summary>
    /// Diagonal Block with right high
    /// </summary>
    class D2
    {
        public D2(TetrisBlock tetrisBlock)
        {
            tetrisBlock.Shape = new bool[4, 4]{
                {false, false, false, false},

                {false, false, true , false},

                {false, true , true , false},

                {false, true , false, false}};

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