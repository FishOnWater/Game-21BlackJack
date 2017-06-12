﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace _21BlackJack.Cards_FrameWork.Utility
{
    public static class MathUtility
    {
        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            //Point relative to origin
            Vector2 u = point - origin;

            if (u == Vector2.Zero)
                return point;

            //Angle relative to origin
            float a = (float)Math.Atan2(u.Y, u.X);

            //Rotate
            a += rotation;

            //U is now the new point relative to origin
            u = u.Length() * new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
            return u + origin;
        }
    }
}