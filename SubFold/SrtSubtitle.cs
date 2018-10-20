using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace SubFold
{
    public class SrtSubtitle : Subtitle
    {
        private const string TimestampFormat = @"hh\:mm\:ss\,fff";

        private const string TimestampSeparatorExpression = @"\s*-*>\s*";

        public SrtSubtitle(string fileName)
        {
            Lines = new List<Line>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                while (true)
                {
                    Line line = ReadSubtitleLine(reader);
                    if (line == null)
                    {
                        break;
                    }

                    Lines.Add(line);
                }
            }
        }

        private Line ReadSubtitleLine(StreamReader reader)
        {
            string textPosition = ReadFirstNonEmpty(reader);

            if (textPosition == null)
            {
                return null;
            }

            string textTimestamps = ReadFirstNonEmpty(reader);

            if (textTimestamps == null)
            {
                return null;
            }

            List<string> textLines = ReadUntilEmpty(reader);

            string[] timestamps = Regex.Split(textTimestamps, TimestampSeparatorExpression);

            if (timestamps.Length != 2)
            {
                return null;
            }

            try
            {
                TimeSpan start = TimeSpan.ParseExact(timestamps[0], TimestampFormat, CultureInfo.InvariantCulture);
                TimeSpan stop = TimeSpan.ParseExact(timestamps[1], TimestampFormat, CultureInfo.InvariantCulture);

                return new Line
                {
                    Start = start,
                    Stop = stop,
                    Text = textLines
                };
            }
            catch
            {
                return null;
            }
        }

        private string ReadFirstNonEmpty(StreamReader reader)
        {
            string line = reader.ReadLine();

            while (line == "")
            {
                line = reader.ReadLine();
            }

            return line;
        }

        private List<string> ReadUntilEmpty(StreamReader reader)
        {
            List<string> result = new List<string>();

            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                result.Add(line);
                line = reader.ReadLine();
            }

            return result;
        }
    }
}
