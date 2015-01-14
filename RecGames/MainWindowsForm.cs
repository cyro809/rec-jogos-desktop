using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecGames
{
    public partial class MainWindowsForm : Form
    {
        string id;
        public MainWindowsForm(string id)
        {
            InitializeComponent();
            this.id = id;

            Program.loadPlayerInformations(id);

            textBoxSteamName.Text = Program.player.SteamName;

            var definingTags = Program.player.DefiningTags.Values;
            for(int i = 0; i < Program.player.DefiningTags.Count; i++) {
                textBoxPlayerCharacteristics.Text += definingTags.ElementAt(i) + Environment.NewLine;
            }

            for (int i = 0; i < Program.player.MyGames.Count; i++)
            {
                textBoxMyGames.Text += Program.player.MyGames.ElementAt(i) + Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.beginRecommendation(id);
            textBoxRecommendedGames.Text = Program.justification;
            button1.Dispose();
        }

    }
}
