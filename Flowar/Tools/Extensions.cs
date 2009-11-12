using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public static class Extensions
{
    public static Vector2 Position(this MouseState mouseState)
    {
        return new Vector2(mouseState.X, mouseState.Y);
    }
}
