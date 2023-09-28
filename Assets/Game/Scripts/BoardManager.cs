using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public enum Games
{
    None,
    Chess,
    Checkers,
}

public class BoardManager : MonoBehaviour
{
    private Transform[] _squares;
    private GameObject[] _pieces;
    private Games _game;

    [Header("Limits")]
    public float MaxDistance;

    [Header("Prefabs")]
    public GameObject Piece;

    [Header("Colors")]
    public Material Player1;
    public Material Player2;

    [Header("Chess")]
    public Mesh King;
    public Mesh Queen;
    public Mesh Bishop;
    public Mesh Knight;
    public Mesh Rook;
    public Mesh Pawn;

    [Header("Checkers")]
    public Mesh Checker;

    [Header("Chess - Player1")]
    public Transform[] KingPos1;
    public Transform[] QueenPos1;
    public Transform[] BishopPos1;
    public Transform[] KnightPos1;
    public Transform[] RookPos1;
    public Transform[] PawnPos1;

    [Header("Chess - Player2")]
    public Transform[] KingPos2;
    public Transform[] QueenPos2;
    public Transform[] BishopPos2;
    public Transform[] KnightPos2;
    public Transform[] RookPos2;
    public Transform[] PawnPos2;

    [Header("Checkers - Player1")]
    public Transform[] CheckerPos1;

    [Header("Checkers - Player2")]
    public Transform[] CheckerPos2;

    [Header("Events")]
    public UnityEvent OnGameStarted;
    public UnityEvent OnGameStopped;

    public void ToggleToolTips()
    {
        foreach (var piece in _pieces ?? Array.Empty<GameObject>())
        {
            var tooltip = piece.GetComponent<ToolTipSpawner>();
            tooltip.enabled = !tooltip.enabled;
        }
    }

    public void RequestQuit()
    {
        Application.Quit();
    }

    public bool TryToSnap(Transform piece)
    {
        Transform nearestSquare = null;
        var nearestDistance = float.MaxValue;

        foreach (var square in _squares)
        {
            var distance = Vector3.Distance(square.position, piece.position);

            if (distance > MaxDistance || distance > nearestDistance)
                continue;

            nearestSquare = square;
            nearestDistance = distance;
        }

        if (nearestSquare == null)
            return false;

        SetLocation(piece, nearestSquare);
        return true;
    }

    public void ClearField()
    {
        foreach (var piece in _pieces ?? Array.Empty<GameObject>())
            Destroy(piece);

        _game = Games.None;
        OnGameStopped?.Invoke();
    }

    public void Reset()
    {
        var game = _game;
        ClearField();
        switch (game)
        {
            case Games.Chess:
                SpawnChess();
                break;

            case Games.Checkers:
                SpawnCheckers();
                break;
        }
    }

    public void SpawnChess()
    {
        ClearField();
        _pieces = SpawnChessEnumerable().ToArray();

        _game = Games.Chess;
        OnGameStarted?.Invoke();
    }

    public void SpawnCheckers()
    {
        ClearField();
        _pieces = SpawnCheckersEnumerable().ToArray();

        _game = Games.Checkers;
        OnGameStarted?.Invoke();
    }

    private IEnumerable<GameObject> SpawnChessEnumerable()
    {
        return SpawnPieces(KingPos1, King, Player1)
            .Concat(SpawnPieces(QueenPos1, Queen, Player1))
            .Concat(SpawnPieces(BishopPos1, Bishop, Player1))
            .Concat(SpawnPieces(KnightPos1, Knight, Player1))
            .Concat(SpawnPieces(RookPos1, Rook, Player1))
            .Concat(SpawnPieces(PawnPos1, Pawn, Player1))
            .Concat(SpawnPieces(KingPos2, King, Player2))
            .Concat(SpawnPieces(QueenPos2, Queen, Player2))
            .Concat(SpawnPieces(BishopPos2, Bishop, Player2))
            .Concat(SpawnPieces(KnightPos2, Knight, Player2))
            .Concat(SpawnPieces(RookPos2, Rook, Player2))
            .Concat(SpawnPieces(PawnPos2, Pawn, Player2));
    }

    private IEnumerable<GameObject> SpawnCheckersEnumerable()
    {
        return SpawnPieces(CheckerPos1, Checker, Player1)
            .Concat(SpawnPieces(CheckerPos2, Checker, Player2));
    }

    private IEnumerable<GameObject> SpawnPieces(IEnumerable<Transform> locations, Mesh piece, Material color)
    {
        return locations.Select(location => SpawnPiece(location, piece, color));
    }

    private readonly FieldInfo ToolTipSpawnerName = typeof(ToolTipSpawner)
        .GetField("toolTipText", BindingFlags.Instance | BindingFlags.NonPublic);

    private GameObject SpawnPiece(Transform location, Mesh piece, Material color)
    {
        var instance = Instantiate(Piece);

        var spawner = instance.GetComponent<ToolTipSpawner>();
        ToolTipSpawnerName.SetValue(spawner, $"{color.name} {piece.name}");

        instance.GetComponent<MeshFilter>().sharedMesh = piece;
        instance.GetComponent<MeshRenderer>().sharedMaterial = color;
        instance.GetComponent<MeshCollider>().sharedMesh = piece;
        SetLocation(instance.transform, location);

        return instance;
    }

    private static void SetLocation(Transform piece, Transform location)
    {
        piece.SetParent(location.parent);
        piece.localPosition = location.localPosition;
        piece.localRotation = location.localRotation;
        piece.localScale = location.localScale;
    }

    private void OnValidate()
    {
        if (_squares == null || !_squares.Any())
            _squares = GetComponentsInChildren<Transform>();
    }
}
