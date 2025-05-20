using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GravityFlip
{
    public partial class MenuForm : Form
    {
        private Form1 gameForm; 

        public MenuForm()
        {
            InitializeComponent(); 
            this.Text = "Gravity Flip - Меню";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var background = new PictureBox
            {
                Image = Image.FromFile("C:\\Users\\Gosha\\source\\repos\\GravityFlip\\GravityFlip\\Resources\\background.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(background);

            AddButtons(background);
        }

        private void AddButtons(Control parent)
        {
            var playButton = new Button
            {
                Text = "Играть",
                Font = new Font("Arial", 16),
                Size = new Size(200, 50),
                Location = new Point(300, 200),
                BackColor = Color.DeepSkyBlue,
                FlatStyle = FlatStyle.Flat
            };

            playButton.Click += (s, e) =>
            {
                this.Hide();
                new Form1().Show();
            };

            var tutorialButton = new Button
            {
                Text = "Обучение",
                Font = new Font("Arial", 16),
                Size = new Size(200, 50),
                Location = new Point(300, 280),
                BackColor = Color.Gold,
                FlatStyle = FlatStyle.Flat
            };
            tutorialButton.Click += TutorialButton_Click;

            var exitButton = new Button
            {
                Text = "Выход",
                Font = new Font("Arial", 16),
                Size = new Size(200, 50),
                Location = new Point(300, 360),
                BackColor = Color.OrangeRed,
                FlatStyle = FlatStyle.Flat
            };
            exitButton.Click += (sender, args) => Application.Exit();

            parent.Controls.Add(playButton);
            parent.Controls.Add(tutorialButton);
            parent.Controls.Add(exitButton);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            gameForm = new Form1();
            gameForm.FormClosed += (s, args) => this.Close();
            this.Hide();
            gameForm.Show();
        }

        private void TutorialButton_Click(object sender, EventArgs e)
        {
            ShowTutorial();
        }

        private void ShowTutorial()
        {
            var tutorialForm = new Form
            {
                Text = "Обучение",
                ClientSize = new Size(600, 400),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var tutorialImage = new PictureBox
            {
                Image = Image.FromFile("C:\\Users\\Gosha\\source\\repos\\GravityFlip\\GravityFlip\\Resources\\tutorial.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };

            var backButton = new Button
            {
                Text = "Назад",
                Size = new Size(100, 30),
                Location = new Point(250, 350)
            };
            backButton.Click += (sender, args) => tutorialForm.Close();

            tutorialForm.Controls.Add(tutorialImage);
            tutorialForm.Controls.Add(backButton);
            tutorialForm.ShowDialog();
        }
    }
}
