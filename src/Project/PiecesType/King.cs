using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class King : Piece
{
    public King() { }
    public King(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    public override void MovePiece(Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, PieceTeam team, Piece[,] board)
    {

    }
}

