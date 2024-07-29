using BocagoiConsole.Common;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class PartitionListElementTests
    {
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 2)]
        [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 8)]
        public void PartitioningListElements_CreatesNewListr_AndRemoveFromOriginal(int[] elements, int elsInPartition)
        {
            var list = new List<int>(elements);

            var newList = list.PartitionListElements(elsInPartition);

            Assert.That(elsInPartition == newList.Count, "New list should have " + elsInPartition + " elements");
            Assert.That(elements.Length - elsInPartition == list.Count, "Original list should have removed elements: " + (elements.Length - elsInPartition));
        }


        [Test]
        public void PartitioningListElements_IfPartitionIsBiggerThatListCount_ReturnsEverythingAndLeavesTheOtherListEmpty()
        {
            var list = new List<int>(new[] { 1, 2, 3, 4, 5 });

            var newList = list.PartitionListElements(15);

            Assert.That(5 == newList.Count, "New list should have 5 lements");
            Assert.That(0 == list.Count, "Original list should be empty");
        }
    }
}