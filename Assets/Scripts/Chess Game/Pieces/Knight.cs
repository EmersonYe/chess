using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    private Vector2Int[] directions = new Vector2Int[] {
        new Vector2Int(-1,-2),
        new Vector2Int(-2,-1),
        new Vector2Int(-2,1),
        new Vector2Int(-1,2),
        new Vector2Int(1,2),
        new Vector2Int(2,1),
        new Vector2Int(2,-1),
        new Vector2Int(1,-2),
    };
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        foreach (Vector2Int direction in directions)
        {
            if (IsDirectionAvailableMove(direction))
                availableMoves.Add(occupiedSquare + direction);
        }
        return availableMoves;
    }
}
