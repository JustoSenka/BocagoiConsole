﻿using System;

namespace BocagoiConsole.Core;

public static class Strings
{
    public const string Menu = @"Bocagoi Language Learning by Justas Glodenis

Enter a command number:

1. Practice words
2. Add/Modify Words in Box
3. Create new word Box
4. Search
5. History
6. Most failed words
7. Least practiced words
8. Settings

0. Quit

...";

    public const string PracticeSelectBox = @"Bocagoi Language Learning by Justas Glodenis

Select a box from which to write words:

{0}

101. Practice 'Most failed words'.
102. Practice 'Least practiced words'.

0. Go back

...";

    public const string PracticeSelectWords1 = @"Bocagoi Language Learning by Justas Glodenis

There are {0} words in total, enter a range which ones you would like to practice:
";

    public const string PracticeSelectWords2 = @"From: ";
    public const string PracticeSelectWords3 = @"To: ";

    public const string PracticeSelectMode = @"Bocagoi Language Learning by Justas Glodenis

Select translation style:

1. Native -> Foreign
2. Foreign -> Native

0. Go back

...";

    public const string AddWordsToBox = @"Bocagoi Language Learning by Justas Glodenis

Select a box which too add words to:

{0}

0. Go back

...";

    public static readonly string AddingWordsToBoxExample = @"foreignWords1 - nativeTranslation1" + Environment.NewLine + "foreign - native";

    public const string SearchWords = @"Bocagoi Language Learning by Justas Glodenis

Enter part of a words or phrase to search for:

...";

    public const string History = @"Bocagoi Language Learning by Justas Glodenis

History:

{0}

Press enter to go back...";

    public const string LeastPracticedWords = @"Bocagoi Language Learning by Justas Glodenis

Least practiced words:

{0}

Press enter to go back...";

    public const string MostFailedWords = @"Bocagoi Language Learning by Justas Glodenis

Most Failed Words:

{0}

Press enter to go back...";
}
