using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Argus.Extensions;

namespace Argus.Data
{
    /// <summary>
    /// A class to manage a list of lines who require operations to be performed on those lines.  The lines are kept interally
    /// and removed or updated.  When ToString or ToStringBuilder are called the outputted text is rebuilt from
    /// what is left in the Lines list.  The lines are parsed when initially passed in and stored in the <see cref="Lines"/>
    /// property against which all operations will occur.
    /// </summary>
    /// <remarks>
    /// It is important to note that this deals with use cases where operations are dealt against individual lines and multiline
    /// operations are mostly not supported.
    /// </remarks>
    public class LineManager
    {
        //*********************************************************************************************************************
        //
        //             Class:  LineManager
        //      Organization:  http://www.blakepell.com
        //      Initial Date:  04/26/2020
        //      Last Updated:  06/26/2020
        //     Programmer(s):  Blake Pell, blakepell@hotmail.com
        //
        //*********************************************************************************************************************

        public LineManager(string text)
        {
            this.Lines = text.Replace("\r", "").Split('\n').ToList();
        }

        /// <summary>
        /// Removes a line if it starts with the supplied pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="compareType"></param>
        public void RemoveIfStartsWith(string pattern, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (this.Lines[i].StartsWith(pattern, compareType))
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if it starts with any of the supplied patterns.
        /// </summary>
        /// <param name="patterns"></param>
        /// <param name="compareType"></param>
        public void RemoveIfStartsWith(string[] patterns, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                foreach (string pattern in patterns)
                {
                    if (this.Lines[i].StartsWith(pattern, compareType))
                    {
                        // If the match is found, remove it, then break out of the inner loop.
                        this.Lines.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a line if it starts with the supplied pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="compareType"></param>
        public void RemoveIfEndsWith(string pattern, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (this.Lines[i].EndsWith(pattern, compareType))
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if it ends with any of the supplied patterns.
        /// </summary>
        /// <param name="patterns"></param>
        /// <param name="compareType"></param>
        public void RemoveIfEndsWith(string[] patterns, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                foreach (string pattern in patterns)
                {
                    if (this.Lines[i].EndsWith(pattern, compareType))
                    {
                        // If the match is found, remove it, then break out of the inner loop.
                        this.Lines.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a line if it starts with the supplied pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="compareType"></param>
        public void RemoveIfContains(string pattern, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                // Meh, Compare doesn't support StringComparison until .NET Standard 2.1
                if (this.Lines[i].IndexOf(pattern, 0, compareType) >= 0)
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if it contains with any of the supplied patterns.
        /// </summary>
        /// <param name="patterns"></param>
        /// <param name="compareType"></param>
        public void RemoveIfContains(string[] patterns, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                foreach (string pattern in patterns)
                {
                    if (this.Lines[i].IndexOf(pattern, 0, compareType) >= 0)
                    {
                        // If the match is found, remove it, then break out of the inner loop.
                        this.Lines.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a line if it equals the supplied pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="compareType"></param>
        public void RemoveIfEquals(string pattern, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (this.Lines[i].Equals(pattern, compareType))
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if it equals with any of the supplied patterns.
        /// </summary>
        /// <param name="patterns"></param>
        /// <param name="compareType"></param>
        public void RemoveIfEquals(string[] patterns, StringComparison compareType = StringComparison.Ordinal)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                foreach (string pattern in patterns)
                {
                    if (this.Lines[i].Equals(pattern, compareType))
                    {
                        // If the match is found, remove it, then break out of the inner loop.
                        this.Lines.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a line if it meets the Regex pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="options">Regular Expression Options: Default is SingleLine only.</param>
        public void RemoveIfRegex(string pattern, RegexOptions options = RegexOptions.Singleline)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (Regex.IsMatch(this.Lines[i], pattern, options))
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if it matches any of the supplied Regex patterns.
        /// </summary>
        /// <param name="patterns"></param>
        /// <param name="options">Regular Expression Options: Default is SingleLine only.</param>
        public void RemoveIfRegex(string[] patterns, RegexOptions options = RegexOptions.Singleline)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                foreach (string pattern in patterns)
                {
                    if (Regex.IsMatch(this.Lines[i], pattern, options))
                    {
                        this.Lines.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a line if it is null or whitespace.
        /// </summary>
        public void RemoveIfNullOrWhiteSpace()
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(this.Lines[i]))
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if it is null or whitespace.
        /// </summary>
        public void RemoveIfIsNullOrEmpty()
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(this.Lines[i]))
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if its word count exceeds the specified value.
        /// </summary>
        /// <param name="words"></param>
        public void RemoveIfWordCountEquals(int words)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (WordCount(this.Lines[i]) == words)
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if its word count exceeds the specified value.
        /// </summary>
        /// <param name="words"></param>
        public void RemoveIfWordCountGreaterThan(int words)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (WordCount(this.Lines[i]) > words)
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes a line if its word count is less than the specified value.
        /// </summary>
        /// <param name="words"></param>
        public void RemoveIfWordCountLessThan(int words)
        {
            for (int i = this.Lines.Count - 1; i >= 0; i--)
            {
                if (WordCount(this.Lines[i]) < words)
                {
                    this.Lines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes duplicate lines keeping the first instance.
        /// </summary>
        public void RemoveDuplicateLines()
        {
            this.Lines = (List<string>)this.Lines.Distinct();
        }

        /// <summary>
        /// Replaces all occurances of a string with another string in the <see cref="Lines"/> list.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replaceWith"></param>
        public void Replace(string pattern, string replaceWith)
        {
            for (int i = 0; i < this.Lines.Count - 1; i++)
            {
                this.Lines[i] = this.Lines[i].Replace(pattern, replaceWith);
            }
        }

        /// <summary>
        /// Replaces all occurances of a char with another char in the <see cref="Lines"/> list.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="replaceWith"></param>
        public void Replace(char c, char replaceWith)
        {
            for (int i = 0; i < this.Lines.Count - 1; i++)
            {
                this.Lines[i] = this.Lines[i].Replace(c, replaceWith);
            }
        }

        /// <summary>
        /// Replaces a regular expression match with the provided string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replaceWith"></param>
        public void ReplaceRegex(string pattern, string replaceWith)
        {
            for (int i = 0; i < this.Lines.Count - 1; i++)
            {
                this.Lines[i] = Regex.Replace(this.Lines[i], pattern, replaceWith);
            }
        }

        /// <summary>
        /// Appends the provided text to each line.
        /// </summary>
        /// <param name="text"></param>
        public void Append(string text)
        {
            for (int i = 0; i < this.Lines.Count - 1; i++)
            {
                this.Lines[i] = $"{this.Lines[i]}{text}";
            }
        }

        /// <summary>
        /// Prepends the provided text to each line.
        /// </summary>
        /// <param name="text"></param>
        public void Prepend(string text)
        {
            for (int i = 0; i < this.Lines.Count - 1; i++)
            {
                this.Lines[i] = $"{text}{this.Lines[i]}";
            }
        }

        /// <summary>
        /// Wraps each line with the specified before and after text.
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public void Wrap(string before, string after)
        {
            for (int i = 0; i < this.Lines.Count - 1; i++)
            {
                this.Lines[i] = $"{before}{this.Lines[i]}{after}";
            }
        }

        /// <summary>
        /// Builds the <see cref="StringBuilder"/> from the contents of <see cref="Lines"/>.
        /// </summary>
        private void BuildOutput()
        {
            _sb.Clear();

            foreach (string line in this.Lines)
            {
                _sb.AppendLine(line);
            }
        }

        /// <summary>
        /// Returns the word count in the current string accounting for whitespace.
        /// </summary>
        /// <param name="text"></param>
        private int WordCount(string text)
        {
            int wordCount = 0;
            int index = 0;

            // Skip whitespace until first word.
            while (index < text.Length && char.IsWhiteSpace(text[index]))
            {
                index++;
            }

            while (index < text.Length)
            {
                // Check if current char is part of a word.
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                {
                    index++;
                }

                wordCount++;

                // Skip whitespace until next word.
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                {
                    index++;
                }
            }

            return wordCount;
        }

        /// <summary>
        /// Returns a string built from the current contents of <see cref="Lines"/>.
        /// </summary>
        public override string ToString()
        {
            this.BuildOutput();
            return _sb.ToString();
        }

        /// <summary>
        /// Returns a StringBuilder with the filtered output.
        /// </summary>
        public StringBuilder ToStringBuilder()
        {
            this.BuildOutput();
            return _sb;
        }

        /// <summary>
        /// A single internal StringBuilder used to process and return the output.
        /// </summary>
        private StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// The list of individual lines that we will filter down.
        /// </summary>
        public List<string> Lines { get; set; } = new List<string>();

    }
}
