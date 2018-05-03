using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Media;
using System.Windows.Media;

namespace game {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static MediaPlayer bgMusic = new MediaPlayer();
        public static MediaPlayer ingameMusicPlayer = new MediaPlayer();
        public static MediaPlayer soundPlayer = new MediaPlayer();

        public App() {
            bgMusic.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/audio/bgmusic.mp3", UriKind.RelativeOrAbsolute));
            bgMusic.Play();
        }

        public static void playIngameMusic(string songName) {
            ingameMusicPlayer.Volume = 0.15;
            ingameMusicPlayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + $"../../resources/audio/{ songName }", UriKind.RelativeOrAbsolute));
            ingameMusicPlayer.Play();
        }

        public static void playSound(string audiofile) {
            soundPlayer.Volume = 0.1;
            soundPlayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + $"../../resources/audio/{ audiofile }", UriKind.RelativeOrAbsolute));
            soundPlayer.Play();
        }
    }
}
