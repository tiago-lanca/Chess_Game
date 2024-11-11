using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Rook : Piece
{
    private bool FirstMove { get; set; } = true;

    public Rook(PieceType pieceType, int x, int y, PieceTeam team, string placeholder, bool firstmove)
        :base(pieceType, x, y, team, placeholder)
    {
        FirstMove = firstmove;
    }
}

