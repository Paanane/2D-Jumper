using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace game {

    class Map {

        int[,] _map_struct;
        public static int tileSize;

        public Map(int[,] map) {
            this._map_struct = map;
        }

        public UniformGrid create() {

            try {

                int MAP_ROWS = _map_struct.GetLength(0);
                int MAP_COLS = _map_struct.GetLength(1);
                int type, row = 0, column = 0;
                tileSize = 1200 / 40;

                UniformGrid grid = new UniformGrid();
                grid.Columns = 40;
                grid.Rows = 20;
                grid.Width = 40 * tileSize;
                grid.Height = 20 * tileSize;

                Image img;
                Uri empty = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/emptyTile.png", UriKind.RelativeOrAbsolute);
                Uri wall = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/wallTile.png", UriKind.RelativeOrAbsolute);
                Uri coin = new Uri(AppDomain.CurrentDomain.BaseDirectory + "../../resources/images/coin.png", UriKind.RelativeOrAbsolute);

                for (row = 0; row < MAP_ROWS; row++) {
                    for (column = 0; column < MAP_COLS; column++) {

                        type = _map_struct[row, column];
                        img = new Image();

                        switch (type) {

                            case 0:
                                img.Source = BitmapFrame.Create(empty);
                                break;

                            case 1:
                                img.Source = BitmapFrame.Create(wall);
                                break;

                            case 2:
                                img.Source = BitmapFrame.Create(coin);
                                break;

                        }
                        grid.Children.Add(img);

                    }
                }    return grid;

            } catch(Exception ex) {
                throw new Exception(ex.Message + ", at Map.InitMap");
            }
                   
        }
    }
}
