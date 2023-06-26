using System;
using Bomber.Constants;
using Bomber.Interfaces;

namespace Bomber.Models
{
    // concrete implementation of the bomb game model.
	public class BombGameModel : IBombGameModel
	{
        public int Lives { get; set; } = AppConstants.MaxLives;
        public int Moves { get; set; } = 0;
        public int Size { get; set; } = 0;
        public string Location { get; set; } = string.Empty;
        public bool IsFinished => (CurrentState == AppConstants.GameState.FinishedSuccess || CurrentState == AppConstants.GameState.FinishedFailure);
        public AppConstants.GameState CurrentState { get; set; } = AppConstants.GameState.NotStarted;

        public void Reset(int size)
        {
            Lives = AppConstants.MaxLives;
            Moves = 0;
            Location = string.Empty;
            CurrentState = AppConstants.GameState.InProgress;
            Size = size;
        }

        public override string ToString() => $"Lives: {Lives}, Moves: {Moves}, Location: {Location}";

        // determine the valid moves that can happen next
        public HashSet<string> ValidMoves
        {
            get
            {
                var validMoves = new HashSet<string>();
                if (Moves == 0)
                {
                    // only first column
                    for(var r = 1; r <= Size; r++)
                    {
                        var key = $"A{r.ToString()}";
                        validMoves.Add(key);
                    }
                }
                else if ((false == string.IsNullOrEmpty(Location)) && (Location.Length >= 2))
                {
                    var column = Location[0];
                    var row = Convert.ToInt32(Location.Substring(1));
                    var columns = new List<char>
                    {
                        (char)(column-1), (char)column, (char)(column + 1)
                    };

                    // remove first column if 'A'
                    if (column == 'A')
                        columns.RemoveAt(0);

                    var rows = new List<int>
                    {
                        row - 1, row, row + 1
                    };

                    // remove first row if 0
                    if (row == 1)
                    {
                        rows.RemoveAt(0);
                    }
                    else if (row == Size)
                    {
                        // don't allow the user past the bottom of the board.
                        rows.RemoveAt(2);
                    }

                    foreach (var c in columns)
                    {
                        foreach (var r in rows)
                        {
                            var key = $"{c.ToString()}{r.ToString()}";
                            validMoves.Add(key);
                        }
                    }
                }
                return validMoves;
            }
        }
    }
}

