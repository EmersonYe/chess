using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IObjectTweener))]
[RequireComponent(typeof(MaterialSetter))]
public abstract class Piece : MonoBehaviour
{
    private MaterialSetter materialSetter;

    public Board board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public TeamColor team { get; set; }
    public bool hasMoved { get; private set; }
    public List<Vector2Int> availableMoves;

    private IObjectTweener tweener;

    public abstract List<Vector2Int> SelectAvailableSquares();

    private void Awake()
    {
        availableMoves = new List<Vector2Int>();
        tweener = GetComponent<IObjectTweener>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial(Material material)
    {
        // Not sure why this is needed but sometimes material setter doesn't get set at this point.
        if (materialSetter == null)
        {
            materialSetter = GetComponent<MaterialSetter>();
        }
        materialSetter.SetSingleMaterial(material);
    }

    public bool IsFromSameTeam(Piece piece)
    {
        return team == piece.team;
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        return availableMoves.Contains(coords);
    }

    public virtual void MovePiece(Vector2Int coords)
    {
        Vector3 targetPosition = board.CalculatePositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;
        tweener.MoveTo(transform, targetPosition);
    }

    protected void TryToAddMove(Vector2Int coords)
    {
        availableMoves.Add(coords);
    }

    public virtual void SetData(Vector2Int squareCoords, TeamColor team, Board board)
    {
        this.team = team;
        occupiedSquare = squareCoords;
        this.board = board;
        transform.position = board.CalculatePositionFromCoords(squareCoords);
    }

    protected void AddAvailableMovesInADirection(Vector2Int direction)
    {
        Vector2Int squareToCheck = occupiedSquare + direction;
        while (board.CheckIfCoordsAreOnBoard(squareToCheck))
        {
            Piece pieceInTheWay = board.GetPieceOnSquare(squareToCheck);
            if (pieceInTheWay != null)
            {
                if (!IsFromSameTeam(pieceInTheWay))
                {
                    TryToAddMove(squareToCheck);
                }
                return;
            }
            TryToAddMove(squareToCheck);
            squareToCheck += direction;
        }
    }

    protected bool IsDirectionAvailableMove(Vector2Int direction)
    {
        Vector2Int squareToCheck = occupiedSquare + direction;
        if (board.CheckIfCoordsAreOnBoard(squareToCheck))
        {
            Piece pieceInTheWay = board.GetPieceOnSquare(squareToCheck);
            if (pieceInTheWay == null || !IsFromSameTeam(pieceInTheWay))
            {
                return true;
            }
        }
        return false;
    }
}
