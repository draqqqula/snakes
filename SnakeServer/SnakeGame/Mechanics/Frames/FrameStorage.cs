using MessageSchemes;

namespace SnakeGame.Mechanics.Frames;

internal class FrameStorage
{
    private Dictionary<int, string> IdToAssetMatcher = new Dictionary<int, string>();
    private Dictionary<string, Dictionary<int, TransformFrame>> Frames { get; } = [];
    public void Add(int id, string asset, TransformFrame frame)
    {
        IdToAssetMatcher.Add(id, asset);
        if (!Frames.ContainsKey(asset))
        {
            Frames.Add(asset, []);
        }
        Frames[asset].Add(id, frame);
    }

    public void Remove(int id)
    {
        Frames[IdToAssetMatcher[id]].Remove(id);
        IdToAssetMatcher.Remove(id);
    }

    public IEnumerable<KeyValuePair<int, TransformFrame>> GetAll()
    {
        return Frames.SelectMany(it => it.Value);
    }

    public void ChangeAsset(int id, string newAsset)
    {
        var oldAsset = IdToAssetMatcher[id];
        IdToAssetMatcher.Remove(id);
        var frame = Frames[oldAsset][id];
        Frames[oldAsset].Remove(id);

        Add(id, newAsset, frame);
    }
    
    public EventMessage GetMessage()
    {
        var frames = Frames.Select(it => new Group() 
        { 
            Asset = it.Key, 
            Frames = ToFrames(it.Value) 
        }).ToList();

        return new EventMessage()
        {
            AngleEvents = [],
            SizeEvents = [],
            Created = frames,
            Disposed = [],
            PositionEvents = [],
        };
    }

    private IList<Frame> ToFrames(Dictionary<int, TransformFrame> pairs)
    {
        return pairs.Select(it => new Frame()
        {
            Position = new Vec2() { X = it.Value.Position.X, Y = it.Value.Position.Y },
            Angle = it.Value.Angle,
            Id = it.Key,
            Size = new Vec2() { X = it.Value.Size.X, Y = it.Value.Size.Y }
        }
        ).ToList();
    }
}
