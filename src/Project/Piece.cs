using System.Net;
using System.Text.Json.Serialization;

public class Piece
{
    public PieceType? Type  {get; set;}

    public Location? Location { get; set; } 
    public PieceTeam? Team { get; set; }
    public string PlaceHolder {get; set;}
    public bool isAlive { get; set; } = true;
    public bool SpecialOperation_ON { get; set; } = true;
    public int Nr_Movements { get; set; } = 0;
    public Piece() { }

    [JsonConstructor]
    public Piece(PieceType type, Location location, PieceTeam team, string placeholder){
        Type = type;
        Location = location;
        Team = team;
        PlaceHolder = placeholder;
    } 
    
    public virtual void MovePiece(Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, PieceTeam team, Piece[,] board)
    {

    }
}
public class Location { 
    public int Row { get; set; }
    public int Col { get; set; }

    public Location(int row, int col) {
        Row = row;
        Col = col;
    }

    public override string ToString()
    {
        return $"Row: {Row} / Col: {Col}";
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