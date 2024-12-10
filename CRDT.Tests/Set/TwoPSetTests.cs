using CRDT.Set;

namespace CRDT.Tests.Set;

[TestClass]
public class TwoPSetTests
{
    [TestMethod]
    public void TestAdd()
    {
        // Arrange: Initialize a TwoPSet and an element to add
        var twoPSet = new TwoPSet<int>();
        const int elementToAdd = 5;

        // Act: Add the element
        twoPSet.Add(elementToAdd);

        // Assert: Check if the element is in the set
        Assert.IsTrue(twoPSet.Contains(elementToAdd), "The element was not found in the TwoPSet after adding.");
    }

    [TestMethod]
    public void TestRemove()
    {
        // Arrange: Initialize a TwoPSet, add an element, and then remove it
        var twoPSet = new TwoPSet<int>();
        int elementToRemove = 7;
        twoPSet.Add(elementToRemove);
        twoPSet.Remove(elementToRemove);

        // Assert: Ensure the element is not present after removal
        Assert.IsFalse(twoPSet.Contains(elementToRemove), "The element was found in the TwoPSet after removing.");
    }

    [TestMethod]
    public void TestAddBack()
    {
        // Arrange: Initialize a TwoPSet, add an element, remove it, and then attempt to add it back
        var twoPSet = new TwoPSet<int>();
        int elementToAddBack = 9;
        twoPSet.Add(elementToAddBack);
        twoPSet.Remove(elementToAddBack);
        twoPSet.Add(elementToAddBack);
        Assert.IsFalse(twoPSet.Contains(elementToAddBack),
            "The element was found in the TwoPSet after adding back.");
    }

    [TestMethod]
    public void TestMerge()
    {
        // Arrange: Create two instances of TwoPSet with unique elements
        var twoPSet1 = new TwoPSet<int>(1, 2);
        var twoPSet2 = new TwoPSet<int>(3, 4);

        // Act: Merge the second TwoPSet into the first one
        twoPSet1.Merge(twoPSet2);

        // Assert: Verify that all elements from both sets are now in the first TwoPSet
        int[] expectedElements = [1, 2, 3, 4];
        CollectionAssert.AreEquivalent(expectedElements, twoPSet1.Query().ToList(),
            "The merged set does not contain the expected elements.");
    }

    [TestMethod]
    public void TestMerge_Commutative()
    {
        // Arrange: Create two instances of TwoPSet with unique elements
        var twoPSetA = new TwoPSet<int>(1, 2, 3);
        var twoPSetB = new TwoPSet<int>(4, 5, 6);

        // Act: Merge A into B, and also B into A
        var mergedAB = new TwoPSet<int>(twoPSetA);
        mergedAB.Merge(twoPSetB);

        var mergedBA = new TwoPSet<int>(twoPSetB);
        mergedBA.Merge(twoPSetA);

        // Assert: Verify that merging A into B yields the same result as merging B into A
        CollectionAssert.AreEquivalent(mergedAB.Query().ToList(), mergedBA.Query().ToList(),
            "The merge operation is not commutative; the order of merging affects the result.");
    }

    [TestMethod]
    public void TestMerge_Associative()
    {
        // Arrange: Create three instances of TwoPSet with unique elements
        var twoPSetA = new TwoPSet<int>(1, 2);
        var twoPSetB = new TwoPSet<int>(3, 4);
        var twoPSetC = new TwoPSet<int>(5, 6);

        // Act: Perform the merges in two different orders
        var mergedABC = new TwoPSet<int>(twoPSetA);
        mergedABC.Merge(twoPSetB);
        mergedABC.Merge(twoPSetC);

        var mergedBC = new TwoPSet<int>(twoPSetB);
        mergedBC.Merge(twoPSetC);
        var mergedACB = new TwoPSet<int>(twoPSetA);
        mergedACB.Merge(mergedBC);

        // Assert: Verify that the associative property holds true
        CollectionAssert.AreEquivalent(mergedABC.Query().ToList(), mergedACB.Query().ToList(),
            "The merge operation is not associative; the grouping of elements during merge affects the result.");
    }

    [TestMethod]
    public void TestMerge_Idempotence()
    {
        // Arrange: Initialize a TwoPSet with some elements
        var originalTwoPSet = new TwoPSet<int>(1, 2, 3);

        // Act: Merge the TwoPSet with itself
        var mergedTwoPSet = new TwoPSet<int>(originalTwoPSet);
        mergedTwoPSet.Merge(originalTwoPSet);

        // Assert: Verify that merging the TwoPSet with itself doesn't change its content
        CollectionAssert.AreEquivalent(originalTwoPSet.Query().ToList(), mergedTwoPSet.Query().ToList(),
            "Merging a TwoPSet with itself did not yield an idempotent result.");
    }
}