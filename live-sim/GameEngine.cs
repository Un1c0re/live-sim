using System;

namespace live_sim
{
    internal class GameEngine
    {
        public enum CellState { Dead = 0, Alive = 1, Aging = 2 }

        public class Cell
        {
            public CellState State  { get; set; }
            public int LifeTime     { get; set; }
            public int MaxLifeTime  { get; set; }

            public Cell(CellState state, int lifeTime, int maxLifeTime)
            {
                State       = state;
                LifeTime    = 0;
                MaxLifeTime = maxLifeTime;
            }

            public Cell(Cell other)
            {
                State = other.State;
                LifeTime = other.LifeTime;
                MaxLifeTime = other.MaxLifeTime;
            }
        }

        private Cell[,] field;
        public uint CurrentGeneration { get; private set; }

        private readonly int rows;
        private readonly int cols;

        private string  birthRule;
        private string  survivalRule;
        private int     agingThreshold;

        public GameEngine(int rows, int cols, int density, string rules)
        {
            this.cols = cols;
            this.rows = rows;
            field = new Cell[cols, rows];

            ParseRules(rules);

            Random random = new Random();

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    bool isCellAlive = random.Next(density) == 0;

                    field[x, y] = new Cell(
                        isCellAlive? CellState.Alive: CellState.Dead,
                        0,
                        agingThreshold
                    );
                }
            }
        }

        private void ParseRules(string rules)
        {
            var parts = rules.Split('/');

            birthRule       = parts[0].Substring(1);
            survivalRule    = parts[1].Substring(1);
            agingThreshold  = int.Parse(parts[2]);
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;

                    bool isSelfChecking = col == x && row == y;
                    bool hasLife = field[col, row].State == CellState.Alive;

                    if (hasLife && !isSelfChecking)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void NextGeneration()
        {
            var newField = new Cell[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Cell cell = new Cell(field[x, y]);
                    string neighboursCount = CountNeighbours(x, y).ToString();

                    if (cell.State == CellState.Alive)
                    {
                        if (!survivalRule.Contains(neighboursCount))
                        {
                            if (cell.LifeTime >= cell.MaxLifeTime)
                            {
                                cell.State = CellState.Dead;
                            }
                            else
                            {
                                cell.State = CellState.Aging;
                            }
                        }
                    }
                    else if (cell.State == CellState.Dead)
                    {
                        if (birthRule.Contains(neighboursCount))
                        {
                            cell.State = CellState.Alive;
                            cell.LifeTime = 0;
                        }
                    }
                    else if (cell.State == CellState.Aging)
                    {
                        cell.LifeTime++;
                        if (cell.LifeTime >= cell.MaxLifeTime)
                        {
                            cell.State = CellState.Dead;
                        }
                    }

                    newField[x, y] = new Cell(cell);
                }
            }

            field = newField;
            CurrentGeneration++;
        }

        public Cell[,] GetCurrentGeneration()
        {
            var result = new Cell[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    result[x, y] = new Cell(field[x, y]);
                }
            }

            return result;
        }

        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void UpdateCell(int x, int y, Cell cell)
        {
            if (ValidateCellPosition(x, y))
                field[x, y] = cell;
        }

        public void AddCell(int x, int y)
        {
            UpdateCell(x, y, new Cell(CellState.Alive, 0, agingThreshold));
        }

        public void RemoveCell(int x, int y)
        {
            UpdateCell(x, y, new Cell(CellState.Dead, 0,agingThreshold));
        }
    }
}
