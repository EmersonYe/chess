using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Finished }
    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;

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
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
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
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private bool CheckIfGameIsFinished()
    {
        // Check if opponent king is in check.
        ChessPlayer opponent = GetOpponentToPlayer(activePlayer);
        if (IsPlayerInCheck(opponent))
        {
            Debug.Log(opponent.team + " in check.");
            bool isOpponentKingSafe = false;
            // Check if opponent has any available moves to get out of check.
            foreach (Piece piece in opponent.activePieces)
            {
                foreach (Vector2Int coordsToMoveTo in piece.availableMoves)
                {
                    Piece pieceOnCoords = board.GetPieceOnSquare(coordsToMoveTo);
                    board.UpdateBoardOnPieceMove(coordsToMoveTo, piece.occupiedSquare, piece, pieceOnCoords);
                    activePlayer.GenerateAllPossibleMoves();
                    if (!IsPlayerInCheck(opponent))
                        isOpponentKingSafe = true;
                }
            }
            if (!isOpponentKingSafe)
                Debug.Log("Opponent checkmated. Game over.");
                return true;
        }    
        // Check for stalemate
        return false;
    }

    public bool IsPlayerInCheck(ChessPlayer player)
    {
        // For some reason this is returning true after the first move
        Vector2Int opponentKingCoords = player.king.occupiedSquare;
        int numPiecesAttackingKing = GetOpponentToPlayer(player).activePieces
            .Select(x => x.availableMoves)
            .Where(x => x.Contains(opponentKingCoords))
            .Count();
        return numPiecesAttackingKing > 0;

    }

    private void EndGame()
    {
        SetGameState(GameState.Finished);
    }

    private void ChangeActiveTeam()
    {
        activePlayer = GetOpponentToPlayer(activePlayer);
        Debug.Log("Active team: " + activePlayer.team);
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

    // Update is called once per frame
    void Update()
    {

    }
}
