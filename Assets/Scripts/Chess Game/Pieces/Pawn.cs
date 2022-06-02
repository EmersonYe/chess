using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private Vector2Int forward;
    public override List<Vector2Int> SelectAvailableSquares()
    {
        // Defining this here instead of Awake() because team is set AFTER Awake() is called.
        forward = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;

        availableMoves.Clear();
        if (!hasMoved && board.CheckIfCoordsAreOnBoard(occupiedSquare + forward + forward) && board.GetPieceOnSquare(occupiedSquare + forward + forward) == null)
        {
            availableMoves.Add(occupiedSquare + forward + forward);
        }
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward) && board.GetPieceOnSquare(occupiedSquare + forward) == null)
        {
            availableMoves.Add(occupiedSquare + forward);
        }
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward + Vector2Int.right)
            && board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.right) != null
            && !board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.right).IsFromSameTeam(this))
        {
            availableMoves.Add(occupiedSquare + forward + Vector2Int.right);
        }
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward + Vector2Int.left)
            && board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.left) != null
            && !board.GetPieceOnSquare(occupiedSquare + forward + Vector2Int.left).IsFromSameTeam(this))
        {
            availableMoves.Add(occupiedSquare + forward + Vector2Int.left);
        }
        return availableMoves;
    }
}
