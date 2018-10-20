using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubFold;

namespace Tests
{
    [TestClass]
    public class BasicReading
    {
        private SrtSubtitle subtitle;

        public BasicReading()
        {
            subtitle = new SrtSubtitle(@"subs\sub1.srt");
        }

        [TestMethod]
        public void TestNumberLines()
        {
            Assert.AreEqual(4, subtitle.Lines.Count);
        }

        [TestMethod]
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
                Assert.AreEqual(start, subtitle.Lines[i].Start);
                Assert.AreEqual(stop, subtitle.Lines[i].Stop);
            }
        }

        [TestMethod]
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
                Assert.AreEqual(lines[i].Count, subtitle.Lines[i].Text.Count);
                for (int j = 0; j < lines[i].Count; j++)
                {
                    Assert.AreEqual(lines[i][j], subtitle.Lines[i].Text[j]);
                }
            }
        }
    }
}
