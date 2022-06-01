using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        AddAvailableMovesInADirection(Vector2Int.down);
        AddAvailableMovesInADirection(Vector2Int.up);
        AddAvailableMovesInADirection(Vector2Int.left);
        AddAvailableMovesInADirection(Vector2Int.right);
        return availableMoves;
    }
}
