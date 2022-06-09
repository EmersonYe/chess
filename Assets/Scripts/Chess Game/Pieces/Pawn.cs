using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : Piece
{
    public bool isVulnerableToEnPassant = false;
    private Vector2Int forward;
    public override List<Vector2Int> SelectAvailableSquares()
    {
        // Defining this here instead of Awake() because team is set AFTER Awake() is called.
        forward = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;

        availableMoves.Clear();
        if (!hasMoved && board.CheckIfCoordsAreOnBoard(occupiedSquare + forward + forward) && board.GetPieceOnSquare(occupiedSquare + forward + forward) == null)
        {
            TryToAddMove(occupiedSquare + forward + forward);
        }
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward) && board.GetPieceOnSquare(occupiedSquare + forward) == null)
        {
            TryToAddMove(occupiedSquare + forward);
        }
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward + Vector2Int.right)
            && board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.right) != null
            && !board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.right).IsFromSameTeam(this))
        {
            TryToAddMove(occupiedSquare + forward + Vector2Int.right);
        }
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward + Vector2Int.left)
            && board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.left) != null
            && !board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.left).IsFromSameTeam(this))
        {
            TryToAddMove(occupiedSquare + forward + Vector2Int.left);
        }
        TryToAddEnPassant();
        return availableMoves;
    }

    private void TryToAddEnPassant()
    {
        if (new[] { 0, 7 }.Contains((occupiedSquare + forward * 3).y))
        {
            Piece leftPiece = board.GetPieceOnSquare(occupiedSquare + Vector2Int.left);
            if (leftPiece && !this.IsFromSameTeam(leftPiece) && leftPiece is Pawn &&
                ((Pawn)leftPiece).isVulnerableToEnPassant)
            {
                TryToAddMove(occupiedSquare + forward + Vector2Int.left);
            }
            Piece rightPiece = board.GetPieceOnSquare(occupiedSquare + Vector2Int.right);
            if (rightPiece && !this.IsFromSameTeam(rightPiece) && rightPiece is Pawn &&
                ((Pawn)rightPiece).isVulnerableToEnPassant)
            {
                TryToAddMove(occupiedSquare + forward + Vector2Int.right);
            }
        }
    }

    public override void MovePiece(Vector2Int coords)
    {
        Vector2Int originalCoord = occupiedSquare;
        base.MovePiece(coords);
        if (coords.x != originalCoord.x && board.GetPieceOnSquare(coords - forward))
        {
            board.TryToCapture(coords - forward);
        }
        else if (coords.Equals(originalCoord + forward * 2))
        {
            isVulnerableToEnPassant = true;
        }
    }
}
