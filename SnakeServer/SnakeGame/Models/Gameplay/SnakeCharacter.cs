﻿using SnakeGame.Common;
using SnakeGame.Mechanics.Bodies;
using SnakeGame.Mechanics.Collision;
using SnakeGame.Mechanics.Collision.Shapes;
using SnakeGame.Mechanics.Frames;
using SnakeGame.Models.Input.Internal;
using SnakeGame.Services.Gameplay.Abilities;
using System.Drawing;
using System.Numerics;
using SnakeGame.Systems.Movement;
using System.Text;
using ServerEngine.Models;

namespace SnakeGame.Models.Gameplay;

internal class SnakeCharacter : SquareBody
{
    public ClientIdentifier ClientId { get; set; }
    public CharacterAbility Ability { get; set; }
    public required List<SnakeBodypart> Body { get; init; }
    public required SquareBody Head { get; init; }

    public TeamColor Team { get; set; }

    #region Сharacteristics
    public float MovementDirection { get; set; } = 0f;
    public float Speed { get; set; } = 40f;
    public float BodyIndentation { get; set; } = 4f;
    public float RotationSpeed { get; set; } = MathF.PI * 2f;
    public float ShrinkSpeed { get; set; } = 80f;
    public bool OnBase { get; set; } = false;
    public float SortingIntervalMax { get; set; } = 0.5f;
    public float SortingIntervalMin { get; set; } = 0.1f;
    public float SortingIntervalCurrent { get; set; } = 0.5f;
    public float SortingIntervalDecrementStep { get; set; } = 0.05f;

    #endregion

    public byte MaxTier => Body.Max(it => it.Tier);

    private float SortingTimer { get; set; } = 0f;
    private bool ActiveSorting { get; set; } = false;

    public void Update(float deltaTime)
    {
        foreach (var bodypart in Body)
        {
            bodypart.Update(deltaTime);
        }

        if (!ActiveSorting)
        {
            SortingIntervalCurrent = SortingIntervalMax;
            return;
        }
        if (SortingTimer > SortingIntervalCurrent)
        {
            SortingTimer = 0f;
            SortingIntervalCurrent = MathF.Max(SortingIntervalCurrent - SortingIntervalDecrementStep, SortingIntervalMin);
            ActiveSorting = OneAtATime(Body.Count);
            return;
        }
        SortingTimer += deltaTime;
    }

    public bool OneAtATime(int index)
    {
        if (index < 2)
        {
            return false;
        }
        var last = Body[index - 1];
        var nextToLast = Body[index - 2];
        if (last.Tier > nextToLast.Tier)
        {
            Body[index - 1] = nextToLast;
            Body[index - 2] = last;
            var transit = last.Path.Cut(last.Path.TotalLength);
            if (transit is not null)
            {
                nextToLast.Path.ExtendBack(transit);
            }
            return true;
        }
        else if (last.Tier == nextToLast.Tier)
        {
            Body.RemoveAt(index - 1);
            last.Transform.Dispose();
            nextToLast.Tier += 1;
            nextToLast.UpdateAsset(Team);
            nextToLast.Bounce();

            if (last.Path.Head is not null)
            {
                nextToLast.Path.ExtendBack(last.Path.Head);
            }

            return true;
        }
        return OneAtATime(index - 1);
    }

    public void JoinLast(byte tier, FrameFactory factory)
    {
        var last = Body.LastOrDefault();
        last?.Path.Clear();
        var transform = last?.Transform.ReadOnly ?? Transform.ReadOnly;
        var newBodypart = new SnakeBodypart()
        {
            Transform = factory.Create($"body{tier}_{Team}", transform),
            Tier = tier
        };
        Body.Add(newBodypart);
        ActiveSorting = true;
    }

    private bool InvokeReaction(byte tier)
    {
        if (tier > 30)
        {
            return false;
        }

        foreach (var part in Body
            .AsEnumerable()
            .Reverse()
            .TakeWhile(it => it.Tier <= tier)
            .ToArray())
        {
            if (part.Tier == tier)
            {
                if (InvokeReaction((byte)(tier + 1)))
                {
                    Body.Remove(part);
                    part.Transform.Dispose();
                }
                else
                {
                    part.Tier += 1;
                    part.Transform.ChangeAsset($"body{part.Tier}_{Team}");
                }
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        if (Body.Count == 0)
        {
            return "empty";
        }
        var builder = new StringBuilder();
        foreach (var segment in Body)
        {
            builder.Append(segment.Value);
            builder.Append(">");
        }
        return builder.ToString();
    }
}
