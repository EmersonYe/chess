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

    [UnityTest]
    public IEnumerator FoolsMate()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        boardInputHandler = GameObject.FindObjectOfType<BoardInputHandler>();
        board = GameObject.FindObjectOfType<Board>();

        // 1. f3, e6
        ClickOnCoords(5, 1);
        ClickOnCoords(5, 2);
        ClickOnCoords(4, 6);
        ClickOnCoords(4, 5);
        // 2. g4, Qh4#
        ClickOnCoords(6, 1);
        ClickOnCoords(6, 3);
        ClickOnCoords(3, 7);
        ClickOnCoords(7, 3);
        yield return null;

        Assert.False(GameObject.FindObjectOfType<ChessGameController>().IsGameInProgress());
    }

    private void ClickOnCoords(int x, int y)
    {
        boardInputHandler.ProcessInput(board.CalculatePositionFromCoords(new Vector2Int(x,y)), null, null);
    }
}
