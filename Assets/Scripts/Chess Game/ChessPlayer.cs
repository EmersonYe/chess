using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPlayer : MonoBehaviour
{
    public TeamColor team { get; }
    public Board board { get; }
    public List<Piece> activePieces { get; private set; }

    public ChessPlayer(TeamColor team, Board board)
    {
        this.team = team;
        this.board = board;
        activePieces = new List<Piece>();
    }

    public void addPiece(Piece piece)
    {
        if (!activePieces.Contains(piece))
            activePieces.Add(piece);
    }

    public void removePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
            activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach (var piece in activePieces)
        {
            if (board.HasPiece(piece))
                piece.SelectAvailableSquares();
        }
    }
}
