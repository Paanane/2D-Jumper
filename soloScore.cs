using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace game {
    class SoloScore {

        public int Rank { get; set; }

        public string Name { get; set; }

        public string Time { get; set; }

        public static List<SoloScore> getScores() {
            try {

                List<SoloScore> scores = new List<SoloScore>();
                SoloScore soloScores;

                string _conn = MainWindow.connStr;

                string sqlGetSolo = "SELECT playername, time FROM highscores WHERE mode = \"solo\" ORDER BY time LIMIT 10";
                int _rank = 1;


                using (MySqlConnection connection = new MySqlConnection(_conn)) {

                    connection.Open();

                    using (MySqlCommand cmdSolo = new MySqlCommand(sqlGetSolo, connection))
                    using (MySqlDataReader reader = cmdSolo.ExecuteReader()) {

                        while (reader.Read()) {
                            soloScores = new SoloScore();
                            soloScores.Rank = _rank;
                            soloScores.Name = reader.GetString(0);
                            soloScores.Time = reader.GetString(1);
                            scores.Add(soloScores);
                            _rank++;
                        }
                        reader.Close();
                    }

                    return scores;
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

    }
}
