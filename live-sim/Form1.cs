using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace live_sim
{
    public partial class Form1 : Form
    {

        private Graphics graphics;
        private int resolution;
        
        private bool[,] field;
        private int currentGeneration = 0;

        private int rows;
        private int cols;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled)
                return;

            currentGeneration = 0;
            Text = $"Generation {currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            resolution = (int)nudResolution.Value;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);


            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            field = new bool[cols, rows];

            Random random = new Random();
            for (int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            
            timer1.Start();
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;

            timer1.Stop();

            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for(int i = -1; i < 2; i ++)
            {
                for(int j = -1; j < 2; j ++)
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

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);

            var newField = new bool[cols, rows];

            for(int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                        newField[x,y] = true;
                    else if(hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x,y] = field[x, y];

                    if (hasLife)
                        graphics.FillRectangle(Brushes.Aquamarine, x * resolution, y * resolution, resolution, resolution);
                }
            }
            field = newField;
            Text = $"Generation {++currentGeneration}";

            pictureBox1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            if(e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;

                var vaildatePassed = ValidateMousePosition(x, y);
                if (vaildatePassed)
                    field[x, y] = true;
            }

            if(e.Button == MouseButtons.Right) 
            { 
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;

                var vaildatePassed = ValidateMousePosition(x, y);
                if (vaildatePassed)
                    field[x, y] = false;
            }
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }
    }
}
