using BocagoiConsole.Core;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Diagnostics;

namespace Tests
{
    public class ScoreTests
    {
        [TestCase(new int [] { 0, 0, 0, 0, 0 }, ExpectedResult = 0)]
        [TestCase(new int [] { 1, 1, 1, 1, 1 }, ExpectedResult = 100)]
        [TestCase(new int [] { 1, 1, 0, 0}, ExpectedResult = 50)]
        [TestCase(new int [] { 1, 1, 0, 0, 0}, ExpectedResult = 40)]
        [TestCase(new int [] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1 }, ExpectedResult = 30)]
        public int DecimalScore_IsCorrect_BasedOnCorrectIncorrectRatios(int[] input)
        {
            var s = new Score();

            foreach (var i in input)
            {
                if (i == 0)
                    s.Incorrect();
                else
                    s.Correct();
            }
            
            Console.WriteLine("Test AAAAAAAAAAAAAAA");
            return s.DecimalScore();
        }
    }
}