using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Flowar
{
    public class MapTile
    {
        public int Id { get; set; }
        public String ContentName { get; set; }
        public float Rotation { get; set; }
        public Vector2 OffsetPosition { get; set; }
        
        public MapTile(int id, string contentName, float rotation, Vector2 offsetPosition)
        {
            this.Id = id;
            this.ContentName = contentName;
            this.Rotation = rotation;
            this.OffsetPosition = offsetPosition;
        }
    }
}
