using System;

namespace XLY.XDD.Control.ReadToTxt
{
    public interface IWordFile
    {
        String ParagraphText { get; }

        String HeaderAndFooterText { get; }

        String CommentText { get; }

        String FootnoteText { get; }

        String EndnoteText { get; }
    }
}