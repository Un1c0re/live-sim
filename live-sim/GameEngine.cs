using System;

namespace live_sim
{
    internal class GameEngine
    {
        private bool[,] field;
        public uint CurrentGeneration { get; private set; }

        private readonly int rows;
        private readonly int cols;

        private string birthRule;
        private string survivalRule;

        public GameEngine(int rows, int cols, int density, string rules)
        {
            this.rows = rows;
            this.cols = cols;
            field = new bool[cols, rows];
            ParseRules(rules);

            Random random = new Random();

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next(density) == 0;
                }
            }
        }

        private void ParseRules(string rules)
        {
            var parts = rules.Split('/');
            birthRule = parts[0].Substring(1);
            survivalRule = parts[1].Substring(1);
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++;
                }
            }

            return count;
        }

        public void NextGeneration()
        {
            var newField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && birthRule.Contains(neighboursCount.ToString()))
                        newField[x, y] = true;
                    else if (hasLife && !survivalRule.Contains(neighboursCount.ToString()))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];
                }
            }
            field = newField;
            CurrentGeneration++;
        }

        public bool[,] GetCurrentGeneration()
        {

            var result = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                    result[x, y] = field[x, y];

            return result;
        }

        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void UpdateCell(int x, int y, bool state)
        {
            if (ValidateCellPosition(x, y))
                field[x, y] = state;
        }

        public void AddCell(int x, int y)
        {
            UpdateCell(x, y, true);
        }

        public void RemoveCell(int x, int y)
        {
            UpdateCell(x, y, false);
        }
    }
}
