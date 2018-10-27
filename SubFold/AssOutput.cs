using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;

namespace SubFold
{
    /// <summary>
    /// Don't laugh, models SSA / ASS subtitles as documented by
    /// https://www.matroska.org/technical/specs/subtitles/ssa.html
    /// </summary>
    public class AssOutput
    {
        private const string AssFormat = @"h\:mm\:ss\,ff";

        public string Title { get; set; }

        public Mode3D Mode { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string FontName { get; set; }

        public int FontSize { get; set; }

        /// <summary>
        /// Color with transparency for web - for example 00FFFFFF
        /// </summary>
        public string FontColor { get; set; }

        private readonly Subtitle _subtitle;

        public AssOutput(Subtitle subtitle, String title = "")
        {
            Title = title;
            Mode = Mode3D.Sbs;
            Width = 384;
            Height = 288;
            FontName = "Calibri";
            FontSize = 26;
            FontColor = "00FFFFFF";

            _subtitle = subtitle;
        }

        public void Render(StreamWriter writer)
        {
            RenderInfo(writer);
            RenderStyles(writer);
            RenderEvents(writer);
        }

        private void RenderInfo(StreamWriter writer)
        {
            writer.WriteLine("[Script Info]");

            writer.WriteLine($"Title: {Title}");
            writer.WriteLine("ScriptType: v4.00+");
            writer.WriteLine("Collisions: Normal");
            writer.WriteLine($"PlayResX: {Width}");
            writer.WriteLine($"PlayResY: {Height}");
            writer.WriteLine("PlayDepth: 0");
            writer.WriteLine("Timer: 100");
            writer.WriteLine("WrapStyle: 0");

            writer.WriteLine();
        }

        private void RenderStyles(StreamWriter writer)
        {
            writer.WriteLine("[v4+ Styles]");
            writer.WriteLine(
                "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding");
            writer.WriteLine(
                $"Style: Default, {FontName}, {FontSize}, &H{FontColor}, &H00000000, &H00000000, &H00000000, 0, 0, 0, 0, 50, 100, 0, 0, 1, 1, 0, 2, 0, 0, 15, 0");

            writer.WriteLine();
        }

        private void RenderEvents(StreamWriter writer)
        {
            writer.WriteLine("[Events]");

            writer.WriteLine("Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text");

            foreach (Line line in _subtitle.Lines)
            {
                string assVersion = line.Text.Select(Ass_ize).Aggregate((accu, currentItem) => accu + @"\n" + currentItem);
                writer.WriteLine($"Dialogue: 0,{line.Start.ToString(AssFormat)},{line.Stop.ToString(AssFormat)},Default,,0000,{Width / 2},0000,{assVersion}");
                writer.WriteLine($"Dialogue: 0,{line.Start.ToString(AssFormat)},{line.Stop.ToString(AssFormat)},Default,,{Width / 2},0000,0000,{assVersion}");
            }
        }

        private string Ass_ize(string textLine)
        {
            // This is a very sad way of parsing a line of text (with multiple
            // regexes). I'm sorry and I'm warned, will go to my corner now.
            string processed = Regex.Replace(textLine, @"<(?<tag>.)>", @"{\\k<tag>1}");
            processed = Regex.Replace(processed, @"</(?<tag>.)>", @"{\\k<tag>0}");
            return processed;
        }
    }
}
