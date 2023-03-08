using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for VsAIWindow.xaml
    /// </summary>
    public partial class VsAIWindow : Window
    {
        public VsAIWindow(int size, bool player1Turn, int inRow, bool success)
        {
            mSize = size;
            mPlayer1Turn= player1Turn;
            mInRow= inRow;
            mSuccess= success;
            InitializeComponent();
            GameHandler();
        }
        public List<Button> Buttons = new List<Button>();
        public int mSize = 3;
        //Array storing current results of the board
        private MarkType[,] mResults;
        //Player1 (X), Player2 (O)
        private bool mPlayer1Turn;
        //Check if game ended
        private bool mGameEnded;
        //Check which player won
        private bool mPlayer1Won;
        //Number of elements in row to win
        private int mInRow;
        private bool mSuccess;

        public void GameHandler()
        {
            if (mSuccess == true)
            {
                this.Show();
                LoadVariables();
                LoadBoard();
                NewGame();
            }
            else
                Close();

        }

        private void LoadVariables()
        {
            mGameEnded = false;          
            mResults = new MarkType[mSize, mSize];
        }

        private void LoadBoard()
        {
            // Remove all existing column definitions
            GameBoard.ColumnDefinitions.Clear();
            GameBoard.RowDefinitions.Clear();
            GameBoard.Children.Clear();

            // Add new column definitions based on NumColumns
            for (int i = 0; i < mSize; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);
                GameBoard.ColumnDefinitions.Add(colDef);

                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                GameBoard.RowDefinitions.Add(rowDef);

                for (int j = 0; j < mSize; j++)
                {
                    Button button = new Button
                    {
                        FontSize = 600 / 2 / mSize,
                        Name = "Button" + i + j,
                    };
                    button.Click += ButtonClicked;
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);

                    GameBoard.Children.Add(button);
                }
            }

            // Update the board with the new column definitions
            // (e.g. add or remove buttons as necessary)
        }

        public void NewGame()
        {
            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < mSize; j++)
                {
                    mResults[i, j] = MarkType.Free;
                    //Iterate every Button on the grid
                }
            }
            GameBoard.Children.Cast<Button>().ToList().ForEach(button =>
            {
                //Change backgroudn, foreground and content to default
                button.Content = string.Empty;
                button.Background = Brushes.White;
                button.Foreground = Brushes.Blue;
            });

            mGameEnded = false;
            mPlayer1Won = false;
        }
        public void ButtonClicked(object sender, RoutedEventArgs e)
        {
            // Check if game hasn't ended
            if (mGameEnded)
            {
                MainWindow main = new MainWindow();
                main.Show();
                Close();
                return;
            }

            //Get button and it's position
            var button = (Button)sender;

            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            //Do nothing if space is taken
            if (mResults[column, row] != MarkType.Free)
            {
                return;
            }
            // Handle positioning
            else
            {
                //Set tile in array
                mResults[column, row] = mPlayer1Turn ? MarkType.Cross : MarkType.Nought;
                //Set tile in wpf
                button.Content = mPlayer1Turn ? "X" : "O";
                //Change to other player
                mPlayer1Turn = !mPlayer1Turn;
                //Set color of "X" 
                if (!mPlayer1Turn)
                {
                    button.Foreground = Brushes.Red;
                }
                CheckForWinner();
            }
            CheckIfEnd();
            AITurn();
        }

        public void AITurn()
        {
            if (moveCounter() == 0)
            {
                return;
            }
            // Handle positioning
            else
            {
                //Set tile in array
                Tuple<int, int> coords = GetBestMove(0, false, MarkType.Nought);
                mResults[coords.Item1, coords.Item2] = mPlayer1Turn ? MarkType.Cross : MarkType.Nought;
                GameBoard.Children.Cast<Button>().ToList().ForEach(button =>
                {
                    if (button.Name == "Button" + coords.Item1 + coords.Item2)
                    {
                        button.Content = mPlayer1Turn ? "X" : "O";
                        mPlayer1Turn = !mPlayer1Turn;
                        if (!mPlayer1Turn)
                        {
                            button.Foreground = Brushes.Red;
                        }
                    }
                });
                //Set tile in wpf

                //Set color of "X" 
                //Change to other player
                CheckIfEnd();
            }
        }
        private void CheckIfEnd()
        {
            if (moveCounter() == 0 || CheckForWinner() != 0)
            {
                if (CheckForWinner() == -10)
                {
                    foreach (var button in Buttons)
                    {
                        button.Background = Brushes.Green;
                    }
                }
                else if (CheckForWinner() == 10)
                {
                    foreach (var button in Buttons)
                    {
                        button.Background = Brushes.Green;
                    }
                }
                else if (CheckForWinner() == 0)
                {
                    GameBoard.Children.Cast<Button>().ToList().ForEach(button =>
                    {
                        button.Background = Brushes.Orange;
                    });
                }
                mGameEnded = true;
            }
        }
        private int moveCounter()
        {
            int moveCounter = 0;
            foreach (var tile in mResults)
            {
                if (tile == MarkType.Free)
                {
                    moveCounter++;
                }
            }
            return moveCounter;
        }
        private int CheckForWinner()
        {
            List<Button> winningButtons = new List<Button>();
            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < mSize; j++)
                {
                    //VERTICAL AND DIAGONALS
                    if (i < mSize - mInRow + 1)
                    {
                        if (mResults[i, j] != MarkType.Free)
                        {

                            //VERTICAL
                            for (int k = 1; k < mInRow; k++)
                            {
                                if (mResults[i + k, j] != mResults[i, j])
                                {
                                    break;
                                }
                                Button? myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i + k) as Button;
                                winningButtons.Add(myButton);
                                if (k == mInRow - 1)
                                {
                                    myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i) as Button;
                                    winningButtons.Add(myButton);
                                    Buttons = winningButtons;
                                    return Winner(i, j, winningButtons);

                                }
                            }
                            winningButtons.Clear();
                            //LEFT DIAGONAL

                            if (j >= mInRow - 1)
                            {
                                for (int k = 1; k < mInRow; k++)
                                {
                                    if (mResults[i + k, j - k] != mResults[i, j])
                                    {
                                        break;
                                    }
                                    Button? myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j - k && Grid.GetColumn(e) == i + k) as Button;
                                    winningButtons.Add(myButton);
                                    if (k == mInRow - 1)
                                    {
                                        myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i) as Button;
                                        winningButtons.Add(myButton);
                                        Buttons = winningButtons;
                                        return Winner(i, j, winningButtons);
                                    }
                                }
                            }
                            winningButtons.Clear();

                            //RIGHT DIAGONAL 

                            if (j < mSize - mInRow + 1)
                            {
                                for (int k = 1; k < mInRow; k++)
                                {
                                    if (mResults[i + k, j + k] != mResults[i, j])
                                    {
                                        break;
                                    }
                                    Button? myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j + k && Grid.GetColumn(e) == i + k) as Button;
                                    winningButtons.Add(myButton);
                                    if (k == mInRow - 1)
                                    {
                                        myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i) as Button;
                                        winningButtons.Add(myButton);
                                        Buttons = winningButtons;
                                        return Winner(i, j, winningButtons);
                                    }
                                }
                            }
                            winningButtons.Clear();
                        }
                    }
                    if (j < mSize - mInRow + 1)
                    {
                        if (mResults[i, j] != MarkType.Free)
                        {
                            for (int k = 1; k < mInRow; k++)
                            {
                                if (mResults[i, j + k] != mResults[i, j])
                                {
                                    break;
                                }
                                Button? myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j + k && Grid.GetColumn(e) == i) as Button;
                                winningButtons.Add(myButton);
                                if (k == mInRow - 1)
                                {
                                    myButton = GameBoard.Children.Cast<Button>().FirstOrDefault(e => Grid.GetRow(e) == j && Grid.GetColumn(e) == i) as Button;
                                    winningButtons.Add(myButton);
                                    Buttons = winningButtons;
                                    return Winner(i, j, winningButtons);
                                }
                            }
                        }
                    }
                    winningButtons.Clear();
                }
            }

            return 0;
        }
        public int Winner(int i, int j, List<Button> winningButtons)
        {
            if (mResults[i, j] == MarkType.Cross)
            {
                return 10;
            }
            else if (mResults[i, j] == MarkType.Nought)
            {
                return -10;
            }
            return 0;
        }
        public int MinMax(int depth, bool isMax, MarkType markType, int alpha, int beta)
        {
            int score = CheckForWinner();

            if (score == 10)
            {
                if (!isMax)
                {
                    return score - depth;
                }
                else return -score + depth;
            }

            if (score == -10)
            {
                if (isMax)
                {
                    return score + depth;
                }
                else return -score - depth;

            }

            if (CountMoves() == 0)
            {
                return 0;
            }

            if (isMax)
            {
                int maxScore = -9999;

                for (int i = 0; i < mSize; i++)
                {
                    for (int j = 0; j < mSize; j++)
                    {
                        if (mResults[i, j] == MarkType.Free)
                        {
                            mResults[i, j] = NextPlayer(markType);

                            int value = MinMax(depth + 1, false, NextPlayer(markType), alpha, beta);
                            maxScore = Math.Max(maxScore, value);
                            alpha = Math.Max(alpha, maxScore);

                            mResults[i, j] = MarkType.Free;

                            if (beta <= alpha)
                            {
                                break;
                            }
                        }
                    }
                }
                return maxScore;
            }
            else
            {
                int maxScore = 9999;
                for (int i = 0; i < mSize; i++)
                {
                    for (int j = 0; j < mSize; j++)
                    {
                        if (mResults[i, j] == MarkType.Free)
                        {
                            mResults[i, j] = NextPlayer(markType);

                            int value = MinMax(depth + 1, true, NextPlayer(markType), alpha, beta);
                            maxScore = Math.Min(maxScore, value);
                            beta = Math.Min(beta, maxScore);

                            mResults[i, j] = MarkType.Free;

                            if (beta <= alpha)
                            {
                                break;
                            }
                        }
                    }
                }
                return maxScore;

            }

        }
        private MarkType NextPlayer(MarkType markType)
        {
            if (markType == MarkType.Nought)
            {
                return MarkType.Cross;
            }
            else
                return MarkType.Nought;
        }
        public Tuple<int, int> GetBestMove(int depth, bool isMax, MarkType markType)
        {
            // Initialize the best score to the worst possible value for the maximizing player
            Tuple<int, int> bestMove = null;

            int bestScoreMax = -999;
            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < mSize; j++)
                {
                    if (mResults[i, j] == MarkType.Free)
                    {
                        mResults[i, j] = markType;

                        int score = MinMax(depth, isMax, markType, -9999, 9999);

                        mResults[i, j] = MarkType.Free;

                        if (score > bestScoreMax)
                        {
                            bestScoreMax = score;
                            bestMove = Tuple.Create(i, j);
                        }
                    }
                }
            }
            return bestMove;
        }
        private int CountMoves()
        {
            int movesPossible = 0;
            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < mSize; j++)
                {
                    if (mResults[i, j] == MarkType.Free)
                    {
                        movesPossible++;
                    }
                }
            }
            return movesPossible;
        }
    }
}
