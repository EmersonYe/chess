using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Board/Layout")]
public class BoardLayout : ScriptableObject
{
    // TODO(mrsn): figure out how to make this class private or internal.
    // This class is public to expose it to tests. Probably an antipattern.
    [Serializable]
    public class BoardSquareSetup
    {
        public Vector2Int position;
        public PieceType pieceType;
        public TeamColor teamColor;
    }

    [SerializeField] private BoardSquareSetup[] boardSquares;

    public int GetPiecesCount()
    {
        return boardSquares.Length;
    }

    public Vector2Int GetSquareCoordsAtIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range.");
            throw new IndexOutOfRangeException("Index: " + index + "out of range: " + boardSquares.Length);
        }
        return new Vector2Int(boardSquares[index].position.x - 1, boardSquares[index].position.y - 1);
    }

    public string GetSquarePieceNameAtIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range.");
            throw new IndexOutOfRangeException("Index: " + index + "out of range: " + boardSquares.Length);
        }
        return boardSquares[index].pieceType.ToString();
    }

    public TeamColor GetSquareTeamColorAtIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range.");
            throw new IndexOutOfRangeException("Index: " + index + "out of range: " + boardSquares.Length);
        }
        return boardSquares[index].teamColor;
    }
    
    // Used for testing.
    public void SetBoardSquares(BoardSquareSetup[] boardSquares)
    {
        this.boardSquares = boardSquares;
    }

}
