using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayModeTestSuite
{
    private BoardInputHandler boardInputHandler;
    private Board board;
    private ChessGameController chessGameController;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        boardInputHandler = GameObject.FindObjectOfType<BoardInputHandler>();
        board = GameObject.FindObjectOfType<Board>();
        chessGameController = GameObject.FindObjectOfType<ChessGameController>();
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

        Assert.False(chessGameController.IsGameInProgress());
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
        Assert.True(chessGameController.IsGameInProgress());
        King whiteKing = (King)board.GetPieceOnSquare(new Vector2Int(4, 7));
        Assert.That(whiteKing.availableMoves.Contains(new Vector2Int(5, 6)));
        ClickOnCoords(4, 7);
        ClickOnCoords(5, 6);
    }

    [UnityTest]
    public IEnumerator PromoteToCheckmate()
    {
        ClickOnCoords(5, 1);
        ClickOnCoords(5, 2);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        ClickOnCoords(5, 2);
        ClickOnCoords(5, 3);
        // Waiting move, just move knight back
        ClickOnCoords(0, 5);
        ClickOnCoords(1, 7);

        ClickOnCoords(5, 3);
        ClickOnCoords(5, 4);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        ClickOnCoords(5, 4);
        ClickOnCoords(5, 5);
        // Waiting move, just move knight back
        ClickOnCoords(0, 5);
        ClickOnCoords(1, 7);

        ClickOnCoords(5, 5);
        ClickOnCoords(4, 6);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        ClickOnCoords(3, 1);
        ClickOnCoords(3, 2);
        // Waiting move, just move knight back
        ClickOnCoords(0, 5);
        ClickOnCoords(1, 7);

        ClickOnCoords(2, 0);
        ClickOnCoords(6, 4);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        ClickOnCoords(4, 6);
        ClickOnCoords(3, 7);
        Assert.IsInstanceOf(typeof(Queen), board.GetPieceOnSquare(new Vector2Int(3, 7)));

        yield return null;
        Assert.False(chessGameController.IsGameInProgress());
    }

    [UnityTest]
    public IEnumerator Castle()
    {
        ClickOnCoords(4, 1);
        ClickOnCoords(4, 3);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        ClickOnCoords(6, 0);
        ClickOnCoords(5, 2);
        // Waiting move, just move knight back
        ClickOnCoords(0, 5);
        ClickOnCoords(1, 7);

        ClickOnCoords(5, 0);
        ClickOnCoords(2, 3);
        // Waiting move, just move knight somewhere
        ClickOnCoords(1, 7);
        ClickOnCoords(0, 5);

        ClickOnCoords(4, 0);
        ClickOnCoords(6, 0);
        // Waiting move, just move knight back
        ClickOnCoords(0, 5);
        ClickOnCoords(1, 7);

        Assert.IsInstanceOf(typeof(King), board.GetPieceOnSquare(new Vector2Int(6, 0)));
        Assert.IsInstanceOf(typeof(Rook), board.GetPieceOnSquare(new Vector2Int(5, 0)));

        yield return null;
    }

    private void ClickOnCoords(int x, int y)
    {
        boardInputHandler.ProcessInput(board.CalculatePositionFromCoords(new Vector2Int(x, y)), null, null);
    }
}
