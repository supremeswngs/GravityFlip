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
        private LevelManager _levelManager;
        private Timer gameLoop;  
        private List<Platform> platforms = new List<Platform>();
        private Rectangle bounds;
        private int _notificationTimer = 0;
        private string _levelNotification = "";
        private const int NotificationDuration = 120;
        private const int BaseWidth = 1280;
        private const int BaseHeight = 720;
        private Font _notificationFont = new Font("Arial", 24, FontStyle.Bold);


        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(BaseWidth, BaseHeight);
            this.StartPosition = FormStartPosition.CenterScreen;

            _levelManager = new LevelManager(BaseWidth, BaseHeight);
            _levelManager.Initialize();

            LoadLevel(_levelManager.CurrentLevel);

            gameLoop = new Timer();
            gameLoop.Interval = 12;
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }
        
        private void ShowLevelNotification(string message)
        {
            _levelNotification = message;
            _notificationTimer = 120; 
        }

        private void LoadLevel(Level level)
        {
            bounds = new Rectangle(0, 0, BaseWidth, BaseHeight);

            player = new Player(bounds)
            {
                X = level.StartPosition.X,
                Y = level.StartPosition.Y,
                IsGravityNormal = true,
                verticalVelocity = 0
            };

            platforms = level.Platforms;
            this.Invalidate();
        }

        private void CheckLevelCompletion()
        {
            var door = _levelManager.CurrentLevel.Platforms
                .FirstOrDefault(p => p.Color == Color.Gold);

            if (door != null)
            {
                bool isNearDoor = Math.Abs((player.X + player.Width / 2) - (door.X + door.Width / 2)) < 50 &&
                                  Math.Abs((player.Y + player.Height / 2) - (door.Y + door.Height / 2)) < 50;

                if (isNearDoor)
                {
                    _levelManager.CurrentLevel.IsCompleted = true;

                    if (_levelManager.LoadNextLevel())
                    {
                        LoadLevel(_levelManager.CurrentLevel);
                        ShowNotification($"Уровень {_levelManager.CurrentLevel.Number}: {_levelManager.CurrentLevel.Name}");
                    }
                    else
                    {
                        ShowNotification("Вы прошли все уровни!");
                        Task.Delay(2000).ContinueWith(_ => this.Close(),
                            TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
            }
        }

        private bool CheckCollisionWithDoor(Platform door)
        {
            return player.X + player.Width > door.X &&
                   player.X < door.X + door.Width &&
                   player.Y + player.Height > door.Y &&
                   player.Y < door.Y + door.Height;
        }
        private void ShowNotification(string message)
        {
            _levelNotification = message;
            _notificationTimer = 120; 
        }

        private void CheckCollisions()
        {
            player.IsGrounded = false;

            if (player.Y + player.Height > bounds.Bottom || player.Y < bounds.Top)
            {
                player.X = _levelManager.CurrentLevel.StartPosition.X;
                player.Y = _levelManager.CurrentLevel.StartPosition.Y;
                player.verticalVelocity = 0;
                return;
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
            CheckLevelCompletion();

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(Color.White);

            foreach (var platform in platforms.Where(p => p.IsActive(player.IsGravityNormal)))
            {
                e.Graphics.FillRectangle(new SolidBrush(platform.Color),
                    platform.X, platform.Y,
                    platform.Width, platform.Height);
            }

            using (var playerBrush = new SolidBrush(Color.Blue))
            {
                e.Graphics.FillRectangle(playerBrush, player.X, player.Y, player.Width, player.Height);

                int eyeY = player.IsGravityNormal ? (int)player.Y + 5 : (int)player.Y + player.Height - 15;
                e.Graphics.FillEllipse(Brushes.White, player.X + 3, eyeY, 8, 8);
                e.Graphics.FillEllipse(Brushes.White, player.X + player.Width - 10, eyeY, 8, 8);
            }
            if (_notificationTimer > 0)
            {
                var size = e.Graphics.MeasureString(_levelNotification, _notificationFont);
                e.Graphics.DrawString(_levelNotification, _notificationFont, Brushes.White,
                    (this.ClientSize.Width - size.Width) / 2, 50);
                _notificationTimer--;
            }

            string debugInfo = $"Pos: {player.X:F0},{player.Y:F0} Gravity: {(player.IsGravityNormal ? "Normal" : "Reversed")}";
            e.Graphics.DrawString($"Уровень: {_levelManager.CurrentLevel.Number}",
            this.Font, Brushes.Black, 30, 30);
        }

        private static class Keyboard
        {
            private static readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();

            public static bool IsKeyDown(Keys key) => pressedKeys.Contains(key);
            public static void KeyDown(Keys key) => pressedKeys.Add(key);
            public static void KeyUp(Keys key) => pressedKeys.Remove(key);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            Keyboard.KeyDown(e.KeyCode);
        }
        protected override void OnKeyUp(KeyEventArgs e) => Keyboard.KeyUp(e.KeyCode);


    }

}

