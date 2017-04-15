using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScoutingFileAnalyzer
{
    public partial class MainForm : Form
    {
        private Dictionary<string, Dictionary<string, double>> _stats;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            using (var box = new OpenFileDialog())
            {
                if (box.ShowDialog() == DialogResult.OK)
                {
                    LoadFile(box.FileName);
                }
            }
        }

        private void LoadFile(string fileName)
        {
            var parser = new CsvParser.CsvParser(',');
            List<string> headers = null;
            var results = new Dictionary<string, List<Dictionary<string, string>>>();
            var file = parser.ParseFile(fileName, Encoding.Default);
            foreach (var row in file)
            {
                if (headers == null)
                {
                    headers = new List<string>();
                    foreach (var header in row)
                    {
                        headers.Add(header.Trim('\"'));
                    }
                }
                else
                {
                    var dict = new Dictionary<string, string>();
                    for (int i = 0; i < row.Length; i++)
                    {
                        dict.Add(headers[i].Trim('\"'), row[i].Trim('\"'));
                    }

                    if (!results.ContainsKey(dict["Team number"]))
                    {
                        results.Add(dict["Team number"], new List<Dictionary<string, string>>());
                        Console.WriteLine("Team '{0}' doesn't exist yet.", dict["Team number"]);
                    }

                    results[dict["Team number"]].Add(dict);
                }
            }

            dgvData.Rows.Clear();

            if (dgvData.Columns.Count == 0)
            {
                foreach (var column in headers)
                {
                    dgvData.Columns.Add(column, column);
                }
            }

            _stats = new Dictionary<string, Dictionary<string, double>>();
            
            foreach (var teamResults in results)
            {
                var dict = new Dictionary<string, List<double>>();
                foreach (var result in teamResults.Value)
                {
                    dgvData.Rows.Add(result.Values.ToArray());
                    foreach (var item in result)
                    {
                        if (!dict.ContainsKey(item.Key))
                        {
                            dict.Add(item.Key, new List<double>());
                        }

                        double value;
                        if (double.TryParse(item.Value, out value))
                        {
                            dict[item.Key].Add(value);
                        }
                    }
                }

                var statsDict = new Dictionary<string, double>();
                foreach (var list in dict)
                {
                    if (list.Value.Count > 0)
                    {
                        statsDict.Add(string.Format("{0} Average", list.Key), list.Value.Average());
                        statsDict.Add(string.Format("{0} Minimum", list.Key), list.Value.Min());
                        statsDict.Add(string.Format("{0} Maximum", list.Key), list.Value.Max());
                    }
                }

                _stats.Add(teamResults.Key, statsDict);
            }

            var sorted = new List<int>();
            foreach (var key in _stats.Keys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    sorted.Add(Convert.ToInt32(key));
                }
            }

            sorted.Sort();

            var statsWeCareAbout = new List<Stats>();
            foreach (var key in sorted)
            {
                var team = key.ToString();
                statsWeCareAbout.Add(new Stats()
                {
                    TeamNumber = team,
                    NumberOfGearsInAutoAverage = _stats[team]["Number of Gears in Auto Average"],
                    NumberOfGearsInTeleOpAverage = _stats[team]["Number of Gears in Tele Average"],
                    RobotTurnedOnLightAverage = _stats[team]["Robot turns on light Average"],
                });
            }

            dgvStats.Columns.Clear();
            dgvStats.Columns.Add("Team Number", "Team Number");
            dgvStats.Columns.Add("Auto Gears Average", "Auto Gears Average");
            dgvStats.Columns.Add("TeleOp Gears Average", "TeleOp Gears Average");
            dgvStats.Columns.Add("Climb Average", "Climb Average");
            dgvStats.Columns.Add("Score", "Score");

            foreach (var stats in statsWeCareAbout)
            {
                dgvStats.Rows.Add(stats.ToArray());
            }
        }

        private void btnLookup_Click(object sender, EventArgs e)
        {
            if (_stats.ContainsKey(txtTeamNumber.Text))
            {
                MessageBox.Show(GetTeamStats(txtTeamNumber.Text));
            }
        }

        private string GetTeamStats(string team)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("{0}:", team));
            foreach (var item in _stats[team])
            {
                builder.AppendLine(string.Format("  {0}: {1}", item.Key, item.Value));
            }

            return builder.ToString();
        }
    }
}
