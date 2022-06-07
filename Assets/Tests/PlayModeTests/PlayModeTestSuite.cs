using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayModeTestSuite
{
    private BoardInputHandler boardInputHandler;
    private Board board;

    [UnityTest]
    public IEnumerator CanMoveAPiece()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        boardInputHandler = GameObject.FindObjectOfType<BoardInputHandler>();
        board = GameObject.FindObjectOfType<Board>();

        ClickOnCoords(0, 1);
        ClickOnCoords(0, 3);

        yield return null;

        Assert.NotNull(board.GetPieceOnSquare(new Vector2Int(0, 3)));
    }
    private void ClickOnCoords(int x, int y)
    {
        boardInputHandler.ProcessInput(board.CalculatePositionFromCoords(new Vector2Int(x,y)), null, null);
    }
}
