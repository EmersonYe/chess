using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private Vector2Int forward;
    private void Awake()
    {
        forward = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
    }
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        if (board.CheckIfCoordsAreOnBoard(occupiedSquare + forward) && board.GetPieceOnSquare(occupiedSquare + forward) == null)
        {
            availableMoves.Add(occupiedSquare + forward);
        }
        return availableMoves;
    }
}
