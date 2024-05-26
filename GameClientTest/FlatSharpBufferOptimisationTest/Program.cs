

using MessageSchemesLight;

var pack = new PositionEventPack();
pack.List = [];
pack.List.Add(new PackedPositionEvent() { Id = 4, PosX = 1, PosY = 1 });
pack.List.Add(new PackedPositionEvent() { Id = 5, PosX = 2, PosY = 2 });
pack.List.Add(new PackedPositionEvent() { Id = 6, PosX = 3, PosY = 3 });
Console.WriteLine(PositionEventPack.Serializer.GetMaxSize(pack));