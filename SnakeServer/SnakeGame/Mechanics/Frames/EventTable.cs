using MessageSchemes;
using SnakeGame.Mechanics.Bodies;
using System;
using System.Collections;
using System.Numerics;
using SnakeCore.Collections;

namespace SnakeGame.Mechanics.Frames;

internal class EventTable
{
    private Dictionary<int, EventEntry> _data = [];
    public EventMessage Serialize()
    {
        var created = new List<FrameInfo>();
        var disposed = new List<int>();
        var positionEvents = new List<PositionEvent>();
        var sizeEvents = new List<SizeEvent>();
        var angleEvents = new List<AngleEvent>();
        var transfromations = new List<(int, string)>();

        foreach (var row in _data)
        {
            if (row.Value.Lifecycle == EventLifecycle.Create || row.Value.Lifecycle == EventLifecycle.Renew)
            {
                created.Add(new FrameInfo() 
                { 
                    Asset = row.Value.Asset ?? "error",
                    Frame = new Frame()
                    {
                        Id = row.Key,
                        Angle = row.Value.Angle ?? 0f,
                        Position = FrameUtils.SerializeVector2(row.Value.Position),
                        Size = FrameUtils.SerializeVector2(row.Value.Size)
                    }
                });
            }
            else if (row.Value.Lifecycle == EventLifecycle.Dispose || row.Value.Lifecycle == EventLifecycle.Renew)
            {
                disposed.Add(row.Key);
            }
            else if (row.Value.Lifecycle == EventLifecycle.Update)
            {
                if (row.Value.Position.HasValue)
                {
                    positionEvents.Add(new PositionEvent()
                    {
                        Id = row.Key,
                        Position = FrameUtils.SerializeVector2(row.Value.Position)
                    });
                }
                if (row.Value.Size.HasValue)
                {
                    sizeEvents.Add(new SizeEvent()
                    {
                        Id = row.Key,
                        Size = FrameUtils.SerializeVector2(row.Value.Size)
                    });
                }
                if (row.Value.Angle.HasValue)
                {
                    angleEvents.Add(new AngleEvent()
                    {
                        Id = row.Key,
                        Angle = row.Value.Angle.Value
                    });
                }
                if (row.Value.Asset is not null)
                {
                    transfromations.Add((row.Key, row.Value.Asset));
                }
            }
        }

        
        return new EventMessage()
        {
            AngleEvents = GetList(angleEvents),
            SizeEvents = GetList(sizeEvents),
            PositionEvents = GetList(positionEvents),
            Disposed = GetList(disposed),

            Created = GetList(created
            .GroupBy(it => it.Asset)
            .Select(it => new Group()
            {
                Asset = it.Key,
                Frames = it.Select(it => it.Frame).ToList()
            }).ToArray()),

            Transformations = GetList(transfromations
            .GroupBy(it => it.Item2)
            .Select(it => new Transformation()
            {
                NewAsset = it.Key,
                Frames = it.Select(it => it.Item1).ToList()
            })
            .ToArray())
        };
    }
    private static List<T>? GetList<T>(ICollection<T> collection)
    {
        return collection.Count > 0 ? collection.ToList() : null;
    }
    public EventTable Join(EventTable other)
    {
        return new EventTable()
        {
            _data = this._data
            .Join(other._data, 
            it => it.Key, 
            it => it.Key, 
            (older, newer) => KeyValuePair.Create(older.Key, older.Value.Join(newer.Value)))
            .Where(it => it.Value.IsValid)
            .ToDictionary()
        };
    }

    public EventEntry this[int id]
    {
        get 
        {
            if (_data.TryGetValue(id, out var value))
            {
                return value;
            }
            var events = new EventEntry();
            _data.Add(id, events);
            return events;
        }
    }

    public DivisionResult<EventTable> Divide(Func<int, EventEntry, bool> condition)
    {
        var include = new Dictionary<int, EventEntry>();
        var exclude = new Dictionary<int, EventEntry>();
        foreach (var entry in _data)
        {
            if (condition(entry.Key, entry.Value))
            {
                include.Add(entry.Key, entry.Value);
            }
            else
            {
                exclude.Add(entry.Key, entry.Value);
            }
        }
        var includeTable = new EventTable()
        {
            _data = include,
        };
        var excludeTable = new EventTable()
        {
            _data = exclude,
        };
        return new DivisionResult<EventTable>()
        {
            Include = includeTable,
            Exclude = excludeTable
        };
    }
}

internal class EventEntry
{
    public EventLifecycle Lifecycle { get; private set; } = EventLifecycle.Cancel;
    public string? Asset { get; set; }
    public Vector2? Position { get; set; }
    public float? Angle { get; set; }
    public Vector2? Size { get; set; }

    public bool IsValid =>
        Lifecycle != EventLifecycle.Cancel &&
        ((
        Lifecycle == EventLifecycle.Update &&
            (Asset is not null ||
            Position.HasValue ||
            Angle.HasValue ||
            Angle.HasValue))
        ||
        ((Lifecycle == EventLifecycle.Create || Lifecycle == EventLifecycle.Renew) && 
            (Asset is not null &&
            Position.HasValue &&
            Angle.HasValue &&
            Size.HasValue))
        ||
        Lifecycle == EventLifecycle.Dispose
        );

    public EventEntry Join(EventEntry other)
    {
        return new EventEntry()
        {
            Lifecycle = ResolveLifecycle(this.Lifecycle, other.Lifecycle),
            Asset = other.Asset ?? this.Asset,
            Position = other.Position ?? this.Position,
            Angle = other.Angle ?? this.Angle,
            Size = other.Size ?? this.Size,
        };
    }

    public void SetAction(EventLifecycle cycle)
    {
        if (cycle == EventLifecycle.Dispose)
        {
            Asset = null;
            Position = null;
            Angle = null;
            Size = null;
        }
        Lifecycle = ResolveLifecycle(this.Lifecycle, cycle);
    }

    private EventLifecycle ResolveLifecycle(EventLifecycle first, EventLifecycle second)
    {
        if ((first == EventLifecycle.Create || first == EventLifecycle.Renew) 
            && second == EventLifecycle.Dispose)
        {
            return EventLifecycle.Cancel;
        }
        else if (first == EventLifecycle.Dispose && 
            (second == EventLifecycle.Create || second == EventLifecycle.Renew))
        {
            return EventLifecycle.Renew;
        }
        else if (second == EventLifecycle.Update && first != EventLifecycle.Cancel)
        {
            return first;
        }
        else
        {
            return second;
        }
    }
}

internal enum EventLifecycle
{
    Create,
    Update,
    Dispose,
    Cancel,
    Renew
}