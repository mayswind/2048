using System;
using System.Windows;
using System.Windows.Controls;

namespace WP2048
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    internal class GameManager
    {
        #region 常量
        private const Int32 Size = 4;
        private const Int32 StartTiles = 2;

        private static readonly TileStyle[] Styles = new TileStyle[]
        {
            new TileStyle(60, FontWeights.Bold, 0xff, 0x77, 0x6e, 0x65, 0x60, 0xee, 0xe4, 0xda),//0
            new TileStyle(60, FontWeights.Bold, 0xff, 0x77, 0x6e, 0x65, 0xff, 0xee, 0xe4, 0xda),//2
            new TileStyle(60, FontWeights.Bold, 0xff, 0x77, 0x6e, 0x65, 0xff, 0xed, 0xe0, 0xc8),//4
            new TileStyle(60, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xf2, 0xb1, 0x79),//8
            new TileStyle(50, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xf5, 0x95, 0x63),//16
            new TileStyle(50, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xf6, 0x7c, 0x5f),//32
            new TileStyle(50, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xf6, 0x5e, 0x3b),//64
            new TileStyle(40, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xed, 0xcf, 0x72),//128
            new TileStyle(40, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xed, 0xcc, 0x61),//256
            new TileStyle(40, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xed, 0xc8, 0x50),//512
            new TileStyle(30, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xed, 0xc5, 0x3f),//1024
            new TileStyle(30, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0xed, 0xc2, 0x2e),//2048
            new TileStyle(30, FontWeights.Bold, 0xff, 0xf9, 0xf6, 0xf2, 0xff, 0x3c, 0x3a, 0x32)//>2048
        };
        #endregion

        #region 字段
        private GridManager _gridManager;
        private GameStatus _gameStatus;
        private Int32 _scores;
        private Random _rand;
        #endregion

        #region 构造方法
        internal GameManager(Grid parentGrid)
        {
            this._gridManager = new GridManager(GameManager.Size, parentGrid);
            this._rand = new Random();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取游戏状态
        /// </summary>
        internal GameStatus GameStatus
        {
            get { return this._gameStatus; }
        }

        /// <summary>
        /// 获取游戏成绩
        /// </summary>
        internal Int32 Scores
        {
            get { return this._scores; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化游戏
        /// </summary>
        internal void InitGame()
        {
            this.RestartGame();
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        internal void RestartGame()
        {
            this._gameStatus = GameStatus.Running;
            this._scores = 0;

            for (Int32 i = 0; i < this._gridManager.GridSize; i++)
            {
                for (Int32 j = 0 ; j < this._gridManager.GridSize; j++)
                {
                    this.SetTileValue(i, j, 0);
                }
            }

            for (Int32 i = 0; i < GameManager.StartTiles; i++)
            {
                this.GenerateRandomTile();
            }
        }

        /// <summary>
        /// 合并磁贴
        /// </summary>
        /// <param name="dx">x分量变化量</param>
        /// <param name="dy">y分量变化量</param>
        internal void CombineTiles(Int32 dx, Int32 dy)
        {
            if (!this._gridManager.IsMovesAvailable())
            {
                this._gameStatus = GameStatus.Failed;
                return;
            }

            Boolean moved = false;

            moved |= this.CompressTiles(dx, dy);

            for (Int32 i = 0; i < this._gridManager.GridSize; i++)
            {
                for (Int32 j = 0; j < this._gridManager.GridSize; j++)
                {
                    Int32 currentValue = this.GetTileValue(i, j);

                    Int32 ni = i + dy;
                    Int32 nj = j + dx;

                    if (!this._gridManager.IsInBounds(ni, nj))
                    {
                        continue;
                    }

                    Int32 otherValue = this.GetTileValue(ni, nj);

                    if (currentValue == otherValue)
                    {
                        Int32 newValue = currentValue * 2;
                        this.SetTileValue(ni, nj, newValue);
                        this.SetTileValue(i, j, 0);

                        this._scores += newValue;
                        moved = true;

                        if (newValue == 2048)
                        {
                            this._gameStatus = GameStatus.Win;
                        }
                    }
                }
            }

            if (moved)
            {
                if (!this._gridManager.IsMovesAvailable())
                {
                    this._gameStatus = GameStatus.Failed;
                    return;
                }

                this.CompressTiles(dx, dy);
                this.GenerateRandomTile();
            }
        }

        private Boolean CompressTiles(Int32 dx, Int32 dy)
        {
            Boolean moved = false;

            for (Int32 i = 0; i < this._gridManager.GridSize; i++)
            {
                for (Int32 j = 0; j < this._gridManager.GridSize; j++)
                {
                    Int32 currentValue = this.GetTileValue(i, j);

                    if (currentValue == 0)
                    {
                        continue;
                    }

                    Int32 ni = i + dy;
                    Int32 nj = j + dx;

                    if (!this._gridManager.IsInBounds(ni, nj))
                    {
                        continue;
                    }

                    Int32 otherValue = this.GetTileValue(ni, nj);

                    if (otherValue == 0)
                    {
                        this.SetTileValue(ni, nj, currentValue);
                        this.SetTileValue(i, j, 0);

                        moved = true;
                    }
                }
            }

            if (moved)
            {
                this.CompressTiles(dx, dy);
            }

            return moved;
        }
        #endregion

        #region 私有方法
        private void GenerateRandomTile()
        {
            Point[] tilePoints = this._gridManager.GetAvailableTiles();

            if (tilePoints == null || tilePoints.Length < 1)
            {
                return;
            }

            Int32 value = this._rand.NextDouble() < 0.9 ? 2 : 4;
            Int32 index = this._rand.Next(0, tilePoints.Length);

            this.SetTileValue((Int32)tilePoints[index].X, (Int32)tilePoints[index].Y, value);
        }

        private Int32 GetTileValue(Int32 x, Int32 y)
        {
            return this._gridManager.GetTileValue(x, y);
        }

        private void SetTileValue(Int32 x, Int32 y, Int32 value)
        {
            Int32 index = (value > 2048 ? 12 : (value < 2 ? 0 : (Int32)Math.Log(value, 2)));
            TileStyle style = GameManager.Styles[index];

            this._gridManager.SetTileValue(x, y, value, style);
        }
        #endregion
    }
}