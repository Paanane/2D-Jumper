using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace game {
    public partial class MainWindow : Window {


        public static string connStr = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + $"../../resources/sql/sqlConnection.txt");
        public static gameWindow game;


        public MainWindow() {
            try {
                InitializeComponent();
                checkDB();
            }   catch(Exception ex) {
                    MessageBox.Show(ex.Message, "Error loading database");
            }
        }

        private void checkDB() {

            string start = $"SELECT * FROM highscores";
            string sqlInit = $"CREATE TABLE IF NOT EXISTS highscores (ID INT(3) AUTO_INCREMENT PRIMARY KEY, playername VARCHAR(30) NOT NULL, score INT(2),time INT(3), mode VARCHAR(5));";
            string defaultScores = File.ReadAllText(@"../../resources/sql/defaultScores.sql");
            bool exists;

                using (MySqlConnection conn = new MySqlConnection(connStr)) {
                    conn.Open();
                try {
                    using (MySqlCommand initSearch = new MySqlCommand(start, conn))
                        initSearch.ExecuteNonQuery();
                        exists = true;
                        } catch {
                    exists = false;
                }                   
                    if (!exists) {
                        using (MySqlCommand addscores = new MySqlCommand(defaultScores, conn)) {
                        using (MySqlCommand initDB = new MySqlCommand(sqlInit, conn))
                            initDB.ExecuteNonQuery();
                            addscores.ExecuteNonQuery();
                        }
                    }
                }
            }


        /*-----------------------*/
        /*-------MAIN MENU-------*/
        /*-----------------------*/

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            MenuButtons.Visibility = Visibility.Hidden;
            gameSelectCanvas.Visibility = Visibility.Visible;
        }

        private void HighscoresButton_Click(object sender, RoutedEventArgs e) {

            try {
                MenuButtons.Visibility = Visibility.Hidden;
                highscoresCanvas.Visibility = Visibility.Visible;

                topSoloScores.ItemsSource = SoloScore.getScores();
                topTimedScores.ItemsSource = TimedScore.getScores();

            }   catch(Exception ex) {
                MessageBox.Show(ex.Message, "Highscores buttons");
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            MenuButtons.Visibility = Visibility.Hidden;
            settingsCanvas.Visibility = Visibility.Visible;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }


        /*------------------------*/
        /*-------GAMES MENU-------*/
        /*------------------------*/

        private void playSolo(object sender, RoutedEventArgs e) {
            gameSelectCanvas.Visibility = Visibility.Hidden;
            SoloCanvas.Visibility = Visibility.Visible;
        }

        private void playTimed(object sender, RoutedEventArgs e) {
            gameSelectCanvas.Visibility = Visibility.Hidden;
            TimedCanvas.Visibility = Visibility.Visible;
        }

        private void playPvP(object sender, RoutedEventArgs e) {
            gameSelectCanvas.Visibility = Visibility.Hidden;
            PvPCanvas.Visibility = Visibility.Visible;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            MenuButtons.Visibility = Visibility.Visible;
            gameSelectCanvas.Visibility = Visibility.Hidden;
            settingsCanvas.Visibility = Visibility.Hidden;
            highscoresCanvas.Visibility = Visibility.Hidden;
        }


        /*--------------------------*/
        /*-------PREGAME MENU-------*/
        /*--------------------------*/

        private void startSoloGameButton_Click(object sender, RoutedEventArgs e) {
            try {
                game = new gameWindow(playerName.Text, null, 0, "Solo");
                App.bgMusic.Stop();
                game.Show();
                this.Close();
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, "StartSoloGame");
            }
            
        }
        private void soloBackButton_Click(object sender, RoutedEventArgs e) {
            SoloCanvas.Visibility = Visibility.Hidden;
            gameSelectCanvas.Visibility = Visibility.Visible;
        }


        private void startPvPGameButton_Click(object sender, RoutedEventArgs e) {
            try {
                game = new gameWindow(player1Name.Text, player2Name.Text, (int)pointsSlider.Value, "PvP");
                App.bgMusic.Stop();
                game.Show();
                this.Close();
            } catch(Exception ex) {
                MessageBox.Show(ex.Message, "StartPvPGame");
            }
        }
        private void pvpBackButton_Click(object sender, RoutedEventArgs e) {
            gameSelectCanvas.Visibility = Visibility.Visible;
            PvPCanvas.Visibility = Visibility.Hidden;
        }
        private void pointsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            pointsForWinLabel.Content = "Points for win: " + pointsSlider.Value;
        }


        private void startTimedGameButton_Click(object sender, RoutedEventArgs e) {
            try { 
                game = new gameWindow(playerNameTimed.Text, null, 0, "Timed");
                App.bgMusic.Stop();
                game.Show();
                this.Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "StartTimedGame");
            }
        }
        private void timedBackButton_Click(object sender, RoutedEventArgs e) {
            TimedCanvas.Visibility = Visibility.Hidden;
            gameSelectCanvas.Visibility = Visibility.Visible;
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            App.bgMusic.Volume = volumeSlider.Value;
        }

        private void toggleMusic_Click(object sender, RoutedEventArgs e) {
            volumeSlider.Visibility = volumeSlider.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e) {
            MenuButtons.Visibility = Visibility.Hidden;
            creditsCanvas.Visibility = Visibility.Visible;
        }

        private void creditsBackButton_Click(object sender, RoutedEventArgs e) {
            creditsCanvas.Visibility = Visibility.Hidden;
            MenuButtons.Visibility = Visibility.Visible;
        }
    }
}