using System;
using System.Collections.Generic;
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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public bool mSuccess;
        public bool mPlayer1Start;
        public Menu()
        {
            mPlayer1Start = true;
            mSuccess = false;
            InitializeComponent();

        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (Player1Button.IsChecked == true)
            {
                mPlayer1Start = true;
            }
            else if (Player2Button.IsChecked == true)
            {
                mPlayer1Start= false;
            }
            mSuccess = true;
            Close();
        }

        private void ButtonQUIT_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
