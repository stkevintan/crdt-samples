using CRDT.Set;

namespace CRDT.Tests.Set;

[TestClass]
public class GSetTests
{
    [TestMethod]
    public void TestAdd()
    {
        // Arrange: Create an instance of GSet
        var gSet = new GSet<int>();

        // Act: Add an element to the GSet
        const int elementToAdd = 5;
        gSet.Add(elementToAdd);

        // Assert: Verify that the element has been added
        Assert.IsTrue(gSet.Contains(elementToAdd), "The element was not found in the GSet after adding.");
    }

    [TestMethod]
    public void TestMerge()
    {
        // Arrange: Create two instances of GSet with unique elements
        var gSet1 = new GSet<int>();
        const int element1 = 5;
        gSet1.Add(element1);

        var gSet2 = new GSet<int>();
        const int element2 = 10;
        gSet2.Add(element2);

        // Act: Merge the second GSet into the first one
        gSet1.Merge(gSet2);

        // Assert: Verify that both elements are now in the first GSet
        Assert.IsTrue(gSet1.Contains(element1), "Element from the first set was not found after merging.");
        Assert.IsTrue(gSet1.Contains(element2), "Element from the second set was not found after merging.");

        // Optionally, you can also verify the size of the merged set
        Assert.AreEqual(2, gSet1.Query().Count(),
            "The merged set does not contain the expected number of elements.");
    }
}