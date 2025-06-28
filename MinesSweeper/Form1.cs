/*
 * Author: Zdenek Simunek
 */

using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MinesSweeper
{
    /// <summary>
    /// GameWindow implements UI for the MinesSweeper game.
    /// </summary>
    public partial class GameWindow : Form
    {
        // Button parameters.
        private const int buttonSide = 24;
        private const int fieldSpaceLeft = 12;
        private const int fieldSpaceTop = 50;

        private const string clockFormat = @"mm\:ss";

        private System.Windows.Forms.Timer gameClock = new();
        // Seconds elapsed since the begining of the game.
        private int elapsed = 0;

        private GameControl gameControl;
        private Button[,]? fieldButtons;

        /// <summary>
        /// Constructor for the MainWindow form. 
        /// </summary>
        public GameWindow()
        {
            InitializeComponent();
            gameClock.Tick += new EventHandler(UpdateTimer);
            // Set the game clock time to tik every second.
            gameClock.Interval = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;
            gameControl = new GameControl(this);
        }

        /// <summary>
        /// Clicks all adjacent buttons to the provided coordinates.
        /// </summary>
        /// <param name="coordinates">Tuple with x and y coordinates of the button.</param>
        internal void ClickAdjacentButtons(Tuple<int, int> coordinates)
        {
            if (fieldButtons is null) throw new NullReferenceException();

            int x = coordinates.Item1;
            int y = coordinates.Item2;
            int distance = 1;

            for (int i = x - distance; i <= x + distance; i++)
            {
                for (int j = y - distance; j <= y + distance; j++)
                {
                    if (i >= 0
                        && i < fieldButtons.GetLength(0)
                        && j >= 0
                        && j < fieldButtons.GetLength(1)
                        && (i != x || j != y))
                    {
                        fieldButtons[i, j].PerformClick();
                    }
                }
            }
        }

        /// <summary>
        /// Creates the minefield by creating new buttons for each element in the buttons array and sets their properties.
        /// </summary>
        /// <param name="rows">Number of rows in the field.</param>
        /// <param name="columns">Number of columns in the field.</param>
        internal void CreateMineField(int rows, int columns)
        {
            if (fieldButtons is not null)
            {
                RemoveMineFieldUI();
            }
            fieldButtons = new Button[rows, columns];

            for (int i = 0; i < fieldButtons.GetLength(0); i++)
            {
                for (int j = 0; j < fieldButtons.GetLength(1); j++)
                {
                    Button button = new()
                    {
                        Size = new Size(width: buttonSide, height: buttonSide),
                        Location = new Point(x: fieldSpaceLeft + (j * buttonSide), y: fieldSpaceTop + (i * buttonSide)),
                        Tag = new Tuple<int, int>(i, j),
                        TabStop = false
                    };
                    fieldButtons[i, j] = button;
                    Controls.Add(button);
                    button.Click += FieldButton_Click;
                    button.MouseDown += FieldButton_MouseDown;
                }
            }
        }

        /// <summary>
        /// Updates the score label.
        /// </summary>
        /// <param name="score">String to show as a score.</param>
        internal void updateScore(string score)
        {
            scoreLabel.Text = score;
        }

        /// <summary>
        /// Renders an empty field and sets the text to the number of adjacent mines
        /// </summary>
        /// <param name="coordinates">The coordinates of the field to be rendered.</param>
        /// <param name="adjacentMinesCnt">Number of adjacent mines.</param>
        internal void RenderEmptyField(Tuple<int, int> coordinates, int adjacentMinesCnt)
        {
            if (fieldButtons is null) throw new NullReferenceException();

            Button button = fieldButtons[coordinates.Item1, coordinates.Item2];
            button.Enabled = false;
            if (adjacentMinesCnt > 0)
            {
                button.BackColor = default;
                button.ForeColor = Color.Black;
                button.Text = adjacentMinesCnt.ToString();
            }
            else
            {
                button.Text = "";
            }
        }

        /// <summary>
        /// Renders a mine.
        /// </summary>
        /// <param name="coordinates">The coordinates of the mine to be rendered.</param>
        internal void RenderMine(Tuple<int, int> coordinates)
        {
            if (fieldButtons is null) throw new NullReferenceException();

            Button button = fieldButtons[coordinates.Item1, coordinates.Item2];
            button.Enabled = false;
            button.BackColor = Color.Red;
            button.Text = "";
        }

        /// <summary>
        /// Signal the end of the game to the UI. Stops the game clock and shows a message box with either "You won!" or "You lost!".
        /// </summary>
        /// <param name="win">Bool to determine if the player won or lost.</param>
        internal void GameOver(bool win)
        {
            gameClock.Stop();
            if (win)
            {
                MessageBox.Show("You won!", "Congratulations!");
            }
            else
            {
                MessageBox.Show("You lost!", "Game over!");
            }
        }

        /// <summary>
        /// Event handler that updates the game clock.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpdateTimer(object? sender, EventArgs e)
        {
            elapsed++;
            TimeSpan time = TimeSpan.FromSeconds(elapsed);
            string timeString = time.ToString(clockFormat);
            timeLabel.Text = timeString;
        }

        /// <summary>
        /// Event handler that starts a new game when the restart button is clicked (the smily face).
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RestartButton_Click(object sender, EventArgs e)
        {
            gameControl.RestartGame();
            ResetGameClock();
        }

        /// <summary>
        /// Resets game clock.
        /// </summary>
        private void ResetGameClock()
        {
            // I need to manually update the 0 time, because the tick event activates 1 second AFTER the begining of the game.
            timeLabel.Text = "00:00";
            elapsed = 0;
            gameClock.Start();
        }

        /// <summary>
        /// Removes minefield buttons from the form.
        /// </summary>
        private void RemoveMineFieldUI()
        {
            if (fieldButtons is null) throw new NullReferenceException();

            for (int i = 0; i < fieldButtons.GetLength(0); i++)
            {
                for (int j = 0; j < fieldButtons.GetLength(1); j++)
                {
                    Controls.Remove(fieldButtons[i, j]);
                }
            }
        }

        /// <summary>
        /// Event handler that handles the click event for the minefield buttons.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void FieldButton_Click(object? sender, EventArgs e)
        {
            if (sender is null) throw new ArgumentException();
            Button button = (Button)sender;
            Tuple<int, int> coordinates = (Tuple<int, int>)button.Tag;
            gameControl.ExploreField(coordinates);

        }


        /// <summary>
        /// Event handler that handles the right click for the minefield buttons.
        /// It changes the text of the button to "!" when right clicked, and back to "" when right clicked again.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void FieldButton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is null) throw new ArgumentException();
            Button button = (Button)sender;
            if (e.Button == MouseButtons.Right)
            {
                button.ForeColor = Color.Red;
                if (button.Text == "")
                {
                    button.Text = "!";
                }
                else
                {
                    button.Text = "";
                }
            }
        }
    }
}
