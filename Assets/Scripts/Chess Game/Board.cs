using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour
{
    public const int BOARD_SIZE = 8;

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;

    private Piece[,] grid;
    private Piece selectedPiece;
    private ChessGameController chessController;
    private SquareSelectorCreator squareSelectorCreator;

    private void Awake()
    {
        squareSelectorCreator = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessController)
    {
        this.chessController = chessController;
    }

    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }
    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        // TODO(mrsn): Consider moving this to BoardInputHandler.
        if (!chessController.IsGameInProgress())
            return;
        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);
        if(selectedPiece)
        {
            if(piece != null && selectedPiece == piece)
                DeselectPiece();
             else if(piece != null && selectedPiece != piece  && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
            else if (selectedPiece.CanMoveTo(coords))
                OnSelectedPieceMoved(coords, selectedPiece);
        } else
        {
            if (piece != null && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
        }
    }

    internal void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }

    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece)
    {
        TryToCapture(coords);
        UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coords);
        DeselectPiece();
        EndTurn();
    }

    private void TryToCapture(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        if(piece != null && !selectedPiece.IsFromSameTeam(piece))
        {
            CapturePiece(piece);
        }
    }

    private void CapturePiece(Piece piece)
    {
        if (piece != null)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
        }
    }

    private void EndTurn()
    {
        chessController.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }

    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        //TODO(mrsn): consider adding a call to chessController.removeMovesThatPutOwnKingIntoCheck here.
        ShowSelectionSquares(selectedPiece.availableMoves);
    }

    private void ShowSelectionSquares(List<Vector2Int> availableMoves)
    {
        Dictionary<Vector3, bool> squareData = new Dictionary<Vector3, bool>();
        foreach (Vector2Int availableMoveCoord in availableMoves)
        {
            Vector3 position = CalculatePositionFromCoords(availableMoveCoord);
            bool isSquareFree = GetPieceOnSquare(availableMoveCoord) == null;
            squareData.Add(position, isSquareFree);
        }
        squareSelectorCreator.ShowSelection(squareData);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        squareSelectorCreator.ClearSelection();
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if(!CheckIfCoordsAreOnBoard(coords))
            return null;
        return grid[coords.x, coords.y];
    }

    public bool CheckIfCoordsAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
        {
            return false;
        }
        return true;
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (CheckIfCoordsAreOnBoard(coords))
        {
            grid[coords.x, coords.y] = piece;
        }
    }

    public bool HasPiece(Piece piece)
    {
        foreach (var gridPiece in grid)
        {
            if (piece.Equals(gridPiece))
            {
                return true;
            }
        }
        return false;
    }
}
