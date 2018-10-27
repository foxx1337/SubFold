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

        private SrtSubtitle()
        {
            Lines = new List<Line>();
        }

        public SrtSubtitle(string subtitle) : this()
        {
            using (StringReader reader = new StringReader(subtitle))
            {
                FromReader(reader);
            }
        }

        public SrtSubtitle(FileInfo inputFile) : this()
        {
            using (StreamReader reader = new StreamReader(inputFile.FullName))
            {
                FromReader(reader);
            }
        }

        private void FromReader(TextReader readerInput)
        {
            while (true)
            {
                Line line = ReadSubtitleLine(readerInput);
                if (line == null)
                {
                    break;
                }

                Lines.Add(line);
            }
        }

        private Line ReadSubtitleLine(TextReader reader)
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

        private string ReadFirstNonEmpty(TextReader reader)
        {
            string line = reader.ReadLine();

            while (line == "")
            {
                line = reader.ReadLine();
            }

            return line;
        }

        private List<string> ReadUntilEmpty(TextReader reader)
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
