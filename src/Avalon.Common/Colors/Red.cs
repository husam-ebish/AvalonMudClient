﻿namespace Avalon.Common.Colors
{
    public class Red : AnsiColor
    {
        public override string AnsiCode => "\x1B[1;31m";

        public override string MudColorCode => "{R";

        public override string Name => "Red";

    }
}
