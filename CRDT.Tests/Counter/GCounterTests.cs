using CRDT.Counter;

namespace CRDT.Tests.Counter;

[TestClass]
public class GCounterTests
{
    [TestMethod]
    public void Test_Increment()
    {
        var gCounter = new GCounter();
        gCounter.Increment(1);
        Assert.AreEqual(1, gCounter.Query());
        gCounter.Increment(100);
        Assert.AreEqual(2, gCounter.Query());
    }

    private static GCounter CreateGCounter(params int[] nodes)
    {
        var gCounter = new GCounter();
        foreach (var node in nodes)
        {
            gCounter.Increment(node);
        }

        return gCounter;
    }

    private static (GCounter, GCounter, GCounter) PrepareGCounters()
    {
        var gCounter1 = CreateGCounter(1, 2, 3);
        var gCounter2 = CreateGCounter(2, 2);
        var gCounter3 = CreateGCounter(3, 3);
        return (gCounter1, gCounter2, gCounter3);
    }

    [TestMethod]
    public void Test_Merge()
    {
        var (g1, g2, g3) = PrepareGCounters();
        Assert.AreEqual(3, g1.Query());
        g1.Merge(g2);
        Assert.AreEqual(4, g1.Query());
        g1.Merge(g3);
        Assert.AreEqual(5, g1.Query());
    }

    [TestMethod]
    public void Test_Merge_Commutative()
    {
        var (g1, g2, g3) = PrepareGCounters();
        g1.Merge(g3);
        g1.Merge(g2);
        var (g4, g5, g6) = PrepareGCounters();
        g4.Merge(g6);
        g4.Merge(g5);
        Assert.AreEqual(g4.Query(), g1.Query());
    }

    [TestMethod]
    public void Test_Merge_Associative()
    {
        var (g1, g2, g3) = PrepareGCounters();
        g1.Merge(g3);
        g1.Merge(g2);
        var (g4, g5, g6) = PrepareGCounters();
        g5.Merge(g6);
        g4.Merge(g5);
        Assert.AreEqual(g4.Query(), g1.Query());
    }
    [TestMethod]
    public void Test_Merge_Idempotence()
    {
        var (g1, g2, g3) = PrepareGCounters();
        g1.Merge(g1);
        g2.Merge(g2);
        g3.Merge(g3);
        var (g4, g5, g6) = PrepareGCounters();
        Assert.AreEqual(g4.Query(), g1.Query());
        Assert.AreEqual(g5.Query(), g2.Query());
        Assert.AreEqual(g6.Query(), g3.Query());
    } 
}