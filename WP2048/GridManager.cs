using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WP2048
{
    /// <summary>
    /// 布局管理器
    /// </summary>
    internal class GridManager<T> where T : ContentControl
    {
        #region 常量
        private static readonly Int32[] VectorX = new Int32[4] { 0, 1, 0, -1 };//Up Right Bottom Left
        private static readonly Int32[] VectorY = new Int32[4] { -1, 0, 1, 0 };
        #endregion

        #region 字段
        private Int32 _gridSize;
        private T[,] _controls;
        private Int32[,] _tilesValue;
        #endregion

        #region 属性
        /// <summary>
        /// 获取布局大小
        /// </summary>
        public Int32 GridSize
        {
            get { return this._gridSize; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的布局管理器
        /// </summary>
        /// <param name="gridSize">布局大小</param>
        /// <param name="parentGrid">界面布局控件</param>
        internal GridManager(Int32 gridSize, Grid parentGrid)
        {
            this._gridSize = gridSize;
            this._controls = new T[this._gridSize, this._gridSize];
            this._tilesValue = new Int32[this._gridSize, this._gridSize];

            this.ForEach((i, j) => this._controls[i, j] = parentGrid.Children[i * this._gridSize + j] as T);
        }
        #endregion

        #region 遍历方法
        /// <summary>
        /// 遍历所有格子
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <returns>自定义返回值</returns>
        internal Boolean ForEach(Func<Int32, Int32, Boolean> func)
        {
            for (Int32 i = 0; i < this._gridSize; i++)
            {
                for (Int32 j = 0; j < this._gridSize; j++)
                {
                    Boolean result = func(i, j);

                    if (result)
                    {
                        return result;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 遍历所有格子
        /// </summary>
        /// <param name="action">操作方法</param>
        internal void ForEach(Action<Int32, Int32> action)
        {
            for (Int32 i = 0; i < this._gridSize; i++)
            {
                for (Int32 j = 0; j < this._gridSize; j++)
                {
                    action(i, j);
                }
            }
        }
        #endregion

        #region 磁贴判断
        /// <summary>
        /// 获取格子中是否有空余的位置
        /// </summary>
        /// <returns>是否有空余的位置</returns>
        internal Boolean IsGridsHasAvailableTile()
        {
            return this.ForEach((i, j) => this.GetTileValue(i, j) == 0);
        }

        /// <summary>
        /// 获取格子中是否有可合并的磁贴
        /// </summary>
        /// <returns>是否有可合并的磁贴</returns>
        internal Boolean IsGridsHasMergableTile()
        {
            return this.ForEach((i, j) =>
            {
                Int32 currentValue = this.GetTileValue(i, j);

                for (Int32 k = 0; k < 4; k++)
                {
                    Int32 ni = i + GridManager<T>.VectorX[k];
                    Int32 nj = j + GridManager<T>.VectorY[k];

                    if (!this.IsInBounds(ni, nj))
                    {
                        continue;
                    }

                    Int32 otherValue = this.GetTileValue(ni, nj);

                    if (currentValue == otherValue)
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// 获取是否可以进行移动
        /// </summary>
        /// <returns>是否可以进行移动</returns>
        internal Boolean IsMovesAvailable()
        {
            return this.IsGridsHasAvailableTile() || this.IsGridsHasMergableTile();
        }

        /// <summary>
        /// 获取指定位置是否在区域中
        /// </summary>
        /// <param name="x">x分量</param>
        /// <param name="y">y分量</param>
        /// <returns>指定位置是否在区域中</returns>
        internal Boolean IsInBounds(Int32 x, Int32 y)
        {
            return (x >= 0 && x < this._gridSize && y >= 0 && y < this._gridSize);
        }
        #endregion

        #region 获取或设置磁贴
        internal Int32 GetTileValue(Int32 x, Int32 y)
        {
            return this._tilesValue[x, y];
        }

        internal Point[] GetAvailableTiles()
        {
            List<Point> result = new List<Point>();

            this.ForEach((i, j) =>
            {
                if (this.GetTileValue(i, j) == 0)
                {
                    result.Add(new Point(i, j));
                }
            });

            return result.ToArray();
        }

        internal void SetTileValue(Int32 x, Int32 y, Int32 value, TileStyle style)
        {
            T tile = this._controls[x, y];

            tile.FontSize = style.FontSize;
            tile.FontWeight = style.FontWeight;
            tile.Foreground = new SolidColorBrush(style.ForeColor);
            tile.Background = new SolidColorBrush(style.BackColor);

            tile.Content = (value < 2 ? "" : value.ToString());
            this._tilesValue[x, y] = value;
        }
        #endregion
    }
}