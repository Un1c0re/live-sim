using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace live_sim
{
    public partial class Form1 : Form
    {

        private Graphics graphics;
        private int resolution;
        private Color selectedColor;
        private GameEngine engine;
        private const int maxInterval = 1000;

        public Form1()
        {
            InitializeComponent();
            selectedColor = colorDialog1.Color;
            timer1.Interval = maxInterval / trackBar1.Value;
        }

        private void StartGame(string rule)
        {
            if (timer1.Enabled)
                return;

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            RuleInput.Enabled = false;
            resolution = (int)nudResolution.Value;

            engine = new GameEngine
            (
                rows: pictureBox1.Height / resolution,
                cols: pictureBox1.Width / resolution,
                density: (int)nudDensity.Minimum + (int)nudDensity.Maximum - (int)nudDensity.Value,
                rules: rule
            );

            Text = $"Generation {engine.CurrentGeneration}";

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);

            timer1.Start();
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;

            timer1.Stop();

            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
            RuleInput.Enabled = true;
        }

        private void DrawNextGeneration()
        {
            graphics.Clear(Color.Black);

            var field = engine.GetCurrentGeneration();

            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (field[x, y])
                    {
                        using (SolidBrush brush = new SolidBrush(selectedColor))
                        {
                            graphics.FillRectangle(brush, x * resolution, y * resolution, resolution, resolution);
                        }
                    }
                }
            }

            pictureBox1.Refresh();

            Text = $"Generation {engine.CurrentGeneration}";

            engine.NextGeneration();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawNextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            if (ValidateRule(RuleInput.Text))
            {
                string rule = RuleInput.Text;
                StartGame(rule);
            }
            else
            {
                MessageBox.Show("Invalid rule format. Please enter in B****/S**** format.");
            }
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                engine.AddCell(x, y);
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                engine.RemoveCell(x, y);
            }
        }

        private void nudResolution_ValueChanged(object sender, EventArgs e)
        {

        }

        private bool ValidateRule(string rule)
        {
            string pattern = @"^B\d{1,10}/S\d{0,10}$";
            return Regex.IsMatch(rule, pattern);
        }

        private void bColorClicker_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedColor = colorDialog1.Color;
            }
        }

        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = maxInterval/trackBar1.Value;
        }
    }
}
