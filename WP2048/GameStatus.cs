using System;

namespace WP2048
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStatus : byte
    {
        /// <summary>
        /// 正在进行
        /// </summary>
        Running = 0,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 1,

        /// <summary>
        /// 胜利
        /// </summary>
        Win = 2
    }
}