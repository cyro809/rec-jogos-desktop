namespace RecGames
{
    partial class MainWindowsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxSteamName = new System.Windows.Forms.TextBox();
            this.textBoxRecommendedGames = new System.Windows.Forms.TextBox();
            this.textBoxPlayerCharacteristics = new System.Windows.Forms.TextBox();
            this.textBoxMyGames = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steam Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Player Characteristics:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 321);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Recommended Game:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "My Games:";
            // 
            // textBoxSteamName
            // 
            this.textBoxSteamName.Location = new System.Drawing.Point(111, 25);
            this.textBoxSteamName.Name = "textBoxSteamName";
            this.textBoxSteamName.ReadOnly = true;
            this.textBoxSteamName.Size = new System.Drawing.Size(129, 20);
            this.textBoxSteamName.TabIndex = 5;
            // 
            // textBoxRecommendedGames
            // 
            this.textBoxRecommendedGames.Location = new System.Drawing.Point(153, 318);
            this.textBoxRecommendedGames.Multiline = true;
            this.textBoxRecommendedGames.Name = "textBoxRecommendedGames";
            this.textBoxRecommendedGames.ReadOnly = true;
            this.textBoxRecommendedGames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxRecommendedGames.Size = new System.Drawing.Size(484, 54);
            this.textBoxRecommendedGames.TabIndex = 8;
            // 
            // textBoxPlayerCharacteristics
            // 
            this.textBoxPlayerCharacteristics.Location = new System.Drawing.Point(426, 25);
            this.textBoxPlayerCharacteristics.Multiline = true;
            this.textBoxPlayerCharacteristics.Name = "textBoxPlayerCharacteristics";
            this.textBoxPlayerCharacteristics.ReadOnly = true;
            this.textBoxPlayerCharacteristics.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPlayerCharacteristics.Size = new System.Drawing.Size(211, 116);
            this.textBoxPlayerCharacteristics.TabIndex = 9;
            // 
            // textBoxMyGames
            // 
            this.textBoxMyGames.Location = new System.Drawing.Point(100, 164);
            this.textBoxMyGames.Multiline = true;
            this.textBoxMyGames.Name = "textBoxMyGames";
            this.textBoxMyGames.ReadOnly = true;
            this.textBoxMyGames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMyGames.Size = new System.Drawing.Size(211, 116);
            this.textBoxMyGames.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(451, 203);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 37);
            this.button1.TabIndex = 11;
            this.button1.Text = "Recomend Me a Game";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainWindowsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 384);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxMyGames);
            this.Controls.Add(this.textBoxPlayerCharacteristics);
            this.Controls.Add(this.textBoxRecommendedGames);
            this.Controls.Add(this.textBoxSteamName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainWindowsForm";
            this.Text = "MainWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxSteamName;
        private System.Windows.Forms.TextBox textBoxRecommendedGames;
        private System.Windows.Forms.TextBox textBoxPlayerCharacteristics;
        private System.Windows.Forms.TextBox textBoxMyGames;
        private System.Windows.Forms.Button button1;
    }
}