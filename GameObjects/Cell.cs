
using Match_3.Enums;

namespace Match_3.GameObjects
{
    public partial class Cell : Button
    {
        public CellType type;
        public Point currentPosition;
        private readonly static Random random = new();

        public bool isPressed;
        public bool isRemoving;
        private bool isMoving;
        private bool isIncreases;

        public Cell(Point pos)
        {
            type = SetRandomType();
            currentPosition = pos;
            
            isPressed = false;
            isRemoving = false;
            isMoving = false;
            isIncreases = true;

            Size = new(100, 100);
            Location = pos;
            BackColor = ConvertTypeToColor();
        }

        /// <summary>
        /// Генерация случайного типа клетки (кроме бонусов и none)
        /// </summary>
        private CellType SetRandomType()
        {
            return random.Next(0, 5) switch
            {
                0 => CellType.red,
                1 => CellType.green,
                2 => CellType.yellow,
                3 => CellType.blue,
                4 => CellType.purple,
                _ => CellType.none
            };
        }

        /// <summary>
        /// Конвертация типа клетки в цвет
        /// </summary>
        private protected Color ConvertTypeToColor()
        {
            return type switch
            {
                CellType.red => Color.Red,
                CellType.green => Color.Green,
                CellType.yellow => Color.Yellow,
                CellType.blue => Color.Blue,
                CellType.purple => Color.Purple,
                _ => Color.White
            };
        }

        /// <summary>
        /// Установление типа бонус горизонтальный Line
        /// </summary>
        public void SetHLineType()
        {
            Image = Image.FromFile("Images\\hLine.png");
            type = CellType.hLine;
        }

        /// <summary>
        /// Установление типа бонус вертикальный Line
        /// </summary>
        public void SetVLineType()
        {
            Image = Image.FromFile("Images\\vLine.png");
            type = CellType.vLine;
        }

        /// <summary>
        /// Установление типа бонус Bomb
        /// </summary>
        public void SetBombType()
        {
            Image = Image.FromFile("Images\\bomb.png");
            type = CellType.bomb;
        }
        
        /// <summary>
        /// Установление нулевого типа (перед удалением)
        /// </summary>
        public void SetNoneType()
        {
            type = CellType.none;
        }

        /// <summary>
        /// Сброс всех параметров до начальных
        /// </summary>
        public void SetOriginalOptions()
        {
            isPressed = false;
            Size = new(100, 100);
            Location = currentPosition;
        }

        /// <summary>
        /// Запуск анимации эффекта нажатия на клетку
        /// </summary>
        public async Task PressedAnimationStart()
        {
            if (isPressed) { return; }

            BringToFront();
            isPressed = true;
            await PressedAnimation();
        }

        /// <summary>
        /// Запуск анимации перемещения клетки
        /// </summary>
        public new async Task Move(Point targetPosition) //new, так как VS ругается на скрытие Controls.Move параметра
        {
            if (isPressed) { isPressed = false; }
            while (isMoving) { await Task.Delay(100); } //ожидание, если анимация уже запущена

            await MotionAnimation(targetPosition);
        }

        /// <summary>
        /// Запуск анимации удаления клетки
        /// </summary>
        public async Task Destroy(int delay)
        {
            while (isMoving) { await Task.Delay(100); }
            
            SetNoneType();
            isRemoving = true;
            await RemovingAnimation(delay);
        }
    }
}
