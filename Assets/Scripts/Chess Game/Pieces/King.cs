using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    private Vector2Int[] directions = new Vector2Int[] {
        new Vector2Int(-1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(1,-1),
        new Vector2Int(1,1),
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up
    };

    private Vector2Int leftCastleSquare;
    private Vector2Int rightCastleSquare;
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        foreach (Vector2Int direction in directions)
        {
            if (IsDirectionAvailableMove(direction))
                TryToAddMove(occupiedSquare + direction);
        }
        TryToAddCastlingMoves();
        return availableMoves;
    }

    // TODO(mrsn): implement castling.
    private void TryToAddCastlingMoves()
    {
        if (hasMoved)
            return;
        Piece leftPiece = board.GetPieceOnSquare(new Vector2Int(0, occupiedSquare.y));
        if (leftPiece && leftPiece.IsFromSameTeam(this) && !leftPiece.hasMoved && leftPiece is Rook
            && board.GetPieceOnSquare(new Vector2Int(1, occupiedSquare.y)) == null
            && board.GetPieceOnSquare(new Vector2Int(2, occupiedSquare.y)) == null
            && board.GetPieceOnSquare(new Vector2Int(3, occupiedSquare.y)) == null)
        {
            TryToAddMove(leftCastleSquare);
        }
        Piece rightPiece = board.GetPieceOnSquare(new Vector2Int(7, occupiedSquare.y));
        if (rightPiece && rightPiece.IsFromSameTeam(this) && !rightPiece.hasMoved && rightPiece is Rook
            && board.GetPieceOnSquare(new Vector2Int(6, occupiedSquare.y)) == null
            && board.GetPieceOnSquare(new Vector2Int(5, occupiedSquare.y)) == null)
        {
            TryToAddMove(rightCastleSquare);
        }
    }

    public override void MovePiece(Vector2Int coords)
    {
        base.MovePiece(coords);
        if (coords.Equals(leftCastleSquare))
        {
            Rook leftRook = (Rook) board.GetPieceOnSquare(new Vector2Int(0, coords.y));
            board.UpdateBoardOnPieceMove(coords + Vector2Int.right, leftRook.occupiedSquare, leftRook, null);
            leftRook.MovePiece(coords + Vector2Int.right);
        } else if (coords.Equals(rightCastleSquare))
        {
            Rook rightRook = (Rook) board.GetPieceOnSquare(new Vector2Int(7, coords.y));
            board.UpdateBoardOnPieceMove(coords + Vector2Int.left, rightRook.occupiedSquare, rightRook, null);
            rightRook.MovePiece(coords + Vector2Int.left);
        }
    }

    public override void SetData(Vector2Int squareCoords, TeamColor team, Board board)
    {
        base.SetData(squareCoords, team, board);
        leftCastleSquare = new Vector2Int(2, occupiedSquare.y);
        rightCastleSquare = new Vector2Int(6, occupiedSquare.y);
    }
}
