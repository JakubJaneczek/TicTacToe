using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    /// Interaction logic for PvP.xaml
    /// </summary>
    public partial class PvP : Window
    {

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
        public PvP(int size, bool player1Turn, int inRow, bool success)
        {
            mSize = size;
            mPlayer1Turn = player1Turn;
            mInRow = inRow;
            mSuccess = success;
            InitializeComponent();
            GameHandler();
        }

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
        }
        private void CheckForWinner()
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
                                    Winner(i, j, winningButtons);
                                    return;

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
                                        Winner(i, j, winningButtons);
                                        return;
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
                                        Winner(i, j, winningButtons);
                                        return;
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
                                    Winner(i, j, winningButtons);
                                    return;
                                }
                            }
                        }
                    }
                    winningButtons.Clear();
                }
            }
            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < mSize; j++)
                {
                    if (mResults[i, j] == MarkType.Free)
                    {
                        return;
                    }
                }

            }
            GameBoard.Children.Cast<Button>().ToList().ForEach(button =>
            {
                button.Background = Brushes.Orange;
            });
            mGameEnded = true;
        }
        public void Winner(int i, int j, List<Button> winningButtons)
        {
            if (mResults[i, j] == MarkType.Cross)
            {
                mPlayer1Won = true;
                mGameEnded = true;

            }
            else if (mResults[i, j] == MarkType.Nought)
            {
                mPlayer1Won = false;
                mGameEnded = true;

            }
            foreach (var button in winningButtons)
            {
                button.Background = Brushes.Green;
            }

        }
    }
}

