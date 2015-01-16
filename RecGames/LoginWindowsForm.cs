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
    public partial class LoginWindowsForm : Form
    {
        public LoginWindowsForm()
        {
            InitializeComponent();
        }

        private void LoginWindows_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            Program.playerID = id;
            Console.WriteLine(id);

            try
            {
                Program.ValidateSteamID(id);
                DialogResult = DialogResult.OK;
            }
            catch (LoginException loginException)
            {
                MessageBox.Show(loginException.Message);
            }
            
        }
    }
}
