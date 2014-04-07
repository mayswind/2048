using System;
using System.IO.IsolatedStorage;

namespace WP2048
{
    /// <summary>
    /// 存储管理器
    /// </summary>
    internal static class StorageManager
    {
        #region 常量
        private const String BestScoreKey = "BestScore";
        #endregion

        #region 方法
        /// <summary>
        /// 读取最好成绩
        /// </summary>
        /// <returns>最好成绩</returns>
        internal static Int32 ReadBestScore()
        {
            Int32 score = 0;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<Int32>(BestScoreKey, out score))
            {
                return score;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 存储最好成绩
        /// </summary>
        /// <param name="score">最好成绩</param>
        internal static Boolean SaveBestScore(Int32 score)
        {
            Int32 savedScore = StorageManager.ReadBestScore();

            if (score > savedScore)
            {
                IsolatedStorageSettings.ApplicationSettings[BestScoreKey] = score;
                IsolatedStorageSettings.ApplicationSettings.Save();
                return true;
            }

            return false;
        }
        #endregion
    }
}