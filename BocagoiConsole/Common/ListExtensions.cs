using System;
using System.Collections.Generic;

namespace BocagoiConsole.Common;

public static class ListExtensions
{
    public static IList<T> PartitionListElements<T>(this List<T> list, int elementsInPartition, Random rand = null)
    {
        if (rand == null)
            rand = new UniqueRandom();

        var wordsLeft = new List<T>();

        for (int i = 0; i < elementsInPartition; ++i)
        {
            var index = rand.Next(0, list.Count - 1);
            wordsLeft.Add(list[index]);
            list.RemoveAt(index);

            if (list.Count == 0)
                break;
        }

        return wordsLeft;
    }
}
