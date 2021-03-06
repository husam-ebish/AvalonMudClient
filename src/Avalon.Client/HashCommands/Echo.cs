﻿using System.Collections.Generic;
using Avalon.Colors;
using Avalon.Common.Interfaces;
using CommandLine;

namespace Avalon.HashCommands
{

    /// <summary>
    /// Echos back the parameter the user sends.
    /// </summary>
    public class Echo : HashCommand
    {
        public Echo(IInterpreter interp) : base (interp)
        {
        }

        public override string Name { get; } = "#echo";

        public override string Description { get; } = "Echos the text to the terminal in the specified color.";

        public override void Execute()
        {
            // Parse the arguments and append to the file.
            var result = Parser.Default.ParseArguments<EchoArguments>(CreateArgs(this.Parameters))
                .WithParsed(o =>
                {
                    string text = o.Text;

                    var foregroundColor = Colorizer.ColorMapByName(o.Color);

                    if (foregroundColor != null)
                    {
                        Interpreter.EchoText(text, foregroundColor.AnsiColor, o.Reverse);
                    }
                    else
                    {
                        Interpreter.EchoText($"{text}");
                    }
                });

            // Display the help or error output from the parameter parsing.
            this.DisplayParserOutput(result);
        }

        /// <summary>
        /// The supported command line arguments for this application.
        /// </summary>
        public class EchoArguments
        {
            [Option('c', "color", Required = false, HelpText = "The name of the supported color the echo should rendered as.")]
            public string Color { get; set; }

            /// <summary>
            /// Returns the rest of the values that weren't parsed by switches into a list of strings.
            /// </summary>
            [Value(0, Min = 1, HelpText = "The text to echo to the terminal window.")]
            public IEnumerable<string> TextList { get; set; }

            /// <summary>
            /// Returns the left over values as a single string that doesn't have to be escaped by quotes.
            /// </summary>
            public string Text => string.Join(" ", TextList);

            [Option('r', "reverse", Required = false, HelpText = "Whether or not to reverse the colors.  Only works when a valid color is specified.")]
            public bool Reverse { get; set; } = false;

        }

    }
}
