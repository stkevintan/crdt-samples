using CRDT.Counter;

namespace CRDT.Tests.Counter;

[TestClass]
public class PNCounterTests
{
    [TestMethod]
    public void Test_Increment() {
        var pnCounter = new PNCounter();
        pnCounter.Increment(1);
        pnCounter.Increment(3);
        Assert.AreEqual(2, pnCounter.Query());
    }
    
    [TestMethod]
    public void Test_Decrement() {
        var pnCounter = new PNCounter();
        pnCounter.Decrement(1);
        pnCounter.Decrement(3);       
        Assert.AreEqual(-2, pnCounter.Query());
    }

    [TestMethod]
    public void Test_Inc_Dec()
    {
        var pnCounter = new PNCounter();
        pnCounter.Increment(1);
        pnCounter.Increment(2);
        Assert.AreEqual(2, pnCounter.Query());
        
        pnCounter.Decrement(1);
        Assert.AreEqual(1, pnCounter.Query());
        
        pnCounter.Increment(1);
        Assert.AreEqual(2, pnCounter.Query());
    }

    static (PNCounter, PNCounter, PNCounter) CreatePNCounters()
    {
        var counterA = new PNCounter();
        counterA.Increment(1);
        counterA.Increment(1);
        counterA.Decrement(2);

        var counterB = new PNCounter();
        counterB.Increment(2);
        counterB.Increment(1);
        counterB.Decrement(3);
        
        var counterC = new PNCounter();
        counterC.Increment(3);
        counterC.Decrement(6);
        return (counterA, counterB, counterC);
    }
    
    [TestMethod]
    public void Test_Merge()
    {
        var (c1, c2, c3) = CreatePNCounters();
        c1.Merge(c2);
        c1.Merge(c3);
        Assert.AreEqual(c1.Query(), 1, "PNCounter merge is not associative.");
    }
    
    [TestMethod]
    public void Test_Commutativity()
    {
        var (c1, c2, _) = CreatePNCounters();
        c1.Merge(c2);
        var (c3, c4, _) = CreatePNCounters();
        c4.Merge(c3);
        Assert.AreEqual(c1.Query(), c4.Query(), "PNCounter merge is not commutative.");
    }
    
    [TestMethod]
    public void Test_Associativity() {
        var (c1, c2, c3) = CreatePNCounters();
        c1.Merge(c2);
        c1.Merge(c3);
        var (c4, c5, c6) = CreatePNCounters();
        c4.Merge(c6);
        c4.Merge(c5);
        Assert.AreEqual(c1.Query(), c4.Query(), "PNCounter merge is not associative.");
    }
    
    [TestMethod]
    public void Test_Idempotence() {
        var (c1, c2, c3) = CreatePNCounters();
        c1.Merge(c1);
        c2.Merge(c2);
        c3.Merge(c3);
        Assert.AreEqual(c1.Query(), c1.Query(), "PNCounter merge is not idempotent.");
        Assert.AreEqual(c2.Query(), c2.Query(), "PNCounter merge is not idempotent.");
        Assert.AreEqual(c3.Query(), c3.Query(), "PNCounter merge is not idempotent.");
    }
}