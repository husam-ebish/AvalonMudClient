using Argus.Extensions;
using Avalon.Common.Colors;
using Avalon.Common.Interfaces;
using ICSharpCode.AvalonEdit.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Avalon
{
    /// <summary>
    /// An immortal utility for restrings.
    /// </summary>
    public partial class LogCreatorWindow : Window
    {

        /// <summary>
        /// The text for the status bar.
        /// </summary>
        public string StatusText
        {
            get => TextBlockStatus.Text;
            set => TextBlockStatus.Text = value;
        }

        /// <summary>
        /// The text in the log editor.
        /// </summary>
        public string Text
        {
            get => LogEditor.Text;
            set => LogEditor.Text = value;
        }

        /// <summary>
        /// The default status bar color.
        /// </summary>
        private SolidColorBrush _defaultStatusColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#007ACC"));

        /// <summary>
        /// A reference to the clients interpreter.
        /// </summary>
        private IInterpreter _interp;

        /// <summary>
        /// A list of the AnsiColors so that we can remove them from the strings for easy viewing.
        /// </summary>
        private List<AnsiColor> _colors = AnsiColors.ToList();

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogCreatorWindow(IInterpreter interp)
        {
            InitializeComponent();
            _interp = interp;
            this.LogEditor.TextArea.TextView.Options.EnableEmailHyperlinks = false;
            this.LogEditor.TextArea.TextView.Options.EnableHyperlinks = false;
            this.LogEditor.TextArea.TextView.Options.ShowBoxForControlCharacters = false;

            // Get rid of the auto indenting.
            this.LogEditor.TextArea.IndentationStrategy = null;
        }

        /// <summary>
        /// Event that executes when the window is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogCreatorWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Code that is executed for the Cancel button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }

        /// <summary>
        /// Code that is executed for the Save button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {

            ClearError();
            this.Close();
        }

        /// <summary>
        /// Sets the form to error colors.
        /// </summary>
        /// <param name="text"></param>
        public void SetError(string text)
        {
            StatusText = text;
            this.BorderBrush = Brushes.Red;
            TextBlockStatus.Background = Brushes.Red;
            StatusBarWindow.Background = Brushes.Red;
        }

        /// <summary>
        /// Resets the color of the form and the text on the status bar to the default.
        /// </summary>
        public void ClearError()
        {
            StatusText = "";
            this.BorderBrush = _defaultStatusColor;
            TextBlockStatus.Background = _defaultStatusColor;
            StatusBarWindow.Background = _defaultStatusColor;
        }

        /// <summary>
        /// Removes all lines that start with a string pattern.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonRemoveLinesThatStartWith_Click(object sender, RoutedEventArgs e)
        {
            string text = await _interp.Conveyor.InputBox("Enter text:", "Remove Lines that Start With");

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = LogEditor.Text.Replace("\r", "").Split('\n');
            var sb = Argus.Memory.StringBuilderPool.Take();

            foreach (string line in lines)
            {
                if (!line.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(line);
                }
            }

            LogEditor.Text = sb.ToString();
            Argus.Memory.StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Removes all lines that end with a set of text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonRemoveLinesThatEndWith_Click(object sender, RoutedEventArgs e)
        {
            string text = await _interp.Conveyor.InputBox("Enter text:", "Remove Lines that End With");

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = LogEditor.Text.Replace("\r", "").Split('\n');
            var sb = Argus.Memory.StringBuilderPool.Take();

            foreach (string line in lines)
            {
                if (!line.EndsWith(text, StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(line);
                }
            }

            LogEditor.Text = sb.ToString();
            Argus.Memory.StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Removes all lines that contains a set of text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonRemoveLinesThatContain_Click(object sender, RoutedEventArgs e)
        {
            string text = await _interp.Conveyor.InputBox("Enter text:", "Remove Lines that Contain");

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = LogEditor.Text.Replace("\r", "").Split('\n');
            var sb = Argus.Memory.StringBuilderPool.Take();

            foreach (string line in lines)
            {
                if (!line.Contains(text, StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(line);
                }
            }

            LogEditor.Text = sb.ToString();
            Argus.Memory.StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Removes two blank lines in a row and replaces them with one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemoveDoubleBlankLines_Click(object sender, RoutedEventArgs e)
        {
            string text = LogEditor.Text;
            text = text.Replace("\r\n", "\n");
            bool containsPattern = text.Contains("\n\n");

            while (containsPattern)
            {
                text = text.Replace("\n\n", "\n");
                containsPattern = text.Contains("\n\n");
            }

            text = text.Replace("\n", "\r\n");            
            LogEditor.Text = text;
        }
    }
}
