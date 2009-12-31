using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flowar
{
    public class MapTile
    {
        public int Id { get; set; }
        public String ContentName { get; set; }
        public float Rotation { get; set; }
        public Vector2 OffsetPosition { get; set; }
        public Color Color { get; set; }

        public MapTile(int id, string contentName, float rotation, Vector2 offsetPosition)
        {
            Constructor(id, contentName, rotation, offsetPosition, Color.White);
        }

        public MapTile(int id, string contentName, float rotation, Vector2 offsetPosition, Color color)
        {
            Constructor(id, contentName, rotation, offsetPosition, color);

        }

        private void Constructor(int id, string contentName, float rotation, Vector2 offsetPosition, Color color)
        {
            this.Id = id;
            this.ContentName = contentName;
            this.Rotation = rotation;
            this.OffsetPosition = offsetPosition;
            this.Color = color;
        }
    }
}
