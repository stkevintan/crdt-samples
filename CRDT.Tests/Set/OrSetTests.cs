using CRDT.Set;

namespace CRDT.Tests.Set;

[TestClass]
public class OrSetTests
{
    [TestMethod]
    public void TestAdd()
    {
        var orSet = new OrSet<int>();

        orSet.Add(1, 1);
        orSet.Add(2, 1);
        CollectionAssert.AreEquivalent(new List<int> { 1, 2 }, orSet.ToList());
    }

    [TestMethod]
    public void TestRemove()
    {
        // Arrange
        var orSet = new OrSet<int>();
        const int node = 1; // Assuming we're using a single node for simplicity

        // Act
        orSet.Add(1, node); // Add an element
        orSet.Add(2, node); // Add another element
        orSet.Remove(1, node); // Remove the first added element

        // Assert
        // Check if the removed element is no longer present
        Assert.IsFalse(orSet.Contains(1), "Element 1 was not successfully removed.");
        // Ensure the other element still exists
        Assert.IsTrue(orSet.Contains(2), "Element 2 should still be present after removing element 1.");
    }

    [TestMethod]
    public void TestAddBack()
    {
        // Arrange
        var orSet = new OrSet<int>();
        const int node = 1;

        // Act
        orSet.Add(1, node); // Initially add the element
        orSet.Remove(1, node); // Remove the element
        orSet.Add(1, node); // Add it back (simulating "AddBack")

        // Assert
        // Verify that the element is now part of the set after adding it back
        Assert.IsTrue(orSet.Contains(1), "Element 1 was not successfully added back.");
    }

    [TestMethod]
    public void TestAddBack_ConCurrent()
    {
        // Arrange
        var orSet = new OrSet<int>();
        const int node = 1;

        // Act
        orSet.Add(1, node); // Initially add the element
        orSet.Remove(1, 2); // Remove the element
        orSet.Add(1, 2); // Add it back (simulating "AddBack")

        // Assert
        // Verify that the element is now part of the set after adding it back
        Assert.IsTrue(orSet.Contains(1), "Element 1 was not successfully added back.");
    }

    [TestMethod]
    public void TestMerge()
    {
        // Arrange
        var orSet1 = new OrSet<int>();
        var orSet2 = new OrSet<int>();

        orSet1.Add(1, 1); // Element 1 added in node 1 for orSet1
        orSet1.Remove(2, 1); // Element 2 removed in node 1 for orSet1
        orSet2.Add(2, 2); // Element 2 added in node 2 for orSet2
        orSet2.Add(3, 2); // Element 3 added in node 2 for orSet2

        // Act
        orSet1.Merge(orSet2);

        // Assert
        // After merging, orSet1 should contain 1 (from itself) and 2, 3 (from orSet2)
        // Element 2 was removed in orSet1 but added again in orSet2, so it should be present
        // Element 3 is new from orSet2
        CollectionAssert.AreEquivalent(new List<int> { 1, 3 }, orSet1.ToList());

        // Also verify individual elements' presence
        Assert.IsTrue(orSet1.Contains(1), "Element 1 should be present after merge.");
        Assert.IsFalse(orSet1.Contains(2), "Element 2 should not be present after merge despite prior removal.");
        Assert.IsTrue(orSet1.Contains(3), "Element 3 should be present after merge.");
    }
}