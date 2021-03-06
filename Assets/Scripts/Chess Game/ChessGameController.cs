using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Finished }
    // TODO(mrsn): figure out how to make this field private or internal.
    // This field is public to expose it to tests. This is an antipattern.
    [SerializeField] public BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private ChessUIManager uiManager;

    private PieceCreator pieceCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;
    private GameState gameState;

    private void Awake()
    {
        SetDependencies();
        CreatePlayers();
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }
    private void CreatePlayers()
    {
        whitePlayer = gameObject.AddComponent<ChessPlayer>();
        blackPlayer = gameObject.AddComponent<ChessPlayer>();
        whitePlayer.InitializeChessPlayer(TeamColor.White, board);
        blackPlayer.InitializeChessPlayer(TeamColor.Black, board);
    }


    // Start is called before the first frame update
    void Start()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        uiManager.HideUI();
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game.");
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public bool IsGameInProgress()
    {
        return gameState.Equals(GameState.Play);
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    internal bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public void EndTurn()
    {
        ChessPlayer opponent = GetOpponentToPlayer(activePlayer);

        UpdateEnPassantVulnerability(opponent);
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(opponent);
        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private void UpdateEnPassantVulnerability(ChessPlayer chessPlayer)
    {
        foreach (Piece piece in chessPlayer.activePieces)
        {
            if (piece is Pawn)
                ((Pawn)piece).isVulnerableToEnPassant = false;
        }
    }

    internal void OnPieceRemoved(Piece piece)
    {
        ChessPlayer owner = GetPlayer(piece.team);
        owner.removePiece(piece);
        Destroy(piece.gameObject);
    }
    internal void Promote(Piece piece)
    {
        Vector2Int coords = piece.occupiedSquare;
        OnPieceRemoved(piece);
        CreatePieceAndInitialize(coords, piece.team, typeof(Queen));
    }

    private bool CheckIfGameIsFinished()
    {
        // Check if opponent king is in check.
        ChessPlayer opponent = GetOpponentToPlayer(activePlayer);
        bool isPlayerInCheck = IsPlayerInCheck(opponent);
        // Only remove self-check moves after seeing if king is in check.
        RemoveMovesThatPutOwnKingIntoCheck(opponent);
        if (isPlayerInCheck)
        {
            Debug.Log(opponent.team + " in check.");
            bool isOpponentKingSafe = false;
            // Check if opponent has any available moves to get out of check.
            foreach (Piece piece in opponent.activePieces)
            {
                foreach (Vector2Int coordsToMoveTo in piece.availableMoves)
                {
                    Vector2Int pieceOriginalOccupiedSquare = piece.occupiedSquare;
                    Piece pieceOnCoords = board.GetPieceOnSquare(coordsToMoveTo);
                    // need to also update piece.occupied square
                    board.UpdateBoardOnPieceMove(coordsToMoveTo, pieceOriginalOccupiedSquare, piece, null);
                    if (pieceOnCoords != null)
                    {
                        GetPlayer(pieceOnCoords.team).removePiece(pieceOnCoords);
                    }
                    activePlayer.GenerateAllPossibleMoves();
                    if (!IsPlayerInCheck(opponent))
                    {
                        // TODO(mrsn): consider breaking out of loop here
                        isOpponentKingSafe = true;
                    }
                    board.UpdateBoardOnPieceMove(pieceOriginalOccupiedSquare, coordsToMoveTo, piece, pieceOnCoords);
                    if (pieceOnCoords != null)
                    {
                        GetPlayer(pieceOnCoords.team).addPiece(pieceOnCoords);
                    }
                }
            }
            if (!isOpponentKingSafe)
            {
                Debug.Log("Opponent checkmated. Game over.");
                return true;
            }
            activePlayer.GenerateAllPossibleMoves();
        }

        // Check for stalemate
        if (!HasAvailableMoves(opponent))
        {
            return true;
        }

        // TODO(mrsn): Check for stalemate by repetition.
        return false;
    }

    private bool HasAvailableMoves(ChessPlayer player)
    {
        foreach (Piece piece in player.activePieces)
        {
            if (piece.availableMoves.Count > 0)
            {
                return true;
            }
        }
        return false;

    }

    private void RemoveMovesThatPutOwnKingIntoCheck(ChessPlayer player)
    {
        for (int i = 0; i < player.activePieces.Count; i++)
        {
            Piece piece = player.activePieces[i];
            // Cacheing coords to remove instead of removing in the foreach loop because cannot modify the enum during the loop.
            List<Vector2Int> coordsToRemove = new List<Vector2Int>();
            for (int j = 0; j < piece.availableMoves.Count; j++)
            {
                Vector2Int pieceOriginalOccupiedSquare = piece.occupiedSquare;
                Vector2Int coordsToMoveTo = piece.availableMoves[j];
                Piece pieceOnCoords = board.GetPieceOnSquare(coordsToMoveTo);
                board.UpdateBoardOnPieceMove(coordsToMoveTo, pieceOriginalOccupiedSquare, piece, null);
                piece.occupiedSquare = coordsToMoveTo;
                if (pieceOnCoords)
                {
                    GetPlayer(pieceOnCoords.team).removePiece(pieceOnCoords);
                }
                GetOpponentToPlayer(player).GenerateAllPossibleMoves();
                if (IsPlayerInCheck(player))
                {
                    coordsToRemove.Add(coordsToMoveTo);
                }
                board.UpdateBoardOnPieceMove(pieceOriginalOccupiedSquare, coordsToMoveTo, piece, pieceOnCoords);
                piece.occupiedSquare = pieceOriginalOccupiedSquare;
                // Not sure if this is needed
                if (pieceOnCoords)
                {
                    GetPlayer(pieceOnCoords.team).addPiece(pieceOnCoords);
                }
            }
            piece.availableMoves.RemoveAll(x => coordsToRemove.Contains(x));
        }
    }

    public bool IsPlayerInCheck(ChessPlayer player)
    {
        // Necessary to prevent hanging when king is not initialized.
        if (player.king == null)
            return false;
        Vector2Int kingCoords = player.king.occupiedSquare;
        List<Piece> opponentsPieces = GetOpponentToPlayer(player).activePieces;
        foreach (Piece opponentsPiece in opponentsPieces)
        {
            if (opponentsPiece.availableMoves.Contains(kingCoords))
            {
                return true;
            }
        }
        return false;
    }
    internal ChessPlayer GetPlayer(TeamColor team)
    {
        return team.Equals(TeamColor.White) ? whitePlayer : blackPlayer;
    }

    private void EndGame()
    {
        uiManager.OnGameFinished(activePlayer.team.ToString());
        SetGameState(GameState.Finished);
    }

    private void ChangeActiveTeam()
    {
        activePlayer = GetOpponentToPlayer(activePlayer);
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player.Equals(whitePlayer) ? blackPlayer : whitePlayer;
    }

    private void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teammaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teammaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.addPiece(newPiece);

        if (type.ToString().Equals("King"))
        {
            currentPlayer.king = (King)newPiece;
        }
    }
}
