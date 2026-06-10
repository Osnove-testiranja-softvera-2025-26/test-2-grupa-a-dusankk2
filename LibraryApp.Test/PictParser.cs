using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using LibraryApp.Models;

namespace LibraryApp.Test
{
   
    public static class PictParser
    {
        private static readonly string PictResultsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "PICT Results.txt");

       
        public static IEnumerable<TestCaseData> GetMemberDiscountTestCases()
        {
            Assert.That(
                File.Exists(PictResultsPath),
                Is.True,
                $"PICT fajl nije pronađen na putanji: {PictResultsPath}"
            );

            string[] lines = File.ReadAllLines(PictResultsPath);

            
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split('\t');

                if (parts.Length < 5)
                    continue;

                int totalBooksBorrowed = int.Parse(parts[0].Trim());
                int yearsOfMembership = int.Parse(parts[1].Trim());
                bool hasPenalty = bool.Parse(parts[2].Trim());

                ActivityFrequency activityFrequency =
                    (ActivityFrequency)Enum.Parse(
                        typeof(ActivityFrequency),
                        parts[3].Trim()
                    );

                int expectedDiscount = int.Parse(parts[4].Trim());

                yield return new TestCaseData(
                    totalBooksBorrowed,
                    yearsOfMembership,
                    hasPenalty,
                    activityFrequency,
                    expectedDiscount
                )
                .SetName(
                    $"GetMemberDiscount_Books={totalBooksBorrowed}_Years={yearsOfMembership}_Penalty={hasPenalty}_Activity={activityFrequency}_Expected={expectedDiscount}"
                );
            }
        }
    }
}