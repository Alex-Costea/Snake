using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Snake.Properties;

namespace Snake
{

    public partial class HighScores : Form
    {
        static int NumberOfHighScores = 10;
        HighScore[] TopScores = new HighScore[NumberOfHighScores+1];
        private string name
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
                Settings.Default.Name = value;
            }
        }
        public HighScores()
        {
            InitializeComponent();
            button1.Enabled = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            name = Settings.Default.Name;
        }

        private void ResetScores(bool ShowNewHighScore=true)
        {
            string[] TextScores = Settings.Default.HighScores.Split('\n');
            for (int i = 0; i < NumberOfHighScores*2; i += 2)
            {
                TopScores[i / 2] = new HighScore(Int32.Parse(TextScores[i + 1]), TextScores[i]);
                if ((Program.CurrentForm.Points > TopScores[i / 2].score) && ShowNewHighScore) 
                {
                    button1.Enabled = true;
                    label1.Text = String.Format("You got a new High Score of {0}! Enter your name:", Program.CurrentForm.Points);
                }
            }
            label2.Text = "";
            for (int i = 0; i < NumberOfHighScores; i++)
                if (TopScores[i].name != "None")
                    label2.Text += String.Format("{0}. {1} - {2}\n", i + 1, TopScores[i].name,TopScores[i].score);
        }

        private void HighScores_Load(object sender, EventArgs e)
        {
            ResetScores();
        }

        private void HighScores_FormClosing(object sender, FormClosingEventArgs e)
        {
            name = textBox1.Text;
            Settings.Default.Save();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(name))&&name!="None")
            {
                TopScores[NumberOfHighScores] = new HighScore(Program.CurrentForm.Points, name);
                Array.Sort(TopScores);
                Settings.Default.HighScores = "";
                for (int i = 0; i < NumberOfHighScores; i++)
                        Settings.Default.HighScores += String.Format("{0}\n{1}\n", TopScores[i].name, TopScores[i].score);
                ResetScores(false);
                button1.Enabled = false;
            }
            else MessageBox.Show("Please insert your name!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Default.HighScores = "";
            for (int i = 0; i < NumberOfHighScores; i++)
                Settings.Default.HighScores += "None\n0\n";
            ResetScores();
        }
    }

    class HighScore:IComparable
    {
        public int score;
        public string name;
        public int CompareTo(object o)
        {
            if(o is HighScore)
            {
                return (o as HighScore).score-score;
            }
            return 1;
        }
        public HighScore(int score, string name)
        {
            this.score = score;
            this.name = name;
        }
    }
}
