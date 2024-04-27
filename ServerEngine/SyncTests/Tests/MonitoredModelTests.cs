using NUnit.Framework;

namespace SyncTests.Tests;

[TestFixture]
internal class MonitoredModelTests
{
    [Test]
    public void CreateMassiveAmountMonitored()
    {
        for (int i = 0; i < 100000; i++)
        {
            var model = new TestModelMonitored();
        }
    }

    [Test]
    public void CreateMassiveAmountPure()
    {
        for (int i = 0; i < 100000; i++)
        {
            var model = new MyAggregate();
        }
    }

    [Test]
    public void EnlistMonitored()
    {
        var list = new List<TestModelMonitored>();
        for (int i = 0; i < 100000; i++)
        {
            var model = new TestModelMonitored();
            list.Add(model);
        }
    }

    [Test]
    public void EnlistPure()
    {
        var list = new List<MyAggregate>();
        for (int i = 0; i < 100000; i++)
        {
            var model = new MyAggregate();
            list.Add(model);
        }
    }

    [Test]
    public void AccessMonitored()
    {
        var list = new List<TestModelMonitored>();
        for (int i = 0; i < 100000; i++)
        {
            var model = new TestModelMonitored();
            list.Add(model);
        }

        foreach (var model in list)
        {
            var i = model.Integer;
            var f = model.Float;
            var t = model.Text;
            var r = model.Rectangle;
            var v = model.Point;
        }    
    }

    [Test]
    public void AccessPure()
    {
        var list = new List<MyAggregate>();
        for (int i = 0; i < 100000; i++)
        {
            var model = new MyAggregate();
            list.Add(model);
        }

        foreach (var model in list)
        {
            var i = model.Integer;
            var f = model.Float;
            var t = model.Text;
            var r = model.Rectangle;
            var v = model.Point;
        }
    }

    [Test]
    public void FindDeltaMonitored()
    {
        var list = new List<TestModelMonitored>();
        for (int i = 0; i < 100000; i++)
        {
            var model = new TestModelMonitored();
            list.Add(model);
        }

        var fieldId = 0;
        foreach (var model in list)
        {
            switch (fieldId)
            {

            }
            fieldId += 1;
        }
    }

    [Test]
    public void FindDeltaPure()
    {
        var list = new List<MyAggregate>();
        for (int i = 0; i < 100000; i++)
        {
            var model = new MyAggregate();
            list.Add(model);
        }

        foreach (var model in list)
        {
            var i = model.Integer;
            var f = model.Float;
            var t = model.Text;
            var r = model.Rectangle;
            var v = model.Point;
        }
    }
}
