using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        AddAvailableMovesInADirection(new Vector2Int(-1,-1));
        AddAvailableMovesInADirection(new Vector2Int(-1,1));
        AddAvailableMovesInADirection(new Vector2Int(1,-1));
        AddAvailableMovesInADirection(new Vector2Int(1,1));
        return availableMoves;
    }
}
