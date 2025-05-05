using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GravityFlip
{
    public class Platform
    {
        public int X { get; set; }
        public int Y { get; set; } 
        public int Width { get; set; }
        public int Height { get; set; } 
        public Color Color { get; set; } 

        public bool IsVisible { get; set; } = true;
        public bool GravityDependent { get; set; }

        public Platform(int x, int y, int width, int height, Color color, bool gravityDependent = false)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            GravityDependent = gravityDependent;
        }

        public void Draw(Graphics g)
        {
            if (IsVisible)
            {
                using (var brush = new SolidBrush(Color))
                {
                    g.FillRectangle(brush, X, Y, Width, Height);
                }
            }
        }

        public bool CollidesWith(Player player)
        {
            return player.X < X + Width &&
                   player.X + player.Width > X &&
                   player.Y < Y + Height &&
                   player.Y + player.Height > Y;
        }
    }
}