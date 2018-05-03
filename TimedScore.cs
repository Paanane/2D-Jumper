using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace game {

    class TimedScore {

        public int Rank { get; set; }

        public string Name { get; set; }

        public string Score { get; set; }

        public static List<TimedScore> getScores() {
            try {

                List<TimedScore> scores = new List<TimedScore>();
                TimedScore timedScores;

                string _conn = MainWindow.connStr;
                string sqlGetTimed = "SELECT playername, score FROM highscores WHERE mode = \"timed\" ORDER BY score DESC LIMIT 10";
                int _rank = 1;

                using (MySqlConnection connection = new MySqlConnection(_conn)) {

                    connection.Open();

                    using (MySqlCommand cmdTimed = new MySqlCommand(sqlGetTimed, connection))
                    using (MySqlDataReader reader = cmdTimed.ExecuteReader()) {

                        while (reader.Read()) {
                            timedScores = new TimedScore();
                            timedScores.Rank = _rank;
                            timedScores.Name = reader.GetString(0);
                            timedScores.Score = reader.GetString(1);
                            scores.Add(timedScores);
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
