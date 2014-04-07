using System;


namespace WP2048
{
    /// <summary>
    /// 游戏状态更改事件委托
    /// </summary>
    /// <param name="sender">游戏管理器</param>
    /// <param name="args">附加参数</param>
    public delegate void GameStatusChangedHandler(Object sender, EventArgs args);

    /// <summary>
    /// 游戏分数更改事件委托
    /// </summary>
    /// <param name="sender">游戏管理器</param>
    /// <param name="args">附加参数</param>
    public delegate void GameScoreChangedHandler(Object sender, EventArgs args);
}
