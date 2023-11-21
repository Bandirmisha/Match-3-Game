
using Match_3.GameObjects;
using Match_3.Enums;

namespace Match_3
{
    public partial class Game : Form
    {
        public int width, height;
        private int secondsLeft;
        private int score;
        private int frameNumber;
        private static Random random = new Random();
        private readonly MainMenu mainMenu;
        private static System.Windows.Forms.Timer timer = new();
        private Cell[,] field;
        private Cell? firstPressedCell;
        private Cell? secondPressedCell;
        private List<Destroyer> destroyers;

        public Game(MainMenu mainMenu, int w, int h)
        {
            InitializeComponent();

            this.mainMenu = mainMenu;
            secondsLeft = 60;
            score = 0;
            frameNumber = 0;

            width = w;
            height = h;
            field = new Cell[w, h];

            firstPressedCell = null;
            secondPressedCell = null;

            destroyers = new List<Destroyer>();
        }

        /// <summary>
        /// Ручной запуск игры
        /// </summary>
        public void Run()
        {
            GenerateField();

            timer.Tick += new EventHandler(FixedUpdate);
            timer.Interval = 25; //40 кадров в секунду
            timer.Start();
        }


        /// <summary>
        /// Имитация FixedUpdate с frameRate = 40 (указан в Run())
        /// </summary>
        private void FixedUpdate(object? obj, EventArgs eventArgs)
        {
            DrawStats();

            //Каждую секунду
            if (frameNumber % 40 == 0)
            {
                if (secondsLeft > 0)
                {
                    secondsLeft -= 1;
                }
                else
                {
                    timer.Tick -= new EventHandler(FixedUpdate);
                    timer.Stop();
                    timer.Dispose();
                    GameOver();
                }

            }
            
            frameNumber++;
        }


        /// <summary>
        /// Отрисовка статистики (Счёт и оставшееся время)
        /// </summary>
        private void DrawStats()
        {
            timeLeftTB.Text = "Time left: " + secondsLeft.ToString() + " sec";
            scoreTB.Text = "Score: " + score.ToString();
        }


        /// <summary>
        /// Генерация поля
        /// </summary>
        private void GenerateField()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    do
                    {
                        field[i, j] = new(new Point(i * 100, j * 100));
                    }
                    // Исключение одинаковости соседних клеток и появления комбинаций в начале игры
                    // Сделал для того, чтобы игрок не получал очки в начале игры
                    // МИНУС - на карте не будет 2х одинаковых соседей
                    while (i != 0 && field[i - 1, j].type == field[i, j].type ||
                           j != 0 && field[i, j - 1].type == field[i, j].type);

                    field[i, j].Click += new EventHandler(OnCellClick);
                    this.Controls.Add(field[i, j]);
                }
            }
        }


        /// <summary>
        ///  Событие, возникающее при клике на клетку
        /// </summary>
        private async void OnCellClick(object? sender, EventArgs eventArgs)
        {
            if (sender is not null)
            {
                Point pos = (sender as Cell).currentPosition;
                int x = pos.X / 100;
                int y = pos.Y / 100;

                //Блокировка лишних нажатий
                if (firstPressedCell != null && secondPressedCell != null)
                    return;


                if (firstPressedCell == null)
                {
                    firstPressedCell = field[x, y];
                }
                else secondPressedCell = field[x, y];

                //Если одна и та же клетка нажата
                if (firstPressedCell == secondPressedCell)
                {
                    secondPressedCell = null;
                    return;
                }

                if (secondPressedCell != null)
                {
                    Point firstPressedCellPos = firstPressedCell.currentPosition;
                    Point secondPressedCellPos = secondPressedCell.currentPosition;

                    //Если обе клетки- соседи
                    if (firstPressedCellPos.X - secondPressedCellPos.X == 100 && firstPressedCellPos.Y == secondPressedCellPos.Y ||
                        secondPressedCellPos.X - firstPressedCellPos.X == 100 && firstPressedCellPos.Y == secondPressedCellPos.Y ||
                        firstPressedCellPos.Y - secondPressedCellPos.Y == 100 && firstPressedCellPos.X == secondPressedCellPos.X ||
                        secondPressedCellPos.Y - firstPressedCellPos.Y == 100 && firstPressedCellPos.X == secondPressedCellPos.X)
                    {
                        firstPressedCell.SetOriginalOptions();
                        secondPressedCell.SetOriginalOptions();

                        //Изменение положения клеток
                        Swap(firstPressedCellPos, secondPressedCellPos); //в матрице
                        firstPressedCell.currentPosition = secondPressedCellPos;
                        secondPressedCell.currentPosition = firstPressedCellPos;

                        //Анимация
                        secondPressedCell.BringToFront();
                        secondPressedCell.Move(firstPressedCellPos); //Без await, чтобы анимация firstPressedCell началась сразу
                        await firstPressedCell.Move(secondPressedCellPos);

                        //Поиск комбинаций
                        List<Cell>? combination1 = FindCombination(secondPressedCell);
                        List<Cell>? combination2 = FindCombination(firstPressedCell);

                        if (combination1 != null || combination2 != null)
                        {
                            if (combination1 != null)
                                await RemoveSingleCombination(combination1);

                            if (combination2 != null)
                                await RemoveSingleCombination(combination2);

                            await CheckForHoles();
                        }
                        else
                        {
                            //Обратное изменение положения клеток
                            Swap(firstPressedCellPos, secondPressedCellPos); //в матрице
                            firstPressedCell.currentPosition = firstPressedCellPos;
                            secondPressedCell.currentPosition = secondPressedCellPos;

                            //Анимация
                            secondPressedCell.Move(secondPressedCellPos);
                            await firstPressedCell.Move(firstPressedCellPos);
                        }

                        firstPressedCell = null;
                        secondPressedCell = null;

                    }
                    else
                    {
                        firstPressedCell.SetOriginalOptions();

                        firstPressedCell = secondPressedCell;
                        firstPressedCell.PressedAnimationStart();

                        secondPressedCell = null;
                    }
                }
                else
                {
                    if (!firstPressedCell.isPressed)
                        firstPressedCell.PressedAnimationStart();
                }

            }
        }


        /// <summary>
        /// Меняет местами клетки в массиве Field
        /// </summary>
        private void Swap(Point pos1, Point pos2)
        {
            int x1 = pos1.X / 100;
            int y1 = pos1.Y / 100;
            int x2 = pos2.X / 100;
            int y2 = pos2.Y / 100;

            (field[x1, y1], field[x2, y2]) = (field[x2, y2], field[x1, y1]);
        }


        /// <summary>
        /// Поиск комбинации вокруг клетки
        /// </summary>
        private List<Cell>? FindCombination(Cell cell)
        {
            //if (cell == null) return null;

            Point pos = cell.currentPosition;
            int x = pos.X / 100;
            int y = pos.Y / 100;

            List<Cell>? horizontalCombination = new();
            List<Cell>? verticalCombination = new();

            horizontalCombination.Add(cell);
            verticalCombination.Add(cell);

            //поиск влево
            for (int i = 1; i < 8; i++)
            {
                if (x - i >= 0)
                {
                    if (cell.BackColor == field[x - i, y].BackColor)
                    {
                        horizontalCombination.Add(field[x - i, y]);
                    }
                    else break;
                }
            }

            //поиск вправо
            for (int i = 1; i < 8; i++)
            {
                if (x + i < width)
                {
                    if (cell.BackColor == field[x + i, y].BackColor)
                    {
                        horizontalCombination.Add(field[x + i, y]);
                    }
                    else break;
                }
            }

            //поиск вверх
            for (int i = 1; i < 8; i++)
            {
                if (y - i >= 0)
                {
                    if (cell.BackColor == field[x, y - i].BackColor)
                    {
                        verticalCombination.Add(field[x, y - i]);
                    }
                    else break;
                }
            }

            //поиск вниз
            for (int i = 1; i < 8; i++)
            {
                if (y + i < height)
                {
                    if (cell.BackColor == field[x, y + i].BackColor)
                    {
                        verticalCombination.Add(field[x, y + i]);
                    }
                    else break;
                }
            }


            if (horizontalCombination.Count >= 3)
            {
                // Г - образная фигура
                if (verticalCombination.Count >= 3)
                {
                    verticalCombination.RemoveAt(0);
                    return horizontalCombination.Concat(verticalCombination).ToList();
                }

                return horizontalCombination;
            }

            if (verticalCombination.Count >= 3)
            {
                return verticalCombination;
            }

            return null;
        }


        /// <summary>
        /// Поиск "пустот" для заполнения клетками
        /// </summary>
        private async Task CheckForHoles()
        {
            List<Cell> emptyCells = new List<Cell>();
            bool emptyCellsFilled = false;

            for (int i = 0; i < width; i++)
            {
                
                for (int j = height - 1; j >= 0; j--)
                {
                    if (field[i, j].type == CellType.none)
                    {
                        emptyCells.Add(field[i, j]);
                    }
                }

                if (emptyCells.Count != 0)
                {
                    emptyCellsFilled = true;
                    await PutDownUpperElements(emptyCells);
                }

                emptyCells.Clear();
            }

            if (emptyCellsFilled)
                await RemoveAllCombinations();
        }


        /// <summary>
        /// Поиск и удаление всех комбинаций на поле
        /// </summary>
        private async Task RemoveAllCombinations()
        {
            List<List<Cell>> combinations = new();
            List<Task> tasks = new();

            for (int i = 0; i < width; i += 2)
            {
                for (int j = 0; j < height; j++)
                {
                    List<Cell>? combination = FindCombination(field[i, j]);
                    if (combination != null)
                    {
                        combinations.Add(combination); 
                    }
                }
            }

            foreach (var combination in combinations)
            {
                tasks.Add(RemoveSingleCombination(combination));
            }

            await Task.WhenAll(tasks);

            if (combinations.Count > 0)
                await CheckForHoles();
        }


        /// <summary>
        /// Удаление одной комбинации
        /// </summary>
        private async Task RemoveSingleCombination(List<Cell> combination)
        {
            List<Task> tasks = new();
            bool bonusCreated = false;

            // Line
            if (combination.Count == 4 &&
                combination[0].type != CellType.vLine &&
                combination[0].type != CellType.hLine)
            {
                switch (random.Next(1, 3))
                {
                    case 1: combination[0].SetHLineType(); break;
                    case 2: combination[0].SetVLineType(); break;
                }
                bonusCreated = true;
            }

            // Bomb
            else if (combination.Count >= 5 &&
                     combination[0].type != CellType.bomb)
            {
                combination[0].SetBombType();
                bonusCreated = true;
            }

            foreach (Cell cell in combination)
            {
                if (bonusCreated) //пропуск клетки, на которой только добавили бонус
                {
                    bonusCreated = false;
                    continue;
                }

                tasks.Add(RemoveCell(cell, 250));
            }

            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Удаление клетки и получение очков
        /// </summary>
        private async Task RemoveCell(Cell cell,int delay)
        {
            score++;

            if (cell.type == CellType.bomb)
            {
                await BombExplode(cell);
            }
            else if (cell.type == CellType.hLine)
            {
                await HLineExplode(cell);
            }
            else if (cell.type == CellType.vLine)
            {
                await VLineExplode(cell);
            }
            else
            {
                await cell.Destroy(delay);
            }
        }


        /// <summary>
        /// Эффект активации Bomb
        /// </summary>
        private async Task BombExplode(Cell bomb)
        {
            List<Task> tasks = new();
            List<Cell> cells = new();
            Point bombPos = bomb.currentPosition;
            int x = bombPos.X / 100;
            int y = bombPos.Y / 100;

            bomb.SetNoneType();
            bomb.Destroy(250);

            if (x - 1 >= 0)
            {
                if (!field[x - 1, y].isRemoving)
                {
                    cells.Add(field[x - 1, y]);
                }

                if (y - 1 > 0)
                {
                    if (!field[x, y - 1].isRemoving)
                    {
                        cells.Add(field[x, y - 1]);
                    }

                    if (!field[x - 1, y - 1].isRemoving)
                    {
                        cells.Add(field[x - 1, y - 1]);
                    }
                }

                if (y + 1 < height)
                {
                    if (!field[x, y + 1].isRemoving)
                    {
                        cells.Add(field[x, y + 1]);
                    }

                    if (!field[x - 1, y + 1].isRemoving)
                    {
                        cells.Add(field[x - 1, y + 1]);
                    }
                }
            }

            if (x + 1 < width)
            {
                if (!field[x + 1, y].isRemoving)
                {
                    cells.Add(field[x + 1, y]);
                }

                if (y - 1 > 0)
                {
                    if (!field[x + 1, y - 1].isRemoving)
                    {
                        cells.Add(field[x + 1, y - 1]);
                    }
                }

                if (y + 1 < height)
                {
                    if (!field[x + 1, y + 1].isRemoving)
                    {
                        cells.Add(field[x + 1, y + 1]);
                    }
                }
            }


            foreach (Cell cell in cells)
            {
                tasks.Add(RemoveCell(cell, 250));
            }

            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Эффект активации горизонтального Line
        /// </summary>
        private async Task HLineExplode(Cell line)
        {
            List<Task> tasks = new();

            Point pos = line.currentPosition;
            CreateDestroyer(pos, Direction.left);
            CreateDestroyer(pos, Direction.right);

            int x = pos.X / 100;
            int y = pos.Y / 100;

            line.SetNoneType();
            line.Destroy(250);

            int count = 1;
            while (x < width - 1)
            {
                x++;
                tasks.Add(RemoveCell(field[x, y], count * 250));
                count++;
            }

            x = pos.X / 100;
            count = 1;
            while (x > 0)
            {
                x--;
                tasks.Add(RemoveCell(field[x, y], count * 250));
                count++;
            }

            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Эффект активации вертикального Line
        /// </summary>
        private async Task VLineExplode(Cell line)
        {
            List<Task> tasks = new();

            Point pos = line.currentPosition;
            CreateDestroyer(pos, Direction.up);
            CreateDestroyer(pos, Direction.down);

            int x = pos.X / 100;
            int y = pos.Y / 100;

            line.SetNoneType();
            line.Destroy(250);

            int count = 1;
            while (y < height - 1)
            {
                y++;
                tasks.Add(RemoveCell(field[x, y], count * 250));
                count++;
            }

            y = pos.Y / 100;
            count = 1;
            while (y > 0)
            {
                y--;
                tasks.Add(RemoveCell(field[x, y], count * 250));
                count++;
            }

            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Создание Destroyer из бонуса Line
        /// </summary>
        private void CreateDestroyer(Point position, Direction dir)
        {
            destroyers.Add(new(position, dir));
            Controls.Add(destroyers.Last());
            destroyers.Last().BringToFront();
            destroyers.Last().Fly();
        }

       
        /// <summary>
        /// Спуск имеющихся верхних клеток на места "пустот" и создание новых клеток
        /// </summary>
        private async Task PutDownUpperElements(List<Cell> emptyCells)
        {
            Point pos = emptyCells[emptyCells.Count - 1].currentPosition;
            int x = pos.X / 100;
            int y = pos.Y / 100; //стартовый индекс

            List<Cell> exchangers = new();
            List<Task> tasks = new();

            //между пустыми может оказаться бонус
            for (int i = 1; i <= emptyCells.Count-1; i++)
            {
                if (field[x, y + i].type == CellType.bomb ||
                    field[x, y + i].type == CellType.hLine ||
                    field[x, y + i].type == CellType.vLine)
                    exchangers.Add(field[x, y + i]);
            }

            //добавление имеющихся сверху клеток
            int ind = 1;
            while (y - ind >= 0)
            {
                exchangers.Add(field[x, y - ind]);
                ind++;
            }

            //добавление новых клеток из-за границы поля
            ind = 1;
            for (int i = 0; i < emptyCells.Count; i++)
            {
                exchangers.Add(new(new Point(x * 100, (0 - ind)*100)));
            }

            //замена и анимация
            pos = emptyCells[0].currentPosition;
            int startY = pos.Y / 100;
            if (exchangers.Count != 0)
            {
                for (int i = 0; i < exchangers.Count; i++)
                {
                    field[x, startY - i] = exchangers[i];
                    exchangers[i].currentPosition = new Point(x * 100, (startY - i) * 100);
                    exchangers[i].Click += new EventHandler(OnCellClick);
                    this.Controls.Add(exchangers[i]);

                    tasks.Add(exchangers[i].Move(new Point(x * 100, (startY - i) * 100)));
                }
            }

            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Заканчивает игру
        /// </summary>
        private void GameOver()
        {
            MessageBox.Show("Game over!");
            mainMenu.Show();
            this.Close();
        }
    }
}
