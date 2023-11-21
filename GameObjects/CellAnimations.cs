using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match_3.GameObjects
{
    public partial class Cell
    {

        /// <summary>
        /// Анимация эффекта нажатия на клетку
        /// </summary>
        private async Task PressedAnimation()
        {
            while (isPressed)
            {
                if (isIncreases)
                {
                    if (Size.Width < 120 && Size.Height < 120)
                    {
                        Size = new Size(Size.Width + 2, Size.Height + 2);
                        Location = new Point(Location.X - 1, Location.Y - 1);
                    }
                    else isIncreases = false;
                }

                if (!isIncreases)
                {
                    if (Size.Width > 100 && Size.Height > 100)
                    {
                        Size = new Size(Size.Width - 2, Size.Height - 2);
                        Location = new Point(Location.X + 1, Location.Y + 1);
                    }
                    else isIncreases = true;
                }

                await Task.Delay(25);
            }
        }

        /// <summary>
        /// Анимация перемещения клетки
        /// </summary>
        private async Task MotionAnimation(Point targetPosition)
        {
            isMoving = true;

            while (Location != targetPosition)
            {
                if (Location.X > targetPosition.X && Location.X - targetPosition.X > 10)
                    Location = new Point(Location.X - 10, Location.Y);
                else if (Location.X < targetPosition.X && targetPosition.X - Location.X > 10)
                    Location = new Point(Location.X + 10, Location.Y);
                else if (Location.Y > targetPosition.Y && Location.Y - targetPosition.Y > 10)
                    Location = new Point(Location.X, Location.Y - 10);
                else if (Location.Y < targetPosition.Y && targetPosition.Y - Location.Y > 10)
                    Location = new Point(Location.X, Location.Y + 10);
                else
                    Location = targetPosition;

                await Task.Delay(25);
            }

            isMoving = false;
        }

        /// <summary>
        /// Анимация удаления клетки
        /// </summary>
        private async Task RemovingAnimation(int delay)
        {
            await Task.Delay(delay);

            while (Size.Width != 0 && Size.Height != 0)
            {
                Size = new Size(Size.Width - 10, Size.Height - 10);
                Location = new Point(Location.X + 5, Location.Y + 5);

                await Task.Delay(25);
            }
        }
    }
}
