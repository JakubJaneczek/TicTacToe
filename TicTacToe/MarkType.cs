using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    // The type of value a cell in the game is currently at
    public enum MarkType
    {
        //Not clicked yet
        Free,
        // O
        Nought,
        // X
        Cross
    }
}
