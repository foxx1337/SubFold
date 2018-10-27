using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace SubFold.Test
{
    [TestFixture]
    class SrtSubtitleTest
    {
        private readonly SrtSubtitle _subtitle;

        public SrtSubtitleTest()
        {
            _subtitle = new SrtSubtitle(
                new FileInfo(
                    Path.Combine(
                        TestContext.CurrentContext.TestDirectory, @"subs\sub1.srt")));
        }

        [Test]
        public void TestNumberLines()
        {
            Assert.AreEqual(4, _subtitle.Lines.Count);
        }

        [Test]
        public void TestTimestamps()
        {
            (TimeSpan, TimeSpan)[] timeSpans = {
                (new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0, 3, 512)),
                (new TimeSpan(0, 0, 0, 5, 100), new TimeSpan(0, 0, 0, 9, 900)),
                (new TimeSpan(0, 0, 12), new TimeSpan(0, 0, 18)),
                (new TimeSpan(0, 0, 19), new TimeSpan(0, 0, 0, 25, 123))
            };

            for (int i = 0; i < timeSpans.Length; i++)
            {
                var (start, stop) = timeSpans[i];
                Assert.That(_subtitle.Lines[i].Start, Is.EqualTo(start));
                Assert.That(_subtitle.Lines[i].Stop, Is.EqualTo(stop));
            }
        }

        [Test]
        public void TestSubContents()
        {
            List<List<string>> lines = new List<List<string>>
            {
                new List<string> {"This is a test"},
                new List<string> {"Hi", "What's happening?"},
                new List<string> {"Featuring text that's <i>italic</i>", "Innit nice?", "And a third line..."},
                new List<string> {"Featuring <i>italic</i>", "and <b>bold</b>", "and <u>underlined</u>", "and <b><i>bold italic</i></b>"}
            };

            for (int i = 0; i < lines.Count; i++)
            {
                Assert.That(_subtitle.Lines[i].Text.Count, Is.EqualTo(lines[i].Count));
                for (int j = 0; j < lines[i].Count; j++)
                {
                    Assert.That(_subtitle.Lines[i].Text[j], Is.EqualTo(lines[i][j]));
                }
            }
        }

    }
}
