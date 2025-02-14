﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Game.src.Project
{
    public class CLI
    {
        public static void Run()
        {
            string input;

            //AppDomain.CurrentDomain.ProcessExit += OnProcessExit; // Autosave ao sair da aplicação

            do
            {
                Console.Write("Comando: ");
                input = Console.ReadLine();
                Console.WriteLine();
                CheckCommand(input);
            }
            while (!string.IsNullOrWhiteSpace(input));

            void CheckCommand(string command)
            {
                string[] words = command.Split(' ');

                switch (words[0])
                {
                    case "RJ": // Registar Jogador
                        if (_HasRequiredInputs(words.Length, 2))
                            PlayerList.RegisterPlayer(words[1]);
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "LJ": // Listar Jogadores
                        if (_HasRequiredInputs(words.Length, 1))
                            PlayerList.ListPlayers();
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "IJ": // Iniciar Jogo
                        if (_HasRequiredInputs(words.Length, 4))
                            Game.StartGame(words[1], words[2], words[3]);
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "MP": // Mover Peça
                        if (_HasRequiredInputs(words.Length, 4))
                            Game.Command_MovePiece(words[1], words[2], words[3]);
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "OS": // Mover Peça
                        if (_HasRequiredInputs(words.Length, 3))
                            Game.Command_SpecialOperation(words[1], words[2]);
                        else if (_HasRequiredInputs(words.Length, 4))
                            Game.Command_SpecialOperation(words[1], words[2], words[3]);
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "DJ": // Detalhes de jogo
                        if (_HasRequiredInputs(words.Length, 1))
                        {
                            if (Game._IsGameInProgress)
                                Board.PrintBoard(Game.board);
                            else { Console.WriteLine("Não existe jogo em curso.\n"); }
                        }
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "D": // Desistir de jogo
                        if (words.Length > 2)
                            Game.ForfeitGame(words[1], words[2]);
                        else Game.ForfeitGame(words[1]);
                        break;

                    case "G": // Gravar 
                        if (_HasRequiredInputs(words.Length, 2))
                            Game.SaveFile(words[1]);
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "L": // Ler Jogo
                        if (_HasRequiredInputs(words.Length, 2))
                            Game.LoadFile(words[1]);
                        else Console.WriteLine("Instrução inválida.\n");
                        break;

                    case "XUndo": // Voltar uma jogada atrás
                        Game.Undo_Round();
                        break;

                    case "XClear": // Limpar Consola
                        Console.Clear();
                        break;

                    default:
                        if (string.IsNullOrWhiteSpace(command)) break;
                        else
                            Console.WriteLine("Instrução inválida.\n");
                        break;
                }
            }

            bool _HasRequiredInputs(int nr_inputs, int nr_reqInputs)
            {
                return nr_inputs == nr_reqInputs;
            }

            // Gravar no ficheiro dado, quando o utilizador sai da aplicação sem gravar
            static void OnProcessExit(object sender, EventArgs e)
            {
                // Codigo que corre quando aplicação está prestes a fechar.
                Console.WriteLine("A aplicação está a gravar os dados ...");
                Game.SaveFileExiting();
            }

            /*void movePiece(string[,] board, string from, string to)
            {
                // From = "C7",  To = "C5"
                char c = from[0];
                int column = c - 'A';
                c = from[1];
                int line = 7 - (c - '1');

                //Guarda a peça que estava na posição, na variavel piece
                string piece = board[line, column];
                board[line, column] = "";

                c = to[0];
                column = c - 'A';
                c = to[1];
                line = 7 - (c - '1');
                board[line, column] = piece;
            }*/

            /*void findPiece(string[,] board, string piece, char[] letters)
            {
                for (int line = 0; line < board.GetLength(0); line++)
                {
                    for (int col = 0; col < board.GetLength(1); col++)
                    {
                        if (board[line, col] == piece)
                        {
                            Console.WriteLine($"\n Location: {letters[col]} {8 - line}");
                        }
                    }
                }
            }*/

            /*void translateToMatrixCoord(string[,] board, string position){
                string[,] newBoardCoord = new string[2];
                char c = position[0];
                int column = c - 'A';
                c = position[1];
                int line = 7 - (c - '1');

                return board;
            }*/
        }
    }
}