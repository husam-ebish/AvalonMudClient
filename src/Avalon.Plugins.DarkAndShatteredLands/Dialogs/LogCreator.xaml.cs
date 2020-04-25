using Argus.Extensions;
using Avalon.Common.Colors;
using Avalon.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
            set
            {
                value = RemovePassword(value);
                LogEditor.Text = value;
            }
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

            // Set the font from the client settings.
            this.LogEditor.FontSize = _interp.Conveyor.ClientSettings.TerminalFontSize;
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
            SetIdle();
            this.Close();
        }

        /// <summary>
        /// Code that is executed for the Save button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            SetProcessing("Status: Please choose where you would like to save your file.");

            var sfd = new SaveFileDialog
            {
                Title = "Save Log File",
                ValidateNames = true,
                AutoUpgradeEnabled = true,
            };

            sfd.ShowDialog();

            if (string.IsNullOrWhiteSpace(sfd.FileName))
            {
                SetStatus("Status: Log Save Cancelled.");
                return;
            }

            try
            {
                SetProcessing($"Status: Saving to {sfd.FileName}");
                System.IO.File.WriteAllText(sfd.FileName, LogEditor.Text, Encoding.ASCII);
                this.Close();
            }
            catch (Exception ex)
            {
                // On error show them the error but don't close the log.
                SetError($"Error: {ex.Message}");
                return;
            }

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
        /// Settings the window as processing.
        /// </summary>
        /// <param name="text"></param>
        public void SetProcessing(string text)
        {
            StatusText = text;
            this.BorderBrush = Brushes.Orange;
            TextBlockStatus.Background = Brushes.Orange;
            StatusBarWindow.Background = Brushes.Orange;
        }

        /// <summary>
        /// Resets the color of the form and the text on the status bar to the default.
        /// </summary>
        public void SetIdle()
        {
            StatusText = "Status: Idle";
            this.BorderBrush = _defaultStatusColor;
            TextBlockStatus.Background = _defaultStatusColor;
            StatusBarWindow.Background = _defaultStatusColor;
        }

        /// <summary>
        /// Resets the color of the form and the text on the status bar to the default.
        /// </summary>
        public void SetStatus(string text)
        {
            StatusText = $"Status: {text}";
            this.BorderBrush = _defaultStatusColor;
            TextBlockStatus.Background = _defaultStatusColor;
            StatusBarWindow.Background = _defaultStatusColor;
        }

        /// <summary>
        /// Gets the lines in the text editor as an array and clears the shared StringBuilder array.
        /// </summary>
        public string[] GetLines()
        {
            return LogEditor.Text.Replace("\r", "").Split('\n');
        }

        /// <summary>
        /// Shared button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonHandler_Click(object sender, RoutedEventArgs e)
        {
            var fe = e.Source as FrameworkElement;
            string desc = fe.Tag as string;

            SetProcessing($"Status: Executing {desc}");

            try
            {
                switch (desc)
                {
                    case "Remove Prompts":
                        RemovePrompts();
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Channels":
                        RemoveChannels();
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Toasts":
                        RemoveToasts();
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Double Blank Lines":
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Lines that Start With":
                        await RemoveLinesThatStartWith("");
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Lines that End With":
                        RemoveLinesThatEndWith();
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Lines that Contain":
                        RemoveLinesThatContain();
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Battle":
                        RemoveBattle();
                        RemoveDoubleBlankLines();
                        break;
                    case "Remove Directions":
                        RemoveDirections();
                        RemoveDoubleBlankLines();
                        break;
                    case "RP Log":
                        CreateRpLog();
                        break;
                    case "Remove Maccus":
                        RemoveLinesContaining("Maccus");
                        break;
                    default:
                        SetError($"Status: {desc} was not found.");
                        return;
                }


            }
            catch (Exception ex)
            {
                SetError($"Status: Error - {ex.Message}");
                return;
            }

            SetStatus($"Status: {desc} completed.");
            e.Handled = true;
        }

        /// <summary>
        /// Removes all lines that contain the phrase (case insenstive).  This should be used for single action
        /// filters since a loop over all the lines will occur (which is inefficient for multiple action filters).
        /// </summary>
        private void RemoveLinesContaining(string text)
        {
            var sb = Argus.Memory.StringBuilderPool.Take();

            try
            {
                var lines = GetLines();

                foreach (string line in lines)
                {
                    if (!line.Contains(text, StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine(line);
                    }
                }

                LogEditor.Text = sb.ToString().Trim();
            }
            catch (Exception ex)
            {
                SetError($"Error: {ex.Message}");
            }
            finally
            {
                Argus.Memory.StringBuilderPool.Return(sb);
            }
        }

        /// <summary>
        /// Removes prompts from the output.
        /// </summary>
        public void RemovePrompts()
        {
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"\<(\d+)/(\d+)hp (\d+)/(\d+)m (\d+)/(\d+)mv \((\d+)\|(\w+)\) \((.*?)\) \((.*?)\) (.*?) (.*?)\>", "", RegexOptions.Multiline);
        }

        /// <summary>
        /// Removes all channels.
        /// </summary>
        private void RemoveChannels()
        {
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"^[\a]?(\[ .* \] )?([\w'-]+|The ghost of [\w'-]+|\(An Imm\)|\(Imm\) [\w'-]+|\(Wizi@\d\d\) \(Imm\) [\w'-]+) (\bclan gossip(s?)\b|\bclan(s?)\b|\bgossip(s?)\b|\bask(s?)\b|\banswers(s?)\b|\btell(s?)\b|\bBloodbath(s?)\b|\bpray(s?)\b|\bgrats\b|\bauction(s?)\b|\bquest(s?)\b|\bradio(s?)\b|\bimm(s?)\b).*'", "", RegexOptions.Multiline);
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"^[\a]?(\[ .* \] )?(?!.*OOC).*Kingdom: .*", "", RegexOptions.Multiline);
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"\((Admin|Coder)\) \(Imm\) [\w'-]+:", "", RegexOptions.Multiline);
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"^[\a]?(\(.*\)?)?([\w'-]+|The ghost of [\w'-]+|\(An Imm\)|\(Imm\) [\w'-]+) (OOC|\[Newbie\]).*", "", RegexOptions.Multiline);
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"^\((Shalonesti|OOC Shalonesti|Clave|OOC Clave)\).*", "", RegexOptions.Multiline);
        }

        private async Task RemoveLinesThatStartWith()
        {
            await RemoveLinesThatStartWith("");
        }

        /// <summary>
        /// Removes all lines that start with a string pattern.
        /// </summary>
        private async Task RemoveLinesThatStartWith(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                await _interp.Conveyor.InputBox("Enter text:", "Remove Lines that Start With");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = GetLines();
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

        private async Task RemoveLinesThatEndWith()
        {
            await RemoveLinesThatEndWith("");
        }

        /// <summary>
        /// Removes all lines that end with a set of text.
        /// </summary>
        private async Task RemoveLinesThatEndWith(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = await _interp.Conveyor.InputBox("Enter text:", "Remove Lines that End With");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = GetLines();
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

        private async Task RemoveLinesThatContain()
        {
            await RemoveLinesThatContain("");
        }

        /// <summary>
        /// Removes all lines that contains a set of text.
        /// </summary>
        private async Task RemoveLinesThatContain(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = await _interp.Conveyor.InputBox("Enter text:", "Remove Lines that Contain");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = GetLines();
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
        private void RemoveDoubleBlankLines()
        {
            string text = LogEditor.Text;
            text = text.Replace("\r\n", "\n");
            bool containsPattern = text.Contains("\n\n\n");

            while (containsPattern)
            {
                text = text.Replace("\n\n\n", "\n\n");
                containsPattern = text.Contains("\n\n\n");
            }

            text = text.Replace("\n", "\r\n");
            LogEditor.Text = text;
        }

        private void RemoveDirections()
        {
            var lines = GetLines();
            var sb = Argus.Memory.StringBuilderPool.Take();
            var list = new List<string>();

            list.Add("u");
            list.Add("up");
            list.Add("d");
            list.Add("down");
            list.Add("n");
            list.Add("north");
            list.Add("s");
            list.Add("south");
            list.Add("e");
            list.Add("east");
            list.Add("w");
            list.Add("west");
            list.Add("northeast");
            list.Add("ne");
            list.Add("northwest");
            list.Add("nw");
            list.Add("southeast");
            list.Add("se");
            list.Add("southwest");
            list.Add("southwest");
            list.Add("Alas, you cannot go that way.");

            foreach (string line in lines)
            {
                bool found = false;

                // Verbatim filters
                foreach (string filter in list)
                {
                    if (string.Equals(line, filter, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                sb.AppendLine(line);
            }

            LogEditor.Text = sb.ToString();
            Argus.Memory.StringBuilderPool.Return(sb);

        }

        /// <summary>
        /// Remove toasts from the output.
        /// </summary>
        private void RemoveToasts()
        {
            LogEditor.Text = Regex.Replace(LogEditor.Text, @"^[\a]?([\[\(](.*?)[\]\)])?[ ]{0,}([\w'-]+) got (.*?) by (.*?) ([\[\(] (.*?) [\]\)])?[ ]{0,}([\(]Arena[\)])?", "", RegexOptions.Multiline);
        }

        /// <summary>
        /// Removes battle echos.
        /// </summary>
        private void RemoveBattle()
        {
            var lines = GetLines();
            var sb = Argus.Memory.StringBuilderPool.Take();
            var filterList = new List<string>();

            filterList.Add("You dodge");
            filterList.Add("You parry");
            filterList.Add("dodges your attack");
            filterList.Add("parries your attack");
            filterList.Add("draws life from");
            filterList.Add("draws energy from");
            filterList.Add("is knocked to the ground");
            filterList.Add("fumes and dissolves");
            filterList.Add("is pitted and etched");
            filterList.Add("is corroded into scrap");
            filterList.Add("corrodes and breaks");
            filterList.Add("is burned into waste");
            filterList.Add("turns blue and shivers");
            filterList.Add("freezes and shatters");
            filterList.Add("is blinded by smoke");
            filterList.Add("Your eyes tear up from smoke");
            filterList.Add("ignites and burns!");
            filterList.Add("bubbles and boils!");
            filterList.Add("crackles and burns!");
            filterList.Add("smokes and chars!");
            filterList.Add("sparks and sputters!");
            filterList.Add("blackens and crisps!");
            filterList.Add("melts and drips!");
            filterList.Add("You feel poison coursing through your veins.");
            filterList.Add("looks very ill.");
            filterList.Add("Your muscles stop responding.");
            filterList.Add("overloads and explodes.");
            filterList.Add("is fused into a worthless lump.");
            filterList.Add("is blinded by the intense heat");
            filterList.Add("You shut your eyes to prevent them from melting");
            filterList.Add("is exhausted by the extreme heat");
            filterList.Add("You are exhausted from the extreme heat");
            filterList.Add("You feel so very tired and sleepy");
            filterList.Add("falls to the floor in a deep sleep.");
            filterList.Add("You feel slow and lethargic");
            filterList.Add("begins to move very slowly.");
            filterList.Add("You are paralyzed");
            filterList.Add("windpipe right out of his throat");
            filterList.Add("is DEAD!!");
            filterList.Add("You have almost completed your QUEST!");
            filterList.Add("Return to the questmaster before your time runs out!");
            filterList.Add("experience points.");
            filterList.Add("coins from the corpse");
            filterList.Add("coins for your sacrifice");
            filterList.Add("death cry.");

            foreach (string line in lines)
            {
                bool found = false;

                // Damage by me to someone else
                if (Regex.IsMatch(line, @"^(You|Your)(\s)?(.*?)?(miss(es)?|scratch(es)?|graze(s)?|hit(s)?|injure(s)?|wound(s)?|maul(s)?|decimate(s)?|devastate(s)?|maim(s)?|MUTILATE(S)?|DISEMBOWEL(S)?|DISMEMBER(S)?|MASSACRE(S)?|MANGLE(S)?|\*\*\* DEMOLISH(ES)? \*\*\*|\*\*\* DEVASTATES \*\*\*|=== OBLITERATE(S)? ===|>>> ANNIHILATE(S)? <<<|<<< ERADICATE(S)? >>>|(does|do) HIDEOUS things to|(does|do) GHASTLY things to|(does|do) UNSPEAKABLE things to) (.*?)(\.|\!)"))
                {
                    continue;
                }

                // Damage by someone else to me
                if (Regex.IsMatch(line, @"^(.*?)?(miss(es)?|scratch(es)?|graze(s)?|hit(s)?|injure(s)?|wound(s)?|maul(s)?|decimate(s)?|devastate(s)?|maim(s)?|MUTILATE(S)?|DISEMBOWEL(S)?|DISMEMBER(S)?|MASSACRE(S)?|MANGLE(S)?|\*\*\* DEMOLISH(ES)? \*\*\*|\*\*\* DEVASTATES \*\*\*|=== OBLITERATE(S)? ===|>>> ANNIHILATE(S)? <<<|<<< ERADICATE(S)? >>>|(does|do) HIDEOUS things to|(does|do) GHASTLY things to|(does|do) UNSPEAKABLE things to) (.*?)(\.|\!)"))
                {
                    continue;
                }

                // Condition
                if (Regex.IsMatch(line, @"^(.*?)? (is in excellent condition|has a few scratches|has some small wounds and bruises|has quite a few wounds|has some big nasty wounds and scratches|looks pretty hurt|is in awful condition|is bleeding to death)."))
                {
                    continue;
                }

                // Verbatim filters
                foreach (string filter in filterList)
                {
                    if (line.Contains(filter))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                sb.AppendLine(line);
            }

            LogEditor.Text = sb.ToString();
            Argus.Memory.StringBuilderPool.Return(sb);
        }

        /// <summary>
        /// Removes the password line from the text provided.
        /// </summary>
        private string RemovePassword(string text)
        {
            var sb = Argus.Memory.StringBuilderPool.Take();

            try
            {
                var lines = text.Replace("\r", "").Split('\n');

                foreach (string line in lines)
                {
                    if (!line.Contains("Password:", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine(line);
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                SetError($"Error: {ex.Message}");
                return text;
            }
            finally
            {
                Argus.Memory.StringBuilderPool.Return(sb);
            }
        }

        private void RemoveSingleWordLines()
        {
            var sb = Argus.Memory.StringBuilderPool.Take();

            try
            {
                foreach (string line in LogEditor.Text.Replace("\r", "").Split('\n'))
                {
                    if (line.ToWords().Count(x => !string.IsNullOrWhiteSpace(x)) == 1)
                    {
                        continue;
                    }

                    sb.AppendLine(line);
                }

                LogEditor.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                SetError($"Error: {ex.Message}");
            }
            finally
            {
                Argus.Memory.StringBuilderPool.Return(sb);
            }
        }

        private async Task RemoveSpells()
        {
            await RemoveLinesThatStartWith("c ");
            await RemoveLinesThatStartWith("cast ");
        }

        private async Task RemoveScore()
        {
            await RemoveLinesThatStartWith(@"Score for ");
            await RemoveLinesThatStartWith(@"----------------------------------------------------------------------------");
            await RemoveLinesThatStartWith(@"LEVEL: ");
            await RemoveLinesThatStartWith(@"YEARS: ");
            await RemoveLinesThatStartWith(@"SEX  : ");
            await RemoveLinesThatStartWith(@"STR  : ");
            await RemoveLinesThatStartWith(@"INT  : ");
            await RemoveLinesThatStartWith(@"WIS  : ");
            await RemoveLinesThatStartWith(@"DEX  : ");
            await RemoveLinesThatStartWith(@"CON  : ");
            await RemoveLinesThatStartWith(@"THA  :             Wimpy: ");
            await RemoveLinesThatStartWith(@"                   Wimpy: ");
            await RemoveLinesThatStartWith(@"BANK : ");
            await RemoveLinesThatStartWith(@"GOLD : ");
            await RemoveLinesThatStartWith(@"PRACT: ");
            await RemoveLinesThatStartWith(@"TRAIN: ");
            await RemoveLinesThatStartWith(@"XP   : ");
            await RemoveLinesThatStartWith(@"PK Trains: ");
            await RemoveLinesThatStartWith(@"Next PK loot change at: ");
            await RemoveLinesThatStartWith(@"                                              256 Color");
            await RemoveLinesThatStartWith(@"Speaking: ");
            await RemoveLinesThatStartWith(@"Remaining Hostile Time: ");
            await RemoveLinesThatStartWith(@"Religion: ");
            await RemoveLinesThatStartWith(@"PKill: ");
            await RemoveLinesThatStartWith(@"PK Points: ");
            await RemoveLinesThatStartWith(@"You feel great.");
            await RemoveLinesThatStartWith("You are a light sleeper.");
            await RemoveLinesThatStartWith("You are exceptionally calm in combat.");
            await RemoveLinesThatStartWith("You are exceptionally intelligent.");
            await RemoveLinesThatStartWith("You have a good knowledge of weaponry.");
            await RemoveLinesThatStartWith("You have an affinity to magic.");
            await RemoveLinesThatStartWith("You are resistant to magic.");
            await RemoveLinesThatStartWith("You are exceptionally agile.");
            await RemoveLinesThatStartWith("You are large for your race.");
            await RemoveLinesThatStartWith("You have a deep perception of beings.");
            await RemoveLinesThatStartWith("You suffer from horrible nightmares.");
            await RemoveLinesThatStartWith("You are completely insane.");
            await RemoveLinesThatStartWith("You have an obsessive hatred for");
            await RemoveLinesThatStartWith("You are deaf.");
            await RemoveLinesThatStartWith("You are vulnerable to magic.");
            await RemoveLinesThatStartWith("You are illiterate.");
            await RemoveLinesThatStartWith("You lost an arm in a childhood accident.");
            await RemoveLinesThatStartWith("You cannot speak.");
            await RemoveLinesThatStartWith("You suffer from a nervous tic.");
            await RemoveLinesThatStartWith("You are exceptionally weak and frail.");
            await RemoveLinesThatStartWith(@"----------------------------------------------------------------------------");
            await RemoveLinesThatStartWith(@"PROFESSION: Son of a God");
            await RemoveLinesThatStartWith(@"----------------------------------------------------------------------------");
        }

        public async Task RemoveHeader()
        {
            await RemoveLinesThatContain("\a");
            await RemoveLinesThatStartWith(@"[ ");
            await RemoveLinesThatStartWith("Do you want color");
            await RemoveLinesThatStartWith("Original DikuMUD by: Hans Staerfelt, Katja Nyboe, Tom Madsen, Michael");
            await RemoveLinesThatStartWith("Seifert, Sebastian Hammer.");
            await RemoveLinesThatStartWith("Original MERC 2.1 by Hatchet, Furey, and Kahn.");
            await RemoveLinesThatStartWith("DSL Owned by Allen Games. (which is owned by Scorn)");
            await RemoveLinesThatStartWith("DSL Web Site: http://www.dsl-mud.org");
            await RemoveLinesThatStartWith(@"Various Snippets from SMAUG");
            await RemoveLinesThatStartWith(@"                                   /   \");
            await RemoveLinesThatStartWith(" _                         )      ((   ))     (");
            await RemoveLinesThatStartWith(@"(@)                       /|\      ))_((     /|\                         _");
            await RemoveLinesThatStartWith(@"|-|`\                    / | \    (/\|/\)   / | \                       (@)");
            await RemoveLinesThatStartWith(@"| |---------------------/--|-voV---\`|'/--Vov-|--\----------------------|-|");
            await RemoveLinesThatStartWith(@"|-|                          '^`   (o o)  '^`                           | |");
            await RemoveLinesThatStartWith(@"| |                                `\Y/'                                |-|");
            await RemoveLinesThatStartWith(@"|-|                                                                     | |");
            await RemoveLinesThatStartWith(@"|-|                    DARK & SHATTERED LANDS (DSL)                     | |");
            await RemoveLinesThatStartWith(@"| |                                                                     |-|");
            await RemoveLinesThatStartWith(@"|-|                           [Implementor]                             | |");
            await RemoveLinesThatStartWith(@"| |                               Scorn                                 |-|");
            await RemoveLinesThatStartWith(@"|-|                        (scorn@dsl-mud.org)                          | |");
            await RemoveLinesThatStartWith(@"|_|_____________________________________________________________________|-|");
            await RemoveLinesThatStartWith(@"(@)                l   /\ /         ( (       \ /\   l                `\|_|");
            await RemoveLinesThatStartWith(@"                   l /   V           \ \       V   \ l                  (@)");
            await RemoveLinesThatStartWith(@"                   l/                _) )_          \I");
            await RemoveLinesThatStartWith(@"                                     `\ /'                         ");
            await RemoveLinesThatStartWith(@"                                       ,");
            await RemoveLinesThatStartWith(@"             Code:  DSL ");
            await RemoveLinesThatStartWith(@"           Based on ROM ");
            await RemoveLinesThatStartWith(@"[DSL] (Push Enter to Continue)");
            await RemoveLinesThatStartWith(@" ------===+*<==(  Dark and Shattered Lands: Main Login Menu )==>*+===------");
            await RemoveLinesThatStartWith(@"    (C)reate a New Character");
            await RemoveLinesThatStartWith(@"    (L)imited Race Creation");
            await RemoveLinesThatStartWith(@"    (P)lay Existing Character");
            await RemoveLinesThatStartWith(@"    (M)aster Account Login     ");
            await RemoveLinesThatStartWith(@"    (F)orm a New Master Account");
            await RemoveLinesThatStartWith(@"    (W)ho is on now?");
            await RemoveLinesThatStartWith(@"    (H)elpfiles");
            await RemoveLinesThatStartWith(@"    (Q)uit");
            await RemoveLinesThatStartWith(@"    Your selection? ->");
            await RemoveLinesThatStartWith(@"(Existing Master Account)");
            await RemoveLinesThatStartWith(@"What is your Master Account's name?");
            await RemoveLinesThatStartWith(@"Password:");
            await RemoveLinesThatStartWith(@" ------===+*<==(  Dark and Shattered Lands: Master Login Menu )==>*+===------");
            await RemoveLinesThatStartWith(@"Master account:");
            await RemoveLinesThatStartWith(@"    (C)reate a New Character");
            await RemoveLinesThatStartWith(@"    (V)iew Characters and Personal information");
            await RemoveLinesThatStartWith(@"    (M)aster Account Password Change");
            await RemoveLinesThatStartWith(@"    (E)mail and Personal Information Change");
            await RemoveLinesThatStartWith(@"    (A)dd existing Character to Master  System Time");
            await RemoveLinesThatStartWith(@"    (R)ewards menu");
            await RemoveLinesThatStartWith(@"    (Q)uit to Main Menu");
            await RemoveLinesThatStartWith(@" ------===+*<==(  Dark and Shattered Lands: Master Character Logon )==>*+===------");
            await RemoveLinesThatStartWith(@"   You can only log on a character attached to this master account.");
            await RemoveLinesThatStartWith(@"   To see that list hit enter then from the main menu hit: ");
            await RemoveLinesThatStartWith(@"   (V) to View Characters and Personal information.");
            await RemoveLinesThatStartWith(@"Player name:");
            await RemoveLinesThatStartWith(@"*");
            await RemoveLinesThatStartWith("WELCOME TO DARK & SHATTERED LANDS");
            await RemoveLinesThatStartWith("-");
            await RemoveLinesThatStartWith("When approaching a Red Dragon, be sure to bring your wand of marshmallow.");
            await RemoveLinesThatStartWith(@"Welcome to DSL! DSL Loves You! Other muds think you are ugly, they said so!  ");

            await RemoveLinesThatStartWith(@"whoami");
            await RemoveLinesThatStartWith(@"score");
            await RemoveLinesThatStartWith(@"prompt ");
            await RemoveLinesThatStartWith(@"score");
            await RemoveLinesThatStartWith(@"You are currently improving ");
            await RemoveLinesThatStartWith(@"Syntax: improve <skillname> / improve none");

        }

        private async Task RemoveUnreadNotes()
        {
            await RemoveLinesThatContain("new news article waiting.");
            await RemoveLinesThatContain("changes waiting to be read.");
            await RemoveLinesThatContain("change waiting to be read.");
            await RemoveLinesThatContain("new ooc note waiting");
            await RemoveLinesThatContain("new ooc notes waiting");
            await RemoveLinesThatContain("new notes waiting.");
            await RemoveLinesThatContain("new note waiting.");
            await RemoveLinesThatContain("new quest note waiting");
            await RemoveLinesThatContain("new quest notes waiting");
            await RemoveLinesThatContain("unread auctions.");
            await RemoveLinesThatContain("unread auction.");
            await RemoveLinesThatContain("story notes have been added.");
            await RemoveLinesThatContain("story note have been added.");
            await RemoveLinesThatContain("bloodbath notes have been added.");
            await RemoveLinesThatContain("bloodbath note have been added.");
        }

        private async Task CreateRpLog()
        {
            RemoveSingleWordLines();
            RemoveToasts();
            RemovePrompts();
            await RemoveHeader();
            await RemoveLinesThatStartWith(@"-->");
            await RemoveLinesThatStartWith(@"[ (C)ontinue, (R)efresh, (B)ack, (H)elp, (E)nd, (T)op, (Q)uit, or RETURN ]");
            await RemoveLinesThatStartWith("[Hit Return to continue]");
            await RemoveLinesThatStartWith("You have become better at");
            await RemoveLinesThatStartWith("You are logged in as:");
            await RemoveLinesThatContain("practice sessions left.");
            await RemoveLinesThatContain("You focus your training on");
            await RemoveSpells();
            await RemoveScore();
            await RemoveLinesThatStartWith("<");
            await RemoveLinesThatStartWith("You are using:");
            await RemoveLinesThatStartWith("You are affected by the following spells:");
            await RemoveLinesThatContain(" : modifies");
            await RemoveLinesThatStartWith("You aren't currently on a quest.");
            await RemoveLinesThatStartWith("There is less than a hour remaining until you can go on another quest.");
            await RemoveLinesThatStartWith("Master account:");
            await RemoveLinesThatStartWith("Personal Name: ");
            await RemoveLinesThatStartWith("E-mail: ");
            await RemoveLinesThatContain("Dark and Shattered Lands: ");

            for (int i = 1;i < 250; i++)
            {
                await RemoveLinesThatStartWith($"{i}. ");
            }

            await RemoveLinesThatStartWith("Please press enter to get back to the master menu or enter character name to log");
            await RemoveLinesThatStartWith("Player name:");

            RemoveBattle();
            await RemoveUnreadNotes();
            RemoveDoubleBlankLines();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAndReplace(object sender, RoutedEventArgs e)
        {
        }
    }
}
