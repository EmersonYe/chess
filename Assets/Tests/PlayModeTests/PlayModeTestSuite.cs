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

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        boardInputHandler = GameObject.FindObjectOfType<BoardInputHandler>();
        board = GameObject.FindObjectOfType<Board>();
    }

    [UnityTest]
    public IEnumerator CanMoveAPiece()
    {
        ClickOnCoords(0, 1);
        ClickOnCoords(0, 3);

        yield return null;

        Assert.NotNull(board.GetPieceOnSquare(new Vector2Int(0, 3)));
    }

    [UnityTest]
    public IEnumerator FoolsMate()
    {
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

    [UnityTest]
    public IEnumerator KingCanTakeAttacker()
    {
        // 1. e3
        ClickOnCoords(4, 1);
        ClickOnCoords(4, 2);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        // 2. Qf3
        ClickOnCoords(3, 0);
        ClickOnCoords(5, 2);
        // Waiting move, just move knight back
        ClickOnCoords(0, 5);
        ClickOnCoords(1, 7);

        // 3. Qf7+, Kf7
        ClickOnCoords(5, 2);
        ClickOnCoords(5, 6);
        yield return null;
        Assert.True(GameObject.FindObjectOfType<ChessGameController>().IsGameInProgress());
        King whiteKing = (King) board.GetPieceOnSquare(new Vector2Int(4, 7));
        Assert.That(whiteKing.availableMoves.Contains(new Vector2Int(5, 6)));
        ClickOnCoords(4, 7);
        ClickOnCoords(5, 6);
    }

    private void ClickOnCoords(int x, int y)
    {
        boardInputHandler.ProcessInput(board.CalculatePositionFromCoords(new Vector2Int(x,y)), null, null);
    }
}
