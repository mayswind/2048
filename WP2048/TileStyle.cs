using System;
using System.Windows;
using System.Windows.Media;

namespace WP2048
{
    /// <summary>
    /// 磁贴样式
    /// </summary>
    public class TileStyle
    {
        #region 字段
        private Int32 _fontSize;
        private FontWeight _fontWeight;
        private Color _foreColor;
        private Color _backColor;
        #endregion

        #region 属性
        /// <summary>
        /// 获取字体大小
        /// </summary>
        public Int32 FontSize
        {
            get { return this._fontSize; }
        }

        /// <summary>
        /// 获取字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return this._fontWeight; }
        }

        /// <summary>
        /// 获取字体颜色
        /// </summary>
        public Color ForeColor
        {
            get { return this._foreColor; }
        }

        /// <summary>
        /// 获取背景颜色
        /// </summary>
        public Color BackColor
        {
            get { return this._backColor; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化新的磁贴样式
        /// </summary>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontWeight">字体粗细</param>
        /// <param name="foreColorA">字体颜色A分量</param>
        /// <param name="foreColorR">字体颜色R分量</param>
        /// <param name="foreColorG">字体颜色G分量</param>
        /// <param name="foreColorB">字体颜色B分量</param>
        /// <param name="backColorA">背景颜色A分量</param>
        /// <param name="backColorR">背景颜色R分量</param>
        /// <param name="backColorG">背景颜色G分量</param>
        /// <param name="backColorB">背景颜色B分量</param>
        public TileStyle(Int32 fontSize, FontWeight fontWeight,
            Byte foreColorA, Byte foreColorR, Byte foreColorG, Byte foreColorB,
            Byte backColorA, Byte backColorR, Byte backColorG, Byte backColorB)
        {
            this._fontSize = fontSize;
            this._fontWeight = fontWeight;
            this._foreColor = Color.FromArgb(foreColorA, foreColorR, foreColorG, foreColorB);
            this._backColor = Color.FromArgb(backColorA, backColorR, backColorG, backColorB);
        }
        #endregion
    }
}