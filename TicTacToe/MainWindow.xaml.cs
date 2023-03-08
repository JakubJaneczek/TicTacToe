using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int size = 3;
        //Player1 (X), Player2 (O)
        private bool mPlayer1Turn;
        //Number of elements in row to win
        private int mInRow = 3;
        private bool mvsAI;

        public bool mSuccess;
        public bool mPlayer1Start;
        public MainWindow()
        {
            mvsAI = true;
            mPlayer1Start = true;
            mSuccess = false;
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            mSuccess = true;
            if (Player1Button.IsChecked == true)
            {
                mPlayer1Start = true;
            }
            else if (Player2Button.IsChecked == true)
            {
                mPlayer1Start = false;
            }
            if(vsAI.IsChecked == true)
            {
                VsAIWindow vsAIWindow = new VsAIWindow(int.Parse(BoardSize.Text), mPlayer1Start, int.Parse(InRow.Text), mSuccess);
            }
            else if(PvP.IsChecked== true)
            {
                PvP pvp = new PvP(int.Parse(BoardSize.Text), mPlayer1Start, int.Parse(InRow.Text), mSuccess);
            }

            Close();
        }

        private void ButtonQUIT_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}