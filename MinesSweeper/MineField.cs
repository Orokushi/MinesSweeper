using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesSweeper
{
    /// <summary>
    /// <c>MineField</c> class represents the mine field.
    /// </summary>
    internal class MineField
    {
        // Minefield dimensions.
        private int rows;
        private int columns;
        // Number of mines in the game.
        private int mines;
        // true = mine, false = empty field
        private bool[,] mineMap;
        // Number of fields that have already been checked.
        private int fieldsChecked;

        /// <summary>
        /// Constructor for the MineField class. 
        /// </summary>
        /// <param name="rows">Number of rows in the minefield.</param>
        /// <param name="columns">Number of columns in the minefield.</param>
        /// <param name="mines">Number of mines in the minefield.</param>
        internal MineField(int rows, int columns, int mines)
        {
            this.rows = rows;
            this.columns = columns;
            this.mines = mines;
            fieldsChecked = 0;
            mineMap = new bool[rows, columns];
        }

        /// <summary>
        /// Resets the data for a new game.
        /// </summary>
        internal void NewGame()
        {
            ResetScore();
            DeleteMineField();
            PlaceMines();
        }

        /// <summary>
        /// Returns the score as a string in the format "[fields checked]/[total fields]".
        /// </summary>
        /// <returns>String of the number of fields checked and total fields.</returns>
        internal string Score()
        {
            return fieldsChecked + "/" + (MapSize() - mines);
        }

        /// <summary>
        /// Cheks the coordinates for a mine.
        /// </summary>
        /// <param name="coordinates">Tuple of coordinates of the field to be checked.</param>
        /// <param name="count">Indicates, if the check should be counted for score purposes.</param>
        /// <returns>Bool indicating if the field contains a mine.</returns>
        internal bool CheckField(Tuple<int, int> coordinates, bool count = true)
        {
            if (count) fieldsChecked++;

            int x = coordinates.Item1;
            int y = coordinates.Item2;
            return mineMap[x, y];
        }

        /// <summary>
        /// Calculates, how many empty fields (fields not containing a mine) are left unchecked.
        /// </summary>
        /// <returns>Number of empty fields left.</returns>
        internal int EmptyFieldsLeft()
        {
            return MapSize() - mines - fieldsChecked;
        }


        /// <summary>
        /// Takes in a tuple of coordinates and counts the number of mines adjacent to that position.
        /// </summary>
        /// <param name="coordinates">Tuple of coordinates of the field to check for adjacent mines.</param>
        /// <returns>Number of adjacent mines.</returns>
        internal int CountAdjacentMines(Tuple<int, int> coordinates)
        {
            int x = coordinates.Item1;
            int y = coordinates.Item2;

            int distance = 1;
            int found = 0;

            for (int i = x - distance; i <= x + distance; i++)
            {
                for (int j = y - distance; j <= y + distance; j++)
                {
                    if (i >= 0
                        && i < mineMap.GetLength(0)
                        && j >= 0
                        && j < mineMap.GetLength(1)
                        && (i != x || j != y) // Dont count yourself.
                        && mineMap[i, j])
                    {
                        found++;
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// Get the total number of fields in the minefield.
        /// </summary>
        /// <returns>Total number of fields.</returns>
        private int MapSize()
        {
            return rows * columns;
        }

        /// <summary>
        /// Resets the scre to zero.
        /// </summary>
        private void ResetScore()
        {
            fieldsChecked = 0;
        }

        /// <summary>
        /// Sets all elements in the mineMap array to false (no mines).
        /// </summary>
        private void DeleteMineField()
        {
            for (int i = 0; i < mineMap.GetLength(0); i++)
            {
                for (int j = 0; j < mineMap.GetLength(1); j++)
                {
                    mineMap[i, j] = false;
                }
            }
        }

        /// <summary>
        /// Places mines randomly on the minefield by selecting random indexes in the mineMap array and setting the corresponding element to true.
        /// </summary>
        private void PlaceMines()
        {
            List<int> possibleNumbers = Enumerable.Range(0, MapSize()).ToList();
            Random random = new();

            for (int i = 0; i < mines; i++)
            {
                if (possibleNumbers.Count == 0)
                {
                    break;
                }
                int randomIndex = random.Next(possibleNumbers.Count);
                int selectedNumber = possibleNumbers[randomIndex];
                possibleNumbers.RemoveAt(randomIndex);

                // The row.
                int x = selectedNumber / columns;
                // The column.
                int y = selectedNumber % columns;

                mineMap[x, y] = true;
            }
        }
    }
}
