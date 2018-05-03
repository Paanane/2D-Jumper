using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Ink;
using MySql.Data.MySqlClient;
using System.Windows.Controls.Primitives;

namespace game {

    public partial class gameWindow : Window {

        /********************\
         *   Variables      *
         *   StartUp        *
        \********************/

        Uri player1normal = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/player1.png", UriKind.RelativeOrAbsolute);
        Uri player1flip = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/player1-flip.png", UriKind.RelativeOrAbsolute);

        Uri player2normal = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/player2.png", UriKind.RelativeOrAbsolute);
        Uri player2flip = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/player2-flip.png", UriKind.RelativeOrAbsolute);

        List<Player> players = new List<Player>();
        Player p1 = null, p2 = null;
        DispatcherTimer timer;

        enum modes { none, Solo, PvP, Timed };
        string player1, player2;
        int frameCounter = 0;
        int maxPoints;
        string mode;        
        int time;

        private int tickInterval = 20;
        private int fps;

        static int [,] orig_map1 = new int[20, 40] {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 1,  },
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1,  },
            { 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,  },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1,  },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,  },
            { 1, 2, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,  },
            { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1,  },
            { 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1,  },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1,  },
            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,  },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1   }
        };
        public static int[,] map_struct = orig_map1;
        Map map = new Map(map_struct);

        public gameWindow(string p1Name, string p2Name, int maxScore, string mode) {
            InitializeComponent();
            this.player1 = p1Name;
            this.player2 = p2Name;
            this.maxPoints = maxScore;
            this.mode = mode;
            this.fps = 1000 / tickInterval;
            map_struct = orig_map1;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {

            try {
                // Init game main loop timer
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(tickInterval);

                // Which game mode is selected
                if (mode == modes.Solo.ToString()) timer.Tick += gameLoopSolo;
                else if (mode == modes.PvP.ToString()) timer.Tick += gameLoopPvP;
                else if (mode == modes.Timed.ToString()) timer.Tick += gameLoopTimed;
                else throw new Exception("Game mode undefined");

                initMap();

                players.Add(p1 = new Player(player1, 100));
                if (mode == modes.PvP.ToString()) players.Add(p2 = new Player(player2, 70));

                // Start the game
                App.playIngameMusic("MetalCrusher.mp3");
                timer.Start();
                
            }   catch (Exception ex){
                MessageBox.Show(ex.Message, "Window_Loaded");
            }
        }


        /********************\
         *   Update game    *
         *   Display Game   *
        \********************/


        /* RESET AND REDRAW MAP */
        public void initMap() {

            map_struct = orig_map1;
            map = new Map(map_struct);

            UniformGrid grid = map.create();

            MyCanvas.Children.Clear();
            MyCanvas.Children.Add(grid);

        }

        /* SOLO SPEEDRUN 10 CHECKPOINTS */
        private void gameLoopSolo(object sender, EventArgs e) {

            try {
                //Update game
                if(p1.update()) initMap();

                movePlayers();

                time = frameCounter / fps;

                p1status.Content = p1.name + "'s score: " + p1.getScore();               
                label.Content = "Time: " + time;
                frameCounter++;

                if (p1.getScore() >= 10) endGame(p1);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "GameTickSolo");
            }

        }

        /* 1V1 FIRST TO X POINTS */
        private void gameLoopPvP(object sender, EventArgs e) {

            try {
                //Update game
                if (p1.update() || p2.update()) initMap();

                movePlayers();

                time = frameCounter / fps;
                p1status.Content = p1.name + "'s score: " + p1.getScore();
                p2status.Content = p2.name + "'s score: " + p2.getScore();

                if (p1.getScore() >= maxPoints) endGame(p1);
                if (p2.getScore() >= maxPoints) endGame(p2);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "GameTickPvP");
            }

        }

        /* GET AS MANY POINTS AS YOU CAN IN X SECONDS */
        private void gameLoopTimed(object sender, EventArgs e) {

             try {
                //Update game
                if (p1.update()) initMap();

                movePlayers();

                time = frameCounter / fps;

                p1status.Content = p1.name + "'s score: " + p1.getScore();                
                label.Content = "Time left: " + (30 - time);
                frameCounter++;

                //~30 Seconds
                if (frameCounter > 1500) endGame(p1);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "GameTickTimed");
            }

        }

        private void movePlayers() {
            Canvas.SetTop(PlayerCharacter, p1.PosY);
            Canvas.SetLeft(PlayerCharacter, p1.PosX);
            Canvas.SetRight(PlayerCharacter, p1.PosX + Player.playerSize);
            Canvas.SetBottom(PlayerCharacter, p1.PosY + Player.playerSize);

            if (p2 != null) {
                Canvas.SetTop(Player2Character, p2.PosY);
                Canvas.SetLeft(Player2Character, p2.PosX);
                Canvas.SetRight(Player2Character, p2.PosX + Player.playerSize);
                Canvas.SetBottom(Player2Character, p2.PosY + Player.playerSize);
            }
        }

        /********************\
         *   XAML           *
         *   UI-Triggers    *
        \********************/

        
        /* CHANGE VOLUME */
        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            App.ingameMusicPlayer.Volume = volumeSlider.Value / 3;
        }

        private void toggleMusic_Click(object sender, RoutedEventArgs e) {
            volumeSlider.Visibility = volumeSlider.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            Keyboard.ClearFocus();
            Keyboard.Focus(MyCanvas);
        }

        /* START MOVEMENT TRIGGERS */
        private void Window_KeyDown(object sender, KeyEventArgs e) {

            switch (e.Key) {
                case Key.A:
                    PlayerCharacter.Source = BitmapFrame.Create(player1flip);
                    p1.movingLeft = true;
                    break;

                case Key.D:
                    PlayerCharacter.Source = BitmapFrame.Create(player1normal);
                    p1.movingRight = true;
                    break;

                case Key.Space:
                    p1.tryJump = true;
                    break;

                case Key.Left:
                    if (p2 != null) {
                        Player2Character.Source = BitmapFrame.Create(player2flip);
                        p2.movingLeft = true;
                    }
                    break;

                case Key.Right:
                    if (p2 != null) {
                        Player2Character.Source = BitmapFrame.Create(player2normal);
                        p2.movingRight = true;
                    }
                    break;

                case Key.Up:
                    if (p2 != null)
                        p2.tryJump = true;
                    break;

                case Key.Escape:
                    pauseButton_Click(null, null);
                    break;
            }
        }

        /* STOP MOVEMENT TRIGGERS */
        private void Window_KeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.A:
                    p1.movingLeft = false;
                    break;

                case Key.D:
                    p1.movingRight = false;
                    break;

                case Key.Left:
                    if (p2 != null)
                        p2.movingLeft = false;
                    break;

                case Key.Right:
                    if (p2 != null)
                        p2.movingRight = false;
                    break;
            }
        }

        /* PAUSE GAME / OPEN OPTIONS */
        private void pauseButton_Click(object sender, RoutedEventArgs e) {
            timer.Stop();
            pauseButton.IsEnabled = false;
            pauseCanvas.Visibility = Visibility.Visible;
        }


        /* CONTINUE GAME */
        private void ContinueButton_Click(object sender, RoutedEventArgs e) {
            pauseCanvas.Visibility = Visibility.Hidden;
            pauseButton.IsEnabled = true;
            timer.Start();
        }


        /* RESTART GAME */
        private void restartButton_Click(object sender, RoutedEventArgs e) {
            players.ForEach(player => player.restart());
            frameCounter = 0;
            pauseCanvas.Visibility = Visibility.Hidden;
            initMap();
            pauseButton.IsEnabled = true;
            timer.Start();
        }

        /* OPEN SETTINGS MENU */
        private void settingsButton_Click(object sender, RoutedEventArgs e) {
            settingsCanvas.Visibility = Visibility.Visible;
        }

        /* PRESS QUIT INGAME */
        private void quitButton_Click(object sender, RoutedEventArgs e) {
            openMainWindow();
            App.ingameMusicPlayer.Stop();
            App.bgMusic.Play();
            this.Close();
        }

        private void openMainWindow() {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e) {
            App.ingameMusicPlayer.Stop();
            App.bgMusic.Play();
            openMainWindow();
            game.Close();           
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            settingsCanvas.Visibility = Visibility.Hidden;
        }

        /* GAME WON / GAME OVER */
        private void endGame(Player winner) {
            
                timer.Stop();
                pauseButton.IsEnabled = false;
                winnerLabel.Content = mode == modes.PvP.ToString() ? $"Congratulations {winner.name}!\nYou are winner!" : $"You got {winner.getScore()} points\n in {time} seconds!";
                endGameCanvas.Visibility = Visibility.Visible;

            try {
                string _conn = MainWindow.connStr;
                string sqlSolo = $"INSERT INTO highscores (playername,score,time,mode) VALUES (\"{ winner.name } \", \"{ winner.getScore() }\", \"{ time }\", \"solo\");";
                string sqlTimed = $"INSERT INTO highscores (playername,score,time,mode) VALUES (\"{ winner.name } \", \"{ winner.getScore() }\", \"{ time }\", \"timed\");";
                string sql = mode == modes.Solo.ToString() ? sqlSolo : sqlTimed;

                if (mode != modes.PvP.ToString()) {
                    using (MySqlConnection conn = new MySqlConnection(_conn)) {

                        conn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            cmd.ExecuteNonQuery();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error submitting score");
            }

        }
    } 
}