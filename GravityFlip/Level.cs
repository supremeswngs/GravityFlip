using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GravityFlip.LevelManager;

namespace GravityFlip
{
    public class Level
    {
        private Rectangle _bounds;
        
        public Point StartPosition { get; set; }
        private Point _startPosition;
        public int Number { get; set; }
        public string Name { get; set; }
        public List<Platform> Platforms { get; set; } = new List<Platform>();

        public bool IsCompleted { get; set; }
        public Platform Door { get; private set; }
        public void AddDoor(Platform door)
        {
            Door = door;
            Platforms.Add(door);
        }

        public void AddPlatform(Platform platform)
        {
            Platforms.Add(platform);
        }
    }

    public class LevelManager
    {
        private List<Level> _levels = new List<Level>();
        private int _currentLevelIndex = 0;

        public Level CurrentLevel => _levels[_currentLevelIndex];
        public int LevelCount => _levels.Count;

        private readonly int _baseWidth;
        private readonly int _baseHeight;

        public LevelManager(int baseWidth, int baseHeight)
        {
            _baseWidth = baseWidth;
            _baseHeight = baseHeight;
        }

        public void Initialize()
        {
            _levels.Clear();

            var level1 = new Level
            {
                Number = 1,
                Name = "Гравитационные основы",
                StartPosition = new Point(50, 620),
                Platforms =
                {
                    new Platform(0, 660, 300, 20, Color.Green),         // Пол
                    new Platform(0, 0, 1280, 20, Color.Green),           // Верхняя граница
                    new Platform(350, 550, 100, 15, Color.Blue, true),   // 1-я платформа
                    new Platform(550, 480, 100, 15, Color.Blue, true),   // 2-я платформа
                    new Platform(750, 410, 100, 15, Color.Blue, true),   // 3-я платформа
                    new Platform(950, 350, 100, 15, Color.Blue, true),   // 4-я платформа
                    new Platform(1150, 300, 100, 15, Color.Blue, true),  // 5-я платформа

                    new Platform(900, 0, 20, 200, Color.Blue, true),     // Вертикальная платформа

                    new Platform(1050, 150, 150, 15, Color.Red, true),   // Красная платформа 

                    new Platform(1200, 150, 40, 100, Color.Gold),        // Дверь
                }
            };

            var level2 = new Level
            {
                Number = 2,
                Name = "Точные прыжки",
                StartPosition = new Point(100, 300),
                Platforms =
                {
                    new Platform(0, 660, 280, 20, Color.Green),           // Нижняя платформа (пол)
                    new Platform(0, 0, 280, 20, Color.Green),             // Верхняя граница слева

                    new Platform(300, 550, 100, 15, Color.Blue, true),    // Синяя платформа 1
                    new Platform(800, 430, 100, 15, Color.Blue, true),    // Синяя платформа 2

                    new Platform(550, 300, 180, 15, Color.Red, true),     // Красная платформа 1
                    new Platform(1050, 180, 180, 15, Color.Red, true),    // Красная платформа 2

                    new Platform(1200, 180, 40, 100, Color.Gold),         // Дверь
                }
            };

            var level3 = new Level
            {
                Number = 3,
                Name = "Прыжковый вызов",
                StartPosition = new Point(50, 600),
                Platforms =
                {
                    new Platform(0, 660, 300, 20, Color.Green),              // Пол
                    new Platform(0, 0, 300, 20, Color.Green),                // Верхняя граница слева

                    new Platform(350, 570, 100, 15, Color.Blue, true),       // Синяя платформа 1
                    new Platform(550, 500, 100, 15, Color.Red, true),        // Красная
                    new Platform(750, 430, 100, 15, Color.Blue, true),       // Синяя платформа 2
                    new Platform(950, 360, 100, 15, Color.Red, true),        // Красная
                    new Platform(1150, 280, 100, 15, Color.Blue, true),      // Синяя платформа 3

                    new Platform(1230, 180, 40, 100, Color.Gold),            // Дверь (высоко)
                }
            };

            var level4 = new Level
            {
                Number = 4,
                Name = "Смешанные вызовы",
                StartPosition = new Point(100, 300),
                Platforms =
                {
                    new Platform(0, 620, 300, 20, Color.Green),             // Стартовая платформа
                    new Platform(350, 680, 100, 15, Color.Blue, true),      // Нижняя синяя
                    new Platform(1100, 700, 100, 15, Color.Blue, true),      // Центральная синяя
                    new Platform(850, 100, 100, 15, Color.Red, true),       // Верхняя красная (опасность)
                    new Platform(1180, 600, 40, 100, Color.Gold)
                }
            };

            _levels.Add(level1);
            _levels.Add(level2);
            _levels.Add(level3);
            _levels.Add(level4);

        }

        public bool LoadNextLevel()
        {
            if (_currentLevelIndex < _levels.Count - 1)
            {
                _currentLevelIndex++;
                return true;
            }
            return false;
        }

        public static class MathHelper
        {
            public static int Clamp(int value, int min, int max)
            {
                return (value < min) ? min : (value > max) ? max : value;
            }

            public static float Clamp(float value, float min, float max)
            {
                return (value < min) ? min : (value > max) ? max : value;
            }
        }
    }
}
