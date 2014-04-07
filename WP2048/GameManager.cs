using System;
using System.Windows;
using System.Windows.Controls;

namespace WP2048
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    internal class GameManager<T> where T : ContentControl
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
        private GridManager<T> _gridManager;
        private GameStatus _gameStatus;
        private Int32 _scores;
        private Random _rand;
        #endregion

        #region 构造方法
        internal GameManager(Grid parentGrid)
        {
            this._gridManager = new GridManager<T>(GameManager<T>.Size, parentGrid);
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
            private set
            {
                GameStatus oldStatus = this._gameStatus;
                this._gameStatus = value;

                if (oldStatus != value && this.GameStatusChanged != null)
                {
                    this.GameStatusChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 获取游戏成绩
        /// </summary>
        internal Int32 Scores
        {
            get { return this._scores; }
            private set
            {
                Int32 oldScores = this._scores;
                this._scores = value;

                if (oldScores != value && this.GameScoreChanged != null)
                {
                    this.GameScoreChanged(this, new EventArgs());
                }
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 游戏状态更改事件
        /// </summary>
        internal event GameStatusChangedHandler GameStatusChanged;

        /// <summary>
        /// 游戏分数更改事件
        /// </summary>
        internal event GameScoreChangedHandler GameScoreChanged;
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
            this.GameStatus = GameStatus.Running;
            this.Scores = 0;

            this._gridManager.ForEach((i, j) => this.SetTileValue(i, j, 0));

            for (Int32 i = 0; i < GameManager<T>.StartTiles; i++)
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
                this.GameStatus = GameStatus.Failed;
                return;
            }

            Int32 newScores = this._scores;
            Boolean moved = false;

            moved |= this.CompressTiles(dx, dy);

            this.ForEach(dx, dy, (i, j, v, ni, nj, nv) =>
            {
                if (v <= 0 || v != nv)
                {
                    return;
                }

                Int32 newValue = v * 2;
                this.SetTileValue(ni, nj, -newValue);//负值不能重复合并
                this.SetTileValue(i, j, 0);

                moved = true;
                newScores += newValue;

                if (newValue == 2048)
                {
                    this.GameStatus = GameStatus.Win;
                }
            });

            this._gridManager.ForEach((i, j) => this._gridManager.SetTileRightValue(i, j));

            if (moved)
            {
                this.CompressTiles(dx, dy);
                this.GenerateRandomTile();

                if (!this._gridManager.IsMovesAvailable())
                {
                    this.GameStatus = GameStatus.Failed;
                }
            }

            this.Scores = newScores;
        }

        private Boolean CompressTiles(Int32 dx, Int32 dy)
        {
            Boolean moved = false;

            this.ForEach(dx, dy, (i, j, v, ni, nj, nv) =>
            {
                if (nv == 0)
                {
                    this.SetTileValue(ni, nj, v);
                    this.SetTileValue(i, j, 0);

                    moved = true;
                }
            });

            if (moved)
            {
                this.CompressTiles(dx, dy);
            }

            return moved;
        }
        #endregion

        #region 私有方法
        private void ForEach(Int32 dx, Int32 dy, Action<Int32, Int32, Int32, Int32, Int32, Int32> action)
        {
            if (dx == 0 && dy < 0)//Up
            {
                for (Int32 i = 0; i < this._gridManager.GridSize; i++)
                {
                    for (Int32 j = 0; j < this._gridManager.GridSize; j++)
                    {
                        this.ForEach(i, j, dx, dy, action);
                    }
                }
            }
            else if (dx > 0 && dy == 0)//Right
            {
                for (Int32 j = this._gridManager.GridSize - 1; j >= 0; j--)
                {
                    for (Int32 i = this._gridManager.GridSize - 1; i >= 0; i--)
                    {
                        this.ForEach(i, j, dx, dy, action);
                    }
                }
            }
            else if (dx == 0 && dy > 0)//Bottom
            {
                for (Int32 i = this._gridManager.GridSize - 1; i >= 0; i--)
                {
                    for (Int32 j = this._gridManager.GridSize - 1; j >= 0; j--)
                    {
                        this.ForEach(i, j, dx, dy, action);
                    }
                }
            }
            else if (dx < 0 && dy == 0)//Left
            {
                for (Int32 j = 0; j < this._gridManager.GridSize; j++)
                {
                    for (Int32 i = 0; i < this._gridManager.GridSize; i++)
                    {
                        this.ForEach(i, j, dx, dy, action);
                    }
                }
            }
        }

        private void ForEach(Int32 i, Int32 j, Int32 dx, Int32 dy, Action<Int32, Int32, Int32, Int32, Int32, Int32> action)
        {
            Int32 currentValue = this._gridManager.GetTileValue(i, j);

            if (currentValue == 0)
            {
                return;
            }

            Int32 ni = i + dy;
            Int32 nj = j + dx;

            if (!this._gridManager.IsInBounds(ni, nj))
            {
                return;
            }

            Int32 otherValue = this._gridManager.GetTileValue(ni, nj);

            action(i, j, currentValue, ni, nj, otherValue);
        }

        private void GenerateRandomTile()
        {
            Point[] tilePoints = this._gridManager.GetAvailableTiles();

            if (tilePoints == null || tilePoints.Length < 1)
            {
                return;
            }

            Int32 index = this._rand.Next(0, tilePoints.Length);
            Int32 value = this._rand.NextDouble() < 0.9 ? 2 : 4;

            this.SetTileValue((Int32)tilePoints[index].X, (Int32)tilePoints[index].Y, value);
        }

        private void SetTileValue(Int32 x, Int32 y, Int32 value)
        {
            Int32 realValue = Math.Abs(value);
            Int32 index = (realValue > 2048 ? 12 : (realValue < 2 ? 0 : (Int32)Math.Log(realValue, 2)));
            TileStyle style = GameManager<T>.Styles[index];

            this._gridManager.SetTileValue(x, y, value, style);
        }
        #endregion
    }
}