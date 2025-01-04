using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public class Pawn : Piece
{
    public bool FirstMove { get; set; } = true;
    public bool En_Passant_Enable { get; set; } = false;
    public int En_Passant_Round { get; set; }
    public Pawn() => Type = PieceType.Pawn;
    public Pawn(PieceType type, Location location, PieceTeam team, string placeholder)
        :base(type, location, team, placeholder)
    {

    }

    public override Piece Clone()
    {
        return new Pawn
        {
            FirstMove = FirstMove,
            En_Passant_Enable = En_Passant_Enable,
            En_Passant_Round = En_Passant_Round,
            Type = Type,
            Location = new Location(Location.Row, Location.Col),
            Team = Team,
            PlaceHolder = PlaceHolder,
        };
    }

    public override void SpecialOperation(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);
        Pawn pawn = (Pawn)piece;

        if (SpecialOperation_Enable)
        {
            if (piece.Team == PieceTeam.White)
            {
                if (piece.Location.Row + 1 < board.GetLength(0))
                {
                    if (board[piece.Location.Row + 1, piece.Location.Col] == null)
                        possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{piece.Location.Row + 2}");
                }
            }
            else
            {
                if (piece.Location.Row - 1 >= 0)
                {
                    if (board[piece.Location.Row - 1, piece.Location.Col] == null)
                        possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{piece.Location.Row}");
                }
            }

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                Testing_PiecePosition(piece, fromLocation, toLocation, board);

                if (friendKing.IsKing_InCheck(board))
                {
                    //Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Console.WriteLine("Movimento inválido.\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                    
                    Console.WriteLine($"Peão {pawn.PlaceHolder} recuou com sucesso.\n");
                    SpecialOperation_Enable = false;
                    if (pawn.FirstMove) pawn.FirstMove = false;

                    // Verifica se o Rei adversário fica Check
                    GetAllMoves(piece, possibleMoves, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {

                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                            FinishGame_Complete();
                        else
                            //Console.WriteLine($"{enemyKing.PlaceHolder} em CHECK.\n");
                            Console.WriteLine("Check.\n");
                    }                    
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }
        else
            Console.WriteLine("Movimento inválido.\n");
    }
   
    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();
        List<string> en_PassantMoves = new List<string>();

        Pawn pawn = (Pawn)piece;
        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);

        if (friendKing.IsKing_InCheck(board))
        {
            GetAllMoves(piece, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                // Movimenta a peça de posição para proceder à verificação se o Rei da equipa fica em check
                Testing_PiecePosition(piece, fromLocation, toLocation, board);

                // Recebe todas as possiveis movimentações do inimigo e verifica se o rei da equipa fica check

                if (friendKing.IsKing_InCheck(board))
                {
                    //Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Console.WriteLine("Movimento inválido.\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    // Faz o movimento da peça e coloca o rei da equipa Não Check
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    
                    if (pawn.FirstMove) pawn.FirstMove = false;
                    friendKing.isCheck = false;

                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }
        // Rei da equipa não está Check
        else
        {
            // RECEBER POSSIVEIS MOVIMENTAÇÕES
            GetAllMoves(pawn, possibleMoves, board);
            // Verifica movimentações de En Passant possiveis
            EnPassant_PossibleMovement(pawn, possibleMoves, en_PassantMoves, input_FromPos, board);

            // EFETUAR A MOVIMENTAÇÃO
            // Verifica se a movimentação pretendida é válida
            if (IsValidMove(possibleMoves, input_ToPos))
            {
                // Se o peão andar 2 casas, ativa a movimentação En Passant nele por uma jogada
                Verify_EnPassant_State(pawn, toLocation);

                Testing_PiecePosition(piece, fromLocation, toLocation, board);                

                if (friendKing.IsKing_InCheck(board))
                {
                    // Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Console.WriteLine("Movimento inválido.\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    // Aqui acrescenta +1 Game.Nr_Moves
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);

                    // Verifica se o Peão foi promovido, se for altera a peça e atualiza o tabuleiro
                    if (PawnInYLimits(pawn, board))
                        PromotePawn(pawn, board);

                    if (pawn.FirstMove) pawn.FirstMove = false;

                    // Verifica se o Rei adversário fica Check, se sim coloca o rei check = true
                    GetAllMoves(piece, possibleMoves, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {
                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                        {
                            FinishGame_Complete();                            
                        }
                        else
                            //Console.WriteLine($"{enemyKing.PlaceHolder} em CHECK.\n");
                            Console.WriteLine("Check.\n");
                    }
                    Game.SaveRound(board, Game.savedRounds);
                }
            }
            else Print_PossibleMovements(possibleMoves);            
        }
    }

    public List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return GetPawnDiagonalMoves((Pawn)piece, possibleMoves, board)
            .Concat(GetPawnVerticalMoves((Pawn)piece, possibleMoves, board))
            .ToList();
    }

    public override List<string> GetMoves_ForCheckKing(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return GetPawn_EmptyDiagonalMoves((Pawn)piece, possibleMoves, board)
            .Concat(GetPawnVerticalMoves((Pawn)piece, possibleMoves, board))
            .ToList();
    }
    public override void PieceMoved_Info(Piece piece, List<string> possibleMoves, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> en_PassantMoves = new List<string>();
        EnPassant_PossibleMovement((Pawn)piece, possibleMoves, en_PassantMoves, input_FromPos, board);
        
        if (en_PassantMoves.Any(move => move.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase)))
        {
            // Peão que sofre en passant fica nulo
            //board[toLocation.Row + 1, toLocation.Col].isAlive = false;
            if (piece.Team == PieceTeam.White)
            {
                Console.WriteLine($"Peça {board[toLocation.Row + 1, toLocation.Col].PlaceHolder} capturada.\n");
                
                board[toLocation.Row + 1, toLocation.Col] = null;
            }
            else
                Console.WriteLine($"Peça {board[toLocation.Row - 1, toLocation.Col].PlaceHolder} capturada.\n");
                board[toLocation.Row - 1, toLocation.Col] = null;

            Console.WriteLine("En passant efetuado.\n");
            Set_EnPassant_Disabled(board);
        }

        
        else {
            Piece toPositionPiece = board[toLocation.Row, toLocation.Col];
            if (toPositionPiece != null)
            {
                Console.WriteLine($"Peça {toPositionPiece.PlaceHolder} capturada.\n");
                //toPositionPiece.isAlive = false;
            }
            else
                Console.WriteLine($"{piece.PlaceHolder} movida com sucesso.\n");
        }
    }

    public List<string> GetPawnDiagonalMoves(Pawn piece, List<string> possibleMoves, Piece[,] board)
    {
        int column, row = piece.Location.Row + 1;
        Piece nextLeftColumnPiece, nextRightColumnPiece;
        
        //White Team
        if (piece.Team == PieceTeam.White)
        {
            // Verifica se a coluna esq. está dentro dos limites
            if (piece.Location.Col - 1 >= 0)
            {
                nextLeftColumnPiece = board[piece.Location.Row - 1, piece.Location.Col - 1];

                // Verifica se a coluna dir. está dentro dos limites
                if (piece.Location.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[piece.Location.Row - 1, piece.Location.Col + 1];
                    // Coluna à esquerda
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }

                    if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                    {
                        // Coluna à direita
                        column = piece.Location.Col + 'A' + 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }
                }
                else // Coluna à direita está fora dos limites. So calcula a esq.
                {
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }
                }
            }
            // Coluna em cima à esquerda está fora do tabuleiro. Só dá para cima e direita
            else
            {
                if (piece.Location.Row - 1 >= 0 && piece.Location.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[piece.Location.Row - 1, piece.Location.Col + 1];
                    if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' + 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }
                }
            }
        }
        //Black Team
        else
        {
            // Verifica se a coluna esq. está dentro dos limites
            if (piece.Location.Col - 1 >= 0)
            {
                nextLeftColumnPiece = board[piece.Location.Row + 1, piece.Location.Col - 1];

                // Verifica se a coluna dir. está dentro dos limites
                if (piece.Location.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[piece. Location.Row + 1, piece.Location.Col + 1];
                    // Coluna à esquerda
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                    }

                    // Coluna à direita
                    if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' + 1;
                        possibleMoves.Add($"{(char)column}{row + 1}");
                    }
                }

                else // Coluna à direita está fora dos limites. So calcula a esq.
                {
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{(char)column}{row + 1}");
                    }
                }                
            }

            // Coluna em baixo à esquerda está fora do tabuleiro. Só dá para baixo e direita
            else
            {
                // Verifica se está dentro dos limites do tabuleiro
                if (piece.Location.Row + 1 < board.GetLength(0) && piece.Location.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[piece.Location.Row + 1, piece.Location.Col + 1];

                    if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' + 1;
                        possibleMoves.Add($"{(char)column}{row + 1}");
                    }
                }
            }
        }
        return possibleMoves;
    }

    public List<string> GetPawnVerticalMoves(Pawn piece, List<string> possibleMoves, Piece[,] board)
    {
        Piece nextPiece;
        int row = piece.Location.Row + 1;

        // WHITE TEAM
        if (piece.Team == PieceTeam.White)
        {
            if (piece.Location.Row - 1 >= 0)
            {
                nextPiece = board[piece.Location.Row - 1, piece.Location.Col];
                //se a posiçao em cima for null, adiciona posiçao à possibleMove
                if (nextPiece == null)
                {
                    possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row - 1}");
                    if (piece.FirstMove && board[piece.Location.Row - 2, piece.Location.Col] == null)
                        possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row - 2}");
                }
            }
        }

        //BLACK TEAM
        else
        {
            if (piece.Location.Row + 1 < board.GetLength(0))
            {
                nextPiece = board[piece.Location.Row + 1, piece.Location.Col];
                if (nextPiece == null)
                {
                    possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                    if (piece.FirstMove && board[piece.Location.Row + 2, piece.Location.Col] == null)
                        possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 2}");
                }
            }
        }

        return possibleMoves;
    }

    public Queen PromotePawn(Pawn piece, Piece[,] board)
    {
        Queen newQueen;
        int indexQueen = 0;

        // WHITE TEAM
        if (piece.Team == PieceTeam.White && PawnInYLimits(piece, board))
        {
            Console.WriteLine($"Peão {piece.PlaceHolder} promovido.\n");
            newQueen = new Queen(PieceType.Queen, new Location(piece.Location.Row, piece.Location.Col), PieceTeam.White, $"WQ{Get_NewQueenIndex(piece, board)}");
            board[piece.Location.Row, piece.Location.Col] = newQueen;

            return newQueen;
        }

        // BLACK TEAM
        else if(piece.Team == PieceTeam.Black && PawnInYLimits(piece, board))
        {
            Console.WriteLine($"Peão {piece.PlaceHolder} promovido.\n");
            newQueen = new Queen(PieceType.Queen, new Location(piece.Location.Row, piece.Location.Col), PieceTeam.Black, $"BQ{Get_NewQueenIndex(piece,board)}");
            board[piece.Location.Row, piece.Location.Col] = newQueen;

            return newQueen;
        }

        else return null;
       
    }

    public int Get_NewQueenIndex(Piece piece, Piece[,] board)
    {
        int indexQueen = 0;

        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != null && board[row, col].Type == PieceType.Queen && board[row, col].Team == piece.Team)
                {
                    if (board[row, col].PlaceHolder[2] - '0' > indexQueen)
                        indexQueen = board[row, col].PlaceHolder[2] - '0';
                }
            }
        }
        return indexQueen + 1;
    }

    public void Verify_EnPassant_State(Pawn pawn, Location toLocation)
    {
        if (Math.Abs(pawn.Location.Row - toLocation.Row) == 2)
        {
            pawn.En_Passant_Enable = true;
            pawn.En_Passant_Round = Game.Nr_Rounds + 1;
        }
    }
    public bool PawnInYLimits(Pawn pawn, Piece[,] board)
    {
        return pawn.Location.Row == board.GetLength(0) - 1 || pawn.Location.Row == 0;
    }
    public List<string> EnPassant_PossibleMovement(Pawn pawn, List<string> possibleMoves, List<string> en_PassantMoves, string input_FromPos, Piece[,] board)
    {

        if (_IsEnPassant_Enabled(board))
        {
            // Verifica peão da direita
            // Verifica se a coluna da direita está dentro dos limites
            if (pawn.Location.Col + 1 < board.GetLength(1))
            {
                if (board[pawn.Location.Row, pawn.Location.Col + 1] != null)
                {
                    if (board[pawn.Location.Row, pawn.Location.Col + 1] is Pawn) { 
                        Pawn rightColumnPawn = (Pawn)board[pawn.Location.Row, pawn.Location.Col + 1];

                        if (rightColumnPawn.Team != pawn.Team)
                        {
                            if (rightColumnPawn.En_Passant_Round == Game.Nr_Rounds)
                            {
                                if (rightColumnPawn.En_Passant_Enable)
                                {
                                    if (pawn.Team == PieceTeam.White)
                                    {
                                        possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rightColumnPawn.Location.Row}");
                                        en_PassantMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rightColumnPawn.Location.Row}");
                                    }
                                    else
                                    {
                                        possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rightColumnPawn.Location.Row + 2}");
                                        en_PassantMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rightColumnPawn.Location.Row + 2}");
                                    }
                                }
                            }
                            else
                            {
                                rightColumnPawn.En_Passant_Enable = false;
                                rightColumnPawn.En_Passant_Round = 0;
                            }
                        }
                        
                    }
                }
            }

            // Verifica peão da esquerda
            // Verifica se a coluna da esquerda está dentro dos limites
            if (pawn.Location.Col - 1 >= 0)
            {
                if (board[pawn.Location.Row, pawn.Location.Col - 1] != null)
                {
                    if (board[pawn.Location.Row, pawn.Location.Col - 1] is Pawn )
                    {
                        Pawn leftColumnPawn = (Pawn)board[pawn.Location.Row, pawn.Location.Col - 1];

                        if (leftColumnPawn.Team != pawn.Team)
                        {
                            if (leftColumnPawn.En_Passant_Round == Game.Nr_Rounds)
                            {
                                if (leftColumnPawn.En_Passant_Enable)
                                {
                                    if (pawn.Team == PieceTeam.White)
                                    {
                                        possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{leftColumnPawn.Location.Row}");
                                        en_PassantMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{leftColumnPawn.Location.Row}");
                                    }
                                    else
                                    {
                                        possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{leftColumnPawn.Location.Row + 2}");
                                        en_PassantMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{leftColumnPawn.Location.Row + 2}");
                                    }
                                }
                            }
                            else
                            {
                                leftColumnPawn.En_Passant_Enable = false;
                                leftColumnPawn.En_Passant_Round = 0;
                            }
                        }
                    }
                }
            }
        }

        return en_PassantMoves;
    }

    public List<string> GetPawn_EmptyDiagonalMoves(Pawn piece, List<string> possibleMoves, Piece[,] board)
    {
        int column, row = piece.Location.Row + 1;
        Piece nextLeftColumnPiece, nextRightColumnPiece;

        //White Team
        if (piece.Team == PieceTeam.White)
        {
            // Verifica se a coluna esq. está dentro dos limites
            if (piece.Location.Col - 1 >= 0)
            {
                nextLeftColumnPiece = board[piece.Location.Row - 1, piece.Location.Col - 1];

                // Verifica se a coluna dir. está dentro dos limites
                if (piece.Location.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[piece.Location.Row - 1, piece.Location.Col + 1];
                    // Coluna à esquerda
                    if (nextLeftColumnPiece == null || nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }

                    if (nextRightColumnPiece == null || nextRightColumnPiece.Team != piece.Team)
                    {
                        // Coluna à direita
                        column = piece.Location.Col + 'A' + 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }
                }
                else // Coluna à direita está fora dos limites. So calcula a esq.
                {
                    if (nextLeftColumnPiece == null || nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{(char)column}{row - 1}");
                    }
                }
            }
            // Coluna em cima à esquerda está fora do tabuleiro. Só dá para cima e direita
            else
            {
                nextRightColumnPiece = board[piece.Location.Row - 1, piece.Location.Col + 1];
                if (nextRightColumnPiece == null || nextRightColumnPiece.Team != piece.Team)
                {
                    column = piece.Location.Col + 'A' + 1;
                    possibleMoves.Add($"{(char)column}{row - 1}");
                }
            }
        }
        //Black Team
        else
        {
            // Verifica se a coluna esq. está dentro dos limites
            if (piece.Location.Col - 1 >= 0)
            {
                nextLeftColumnPiece = board[piece.Location.Row + 1, piece.Location.Col - 1];

                // Verifica se a coluna dir. está dentro dos limites
                if (piece.Location.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[piece.Location.Row + 1, piece.Location.Col + 1];
                    // Coluna à esquerda
                    if (nextLeftColumnPiece == null || nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                    }

                    // Coluna à direita
                    if (nextRightColumnPiece == null || nextRightColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' + 1;
                        possibleMoves.Add($"{(char)column}{row + 1}");
                    }
                }

                else // Coluna à direita está fora dos limites. So calcula a esq.
                {
                    if (nextLeftColumnPiece == null || nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = piece.Location.Col + 'A' - 1;
                        possibleMoves.Add($"{(char)column}{row + 1}");
                    }
                }
            }

            // Coluna em baixo à esquerda está fora do tabuleiro. Só dá para baixo e direita
            else
            {
                nextRightColumnPiece = board[piece.Location.Row + 1, piece.Location.Col + 1];

                if (nextRightColumnPiece == null || nextRightColumnPiece.Team != piece.Team)
                {
                    column = piece.Location.Col + 'A' + 1;
                    possibleMoves.Add($"{(char)column}{row + 1}");
                }
            }
        }
        return possibleMoves;
    }

    public static bool _IsEnPassant_Enabled(Piece[,] board)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != null && board[row, col].Type == PieceType.Pawn)
                {
                    Pawn pawn = (Pawn)board[row, col];
                    if (pawn.En_Passant_Enable)
                        return true;
                }                
            }
        }
        return false;
    }

    public static void Set_EnPassant_Disabled(Piece[,] board)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != null && board[row, col].Type == PieceType.Pawn)
                {
                    Pawn pawn = (Pawn)board[row, col];
                    if (pawn.En_Passant_Enable)
                        pawn.En_Passant_Enable = false;
                }
            }
        }
    }
}

