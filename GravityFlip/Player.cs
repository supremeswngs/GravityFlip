﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GravityFlip.LevelManager;

namespace GravityFlip
{
    public class Player
    {
        public float X, Y;
        public int Width = 25, Height = 35;
        public float Speed = 5f;
        public float JumpForce = 15f;
        public bool IsGravityNormal = true;
        public bool IsGrounded = false;
        public float GravityCooldown = 0f;
        public const float GravityDelay = 1f;

        private float gravity = 0.5f;
        public float verticalVelocity = 0;
        private Rectangle bounds;

        public Player(Rectangle gameBounds)
        {
            bounds = gameBounds;
        }

        public void Update(bool moveLeft, bool moveRight, bool jump)
        {
            X = MathHelper.Clamp(X, 0, bounds.Width - Width);
            Y = MathHelper.Clamp(Y, 0, bounds.Height - Height);
            if (GravityCooldown > 0)
            {
                GravityCooldown -= 1f / 60f;
            }

            if (moveLeft) X = Math.Max(0, X - Speed);
            if (moveRight) X = Math.Min(bounds.Width - Width, X + Speed);

            verticalVelocity += IsGravityNormal ? gravity : -gravity;
            Y += verticalVelocity;

            if (IsGrounded && jump)
            {
                verticalVelocity = IsGravityNormal ? -JumpForce : JumpForce;
                IsGrounded = false;
            }
        }

        public bool FlipGravity()
        {
            if (GravityCooldown <= 0f)
            {
                IsGravityNormal = !IsGravityNormal;
                verticalVelocity = 0;
                GravityCooldown = GravityDelay;
                return true;
            }
            return false;
        }
    }
}
