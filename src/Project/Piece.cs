class Piece
{
    public PieceType? Type  {get; set;} = PieceType.None;
    public Location? Location { get; set; } = new Location();
    public PieceTeam? Team { get; set; } = PieceTeam.None;

    public string PieceText {get; set;}

    public Piece() { }
    public Piece(PieceType type, int x, int y, PieceTeam team, string pieceText){
        Type = type;
        Location.X = x;
        Location.Y = y;
        Team = team;
        PieceText = pieceText;
    }    
}
public class Location { 
    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString()
    {
        return $"X: {X} / Y: {Y}";
    }
}

public enum PieceTeam {
        None,
        White,
        Black
    }

    public enum PieceType {
        None,
        Pawn,
        Bishop,
        Knight,
        Rook,
        Queen,
        King
    }