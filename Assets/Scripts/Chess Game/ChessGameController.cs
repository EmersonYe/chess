using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;

    private PieceCreator pieceCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;

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
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
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
        ChangeActiveTeam();
    }

    private void ChangeActiveTeam()
    {
        activePlayer = GetOpponentToPlayer(activePlayer);
        Debug.Log("Active team: " + activePlayer.team);
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        Debug.Log(player + ".Equals("+whitePlayer+"): " + player.Equals(whitePlayer));
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
