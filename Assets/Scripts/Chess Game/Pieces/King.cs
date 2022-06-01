using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    private Vector2Int[] directions = new Vector2Int[] { new Vector2Int(-1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,1),
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up
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
