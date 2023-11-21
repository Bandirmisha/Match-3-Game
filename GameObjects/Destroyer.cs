using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Match_3.Enums;

namespace Match_3.GameObjects
{
    public class Destroyer : PictureBox
    {
        private Direction direction;

        public Destroyer(Point startPos, Direction dir) 
        {
            Image = Image.FromFile("Images\\Destroyer.png");
            Location = startPos;
            direction = dir;
            Size = new Size(100, 100);
        }

        public async Task Fly()
        {
            while (true)
            {
                if (direction == Direction.left)
                {
                    if (Location.X > 0)
                        Location = new Point(Location.X - 10, Location.Y);
                    else Dispose();
                }

                else if (direction == Direction.right)
                {
                    if (Location.X < 800)
                        Location = new Point(Location.X + 10, Location.Y);
                    else Dispose();
                }

                else if (direction == Direction.up)
                {
                    if (Location.Y > 0)
                        Location = new Point(Location.X, Location.Y - 10);
                    else Dispose();
                }

                else if (direction == Direction.down)
                {
                    if (Location.Y < 800)
                        Location = new Point(Location.X, Location.Y + 10);
                    else Dispose();
                }

                await Task.Delay(25);
            }
        }
    }
}
