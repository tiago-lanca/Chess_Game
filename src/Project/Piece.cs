using System.Net;

class Piece
{
    public PieceType? Type  {get; set;}
    public Location? Location { get; set; } = new Location();
    public PieceTeam? Team { get; set; }

    public string PlaceHolder {get; set;}

    public Piece() { }
   
    public Piece(PieceType type, int x, int y, PieceTeam team, string placeholder){
        Type = type;
        Location.X = x;
        Location.Y = y;
        Team = team;
        PlaceHolder = placeholder;
    }    
}
public class Location { 
    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString()
    {
        return $"Row: {X} / Col: {Y}";
    }
}

public enum PieceTeam {
        White,
        Black
    }

public enum PieceType {
    Pawn,
    Bishop,
    Knight,
    Rook,
    Queen,
    King
}