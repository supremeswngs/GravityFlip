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
    public partial class Form1 : Form
    {
        private Player player; 
        private Timer gameLoop;  
        private List<Platform> platforms = new List<Platform>();
        private Rectangle bounds;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(800, 600);
            this.DoubleBuffered = true;

            bounds = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);

            player = new Player(bounds)
            {
                X = 200,
                Y = 300
            };

            platforms = new List<Platform>
            {
                new Platform(0, 550, 800, 20, Color.Green),
                new Platform(0, 0, 800, 20, Color.Green),
                new Platform(100, 400, 200, 20, Color.Blue, true),
                new Platform(400, 300, 200, 20, Color.Red, true),
                new Platform(200, 200, 200, 20, Color.Blue, true)
            };

            gameLoop = new Timer();
            gameLoop.Interval = 16;
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }

        private void CheckCollisions()
        {
            player.IsGrounded = false;

            if (player.IsGravityNormal)
            {
                if (player.Y + player.Height > bounds.Bottom)
                {
                    player.Y = bounds.Bottom - player.Height;
                    player.IsGrounded = true;
                    player.verticalVelocity = 0;
                }
            }
            else
            {
                if (player.Y < 0)
                {
                    player.Y = 0;
                    player.IsGrounded = true;
                    player.verticalVelocity = 0;
                }
            }

            foreach (var platform in platforms)
            {
                if (!platform.IsActive(player.IsGravityNormal))
                    continue;

                if (player.X < platform.X + platform.Width &&
                    player.X + player.Width > platform.X &&
                    player.Y < platform.Y + platform.Height &&
                    player.Y + player.Height > platform.Y)
                {
                    float overlapTop = player.Y + player.Height - platform.Y;
                    float overlapBottom = platform.Y + platform.Height - player.Y;
                    float overlapLeft = player.X + player.Width - platform.X;
                    float overlapRight = platform.X + platform.Width - player.X;

                    float minOverlap = Math.Min(Math.Min(overlapTop, overlapBottom),
                                              Math.Min(overlapLeft, overlapRight));

                    if (minOverlap == overlapTop && player.verticalVelocity > 0)
                    {
                        player.Y = platform.Y - player.Height;
                        player.IsGrounded = true;
                        player.verticalVelocity = 0;
                    }
                    else if (minOverlap == overlapBottom && player.verticalVelocity < 0)
                    {
                        player.Y = platform.Y + platform.Height;
                        player.IsGrounded = true;
                        player.verticalVelocity = 0;
                    }
                    else if (minOverlap == overlapLeft)
                    {
                        player.X = platform.X - player.Width;
                    }
                    else if (minOverlap == overlapRight)
                    {
                        player.X = platform.X + platform.Width;
                    }
                }
            }
        }

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            bool flipGravity = Keyboard.IsKeyDown(Keys.G);
            bool moveLeft = Keyboard.IsKeyDown(Keys.A) || Keyboard.IsKeyDown(Keys.Left);
            bool moveRight = Keyboard.IsKeyDown(Keys.D) || Keyboard.IsKeyDown(Keys.Right);
            bool jump = Keyboard.IsKeyDown(Keys.Space);

            if (Keyboard.IsKeyDown(Keys.G))
                player.FlipGravity();

            player.Update(moveLeft, moveRight, jump);
            CheckCollisions();

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(Color.White);

            foreach (var platform in platforms)
            {
                if (platform.IsActive(player.IsGravityNormal))
                {
                    using (var brush = new SolidBrush(platform.Color))
                    {
                        e.Graphics.FillRectangle(brush,
                            platform.X, platform.Y,
                            platform.Width, platform.Height);
                    }
                }
            }

            using (var playerBrush = new SolidBrush(Color.Blue))
            {
                e.Graphics.FillRectangle(playerBrush, player.X, player.Y, player.Width, player.Height);

                int eyeY = player.IsGravityNormal ? (int)player.Y + 5 : (int)player.Y + player.Height - 15;
                e.Graphics.FillEllipse(Brushes.White, player.X + 5, eyeY, 8, 8);
                e.Graphics.FillEllipse(Brushes.White, player.X + player.Width - 13, eyeY, 8, 8);
            }

            string debugInfo = $"Pos: {player.X:F0},{player.Y:F0} Gravity: {(player.IsGravityNormal ? "Normal" : "Reversed")}";
            e.Graphics.DrawString(debugInfo, this.Font, Brushes.Black, 10, 10);
        }

        private static class Keyboard
        {
            private static readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();

            public static bool IsKeyDown(Keys key) => pressedKeys.Contains(key);
            public static void KeyDown(Keys key) => pressedKeys.Add(key);
            public static void KeyUp(Keys key) => pressedKeys.Remove(key);
        }

        protected override void OnKeyDown(KeyEventArgs e) => Keyboard.KeyDown(e.KeyCode);
        protected override void OnKeyUp(KeyEventArgs e) => Keyboard.KeyUp(e.KeyCode);

    }
}

