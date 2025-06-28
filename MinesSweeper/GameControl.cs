using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesSweeper
{
    /// <summary>
    /// GameControl implements game logic of the MinesSweeper game.
    /// </summary>
    internal class GameControl
    {
        private const int rows = 10;
        private const int columns = 10;
        private const int mines = 15;
        private bool gameOver = true;
        private MineField mineField = new(rows, columns, mines);
        private GameWindow gameWindow;

        /// <summary>
        /// Constructor for the GameControl unit. 
        /// </summary>
        /// <param name="gameWindow">The game UI.</param>
        internal GameControl(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        internal void RestartGame()
        {
            gameOver = false;
            gameWindow.CreateMineField(rows, columns);
            mineField.NewGame();
            gameWindow.updateScore(mineField.Score());
        }

        /// <summary>
        /// Explore a field. Provides a reaction on mine or empty field.
        /// </summary>
        /// <param name="coordinates">Tuple with x and y coordinates of the field that is to be explored.</param>
        internal void ExploreField(Tuple<int, int> coordinates)
        {
            if (mineField.CheckField(coordinates))
            {
                gameOver = true;
                RevealAllFields();
                gameWindow.GameOver(win: false);
                return;
            }

            int adjacentMinesCnt = mineField.CountAdjacentMines(coordinates);
            gameWindow.RenderEmptyField(coordinates, adjacentMinesCnt);

            // If there are no adjacent mines, automatically reweal the adjacent fields for the user.
            if (adjacentMinesCnt == 0)
            {
                gameWindow.ClickAdjacentButtons(coordinates);
                if (gameOver) return; // The game can end after adjacent button is clicked.
            }

            gameWindow.updateScore(mineField.Score());

            if (mineField.EmptyFieldsLeft() == 0)
            {
                gameOver = true;
                RevealAllFields();
                gameWindow.GameOver(win: true);
            }
        }

        /// <summary>
        /// Reveals all fields in the minefield.
        /// </summary>
        private void RevealAllFields()
        {
            if (mineField is null) throw new NullReferenceException();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Tuple<int, int> coordinates = new Tuple<int, int>(i, j);

                    if (mineField.CheckField(coordinates, count: false))
                    {
                        gameWindow.RenderMine(coordinates);
                    }
                    else
                    {
                        int adjacentMinesCnt = mineField.CountAdjacentMines(coordinates);
                        gameWindow.RenderEmptyField(coordinates, adjacentMinesCnt);
                    }
                }
            }
        }
    }
}
