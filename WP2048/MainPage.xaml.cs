using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace WP2048
{
    public partial class MainPage : PhoneApplicationPage
    {
        private GameManager _gameManager;

        public MainPage()
        {
            InitializeComponent();
            this._gameManager = new GameManager(this.gridTiles);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this._gameManager.InitGame();
            this.gridGameOverOverlay.Visibility = Visibility.Collapsed;
            this.bestScore.Text = StorageManager.ReadBestScore().ToString();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            this.gridGameOverOverlay.Visibility = Visibility.Collapsed;
            this._gameManager.RestartGame();
        }

        private void gridTiles_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (this._gameManager.GameStatus == GameStatus.Failed)
            {
                return;
            }

            Point p = e.TotalManipulation.Translation;

            if (p.X == 0 && p.Y == 0)
            {
                return;
            }

            Int32 dx = Math.Abs(p.X) > Math.Abs(p.Y) ? (Int32)p.X : 0;
            Int32 dy = Math.Abs(p.Y) > Math.Abs(p.X) ? (Int32)p.Y : 0;

            dx = (dx == 0 ? 0 : (dx > 0 ? 1 : -1));
            dy = (dy == 0 ? 0 : (dy > 0 ? 1 : -1));

            this._gameManager.CombineTiles(dx, dy);
            this.currentScore.Text = this._gameManager.Scores.ToString();

            if (StorageManager.SaveBestScore(this._gameManager.Scores))
            {
                this.bestScore.Text = StorageManager.ReadBestScore().ToString();
            }

            if (this._gameManager.GameStatus == GameStatus.Win)
            {
                MessageBox.Show("You are win!", "2048", MessageBoxButton.OK);
            }
            else if (this._gameManager.GameStatus == GameStatus.Failed)
            {
                this.gridGameOverOverlay.Visibility = Visibility.Visible;
            }
        }

        private void linkCreatorUrl_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://github.com/mayswind/2048");
            task.Show();
        }

        private void linkOriginUrl_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://gabrielecirulli.github.io/2048");
            task.Show();
        }
    }
}