using System;
using System.Text;
using System.Text.RegularExpressions;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// a sample implementation which uses Bi-Gram algorithm to suport Eastern Asian languages(Chinese, Japanese, Korean)
    /// </summary>
    public class CJKTokenizer : SQLiteFtsTokenizer
    {
        private enum TokenType
        {
            Unknown,
            CJK,
            Number,
            Word
        }

        private int pos;
        private TokenType tokenType;

        public override string Name
        {
            get { return "cjk"; }
        }

        protected override void PrepareToStart()
        {
            this.pos = -1;
        }

        protected override bool MoveNext()
        {
            while (ReadNext())
            {
                if (this.tokenType == TokenType.Word)
                {
                    base.Token = base.Token.ToLower();
                    if (Array.IndexOf(STOP_WORDS, base.Token) == -1)
                    {
                        return true;
                    }
                }
                else if (this.tokenType != TokenType.Unknown)
                {
                    return true;
                }
            }
            return false;
        }

        private static String[] STOP_WORDS = 
		{
			"a", "about", "an", "and", "are", "as", "at", "be",
			"by", "com", "d", "for", "from", "how", "in",
			"i", "is", "it", "ll", "m", "of", "on", "or", 
			"s", "t", "that", "the", "then", "this", "to", 
			"was", "what", "when", "where", "who", "will", 
			"with", "www", "http"
		};

        private bool ReadNext()
        {
            this.tokenType = TokenType.Unknown;
            base.NextIndexOfString = -1;

            while (this.pos < base.InputString.Length - 1)
            {
                char ch = base.InputString[++this.pos];
                if (Char.IsWhiteSpace(ch))
                {
                    continue;
                }
                if (IsCJK(ch))
                {
                    ReadCJK();
                    return true;
                }
                if (Char.IsLetterOrDigit(ch))
                {
                    ReadAlphaNum();
                    return true;
                }
            }
            return false;
        }

        #region ReadAlphaNum
        private void ReadAlphaNum()
        {
            int delta = 1;
            bool containsFullWidthCharacter = false;

            while (this.pos + delta < base.InputString.Length)
            {
                char ch2 = base.InputString[this.pos + delta];
                if (IsCJK(ch2) || (!Char.IsLetterOrDigit(ch2) && ch2 != '.' && ch2 != '．'))
                {
                    delta--;
                    break;
                }
                if (IsFullWidthAlphaNum(ch2))
                {
                    containsFullWidthCharacter = true;
                }
                delta++;
            }
            if (this.pos + delta >= base.InputString.Length ||
                base.InputString[this.pos + delta] == '.' || base.InputString[this.pos + delta] == '．')
            {
                delta--;
            }
            string str = base.InputString.Substring(this.pos, delta + 1);
            if (containsFullWidthCharacter)
            {
                str = FilterFullWidthAlphaNums(str);
            }

            if (Regex.IsMatch(str, @"^(-?\d+)(\.\d+)?$"))
            {
                this.tokenType = TokenType.Number;
                base.Token = str;
                base.TokenIndexOfString = this.pos;
                this.pos += delta;
            }
            else
            {
                if (str.IndexOf('.') > -1)
                {
                    delta = str.IndexOf('.') - 1;
                    str = base.InputString.Substring(this.pos, delta + 1);
                    if (containsFullWidthCharacter)
                    {
                        str = FilterFullWidthAlphaNums(str);
                    }
                }
                this.tokenType = TokenType.Word;
                base.Token = str;
                base.TokenIndexOfString = this.pos;
                this.pos += delta;
            }
        }
        #endregion


        #region ReadCJK
        private void ReadCJK()
        {
            if (IsHalfWidthKatakana(base.InputString[this.pos]))
            {
                int p = -1;
                char[] ch = new char[5];
                for (int i = 0; i < 5; i++)
                {
                    ch[i] = (this.pos + i < this.InputString.Length ? base.InputString[this.pos + i] : (char)0);
                    if (IsHalfWidthKatakana(ch[i]))
                    {
                        p = Array.IndexOf(HankakuKatakana, ch[i]);
                        if (p > -1)
                        {
                            ch[i] = ZenkakuKatakana[p];
                        }
                    }
                    else
                    {
                        ch[i] = (char)0;
                    }
                }
                int delta1 = 0; // for the first kana
                int delta2 = -1; // for the second kana (-1 means no second)
                char chA = ch[0];
                char chB = (char)0;

                #region try reading two kana
                if (ch[0] == 'ﾞ' || ch[0] == 'ﾟ')
                {
                    if (ch[1] > 0)
                    {
                        #region second kana ?
                        chB = ch[1];
                        delta2 = 1;

                        if (ch[2] == 'ﾞ')
                        {
                            p = Array.IndexOf(DakutenAddable, ch[1]);
                            if (p > -1)
                            {
                                chB = DakutenAdded[p];
                                delta2 = 2;
                            }
                        }
                        else if (ch[2] == 'ﾟ')
                        {
                            p = Array.IndexOf(HanDakutenAddable, ch[1]);
                            if (p > -1)
                            {
                                chB = HanDakutenAdded[p];
                                delta2 = 2;
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    if (ch[1] > 0)
                    {
                        if (ch[1] == 'ﾞ')
                        {
                            p = Array.IndexOf(DakutenAddable, ch[0]);
                            if (p > -1)
                            {
                                delta1 = 1;
                                chA = DakutenAdded[p];
                                if (ch[2] > 0)
                                {
                                    #region second kana ?
                                    delta2 = 2;
                                    chB = ch[2];

                                    if (ch[2] != 'ﾞ' && ch[2] != 'ﾟ')
                                    {
                                        if (ch[3] == 'ﾞ')
                                        {
                                            p = Array.IndexOf(DakutenAddable, ch[2]);
                                            if (p > -1)
                                            {
                                                chB = DakutenAdded[p];
                                                delta2 = 3;
                                            }
                                        }
                                        else if (ch[3] == 'ﾟ')
                                        {
                                            p = Array.IndexOf(HanDakutenAddable, ch[2]);
                                            if (p > -1)
                                            {
                                                chB = HanDakutenAdded[p];
                                                delta2 = 3;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                chB = ch[1];
                                delta2 = 1;
                            }
                        }
                        else if (ch[1] == 'ﾟ')
                        {
                            p = Array.IndexOf(HanDakutenAddable, ch[0]);
                            if (p > -1)
                            {
                                delta1 = 1;
                                chA = HanDakutenAdded[p];
                                if (ch[2] > 0)
                                {
                                    #region second kana ?
                                    delta2 = 2;
                                    chB = ch[2];

                                    if (ch[2] != 'ﾞ' && ch[2] != 'ﾟ')
                                    {
                                        if (ch[3] == 'ﾞ')
                                        {
                                            p = Array.IndexOf(DakutenAddable, ch[2]);
                                            if (p > -1)
                                            {
                                                chB = DakutenAdded[p];
                                                delta2 = 3;
                                            }
                                        }
                                        else if (ch[3] == 'ﾟ')
                                        {
                                            p = Array.IndexOf(HanDakutenAddable, ch[2]);
                                            if (p > -1)
                                            {
                                                chB = HanDakutenAdded[p];
                                                delta2 = 3;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                chB = ch[1];
                                delta2 = 1;
                            }
                        }
                        else
                        {
                            #region second kana ?
                            chB = ch[1];
                            delta2 = 1;

                            if (ch[2] == 'ﾞ')
                            {
                                p = Array.IndexOf(DakutenAddable, ch[1]);
                                if (p > -1)
                                {
                                    chB = DakutenAdded[p];
                                    delta2 = 2;
                                }
                            }
                            else if (ch[2] == 'ﾟ')
                            {
                                p = Array.IndexOf(HanDakutenAddable, ch[1]);
                                if (p > -1)
                                {
                                    chB = HanDakutenAdded[p];
                                    delta2 = 2;
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                if (chB > 0)
                {
                    base.Token = chA.ToString() + chB.ToString();
                    base.TokenIndexOfString = this.pos;
                    this.tokenType = TokenType.CJK;

                    if (ch[delta2 + 1] == 0)
                    {
                        base.NextIndexOfString = this.pos + delta2 + 1;
                        this.pos += delta2;
                    }
                    else
                    {
                        base.NextIndexOfString = this.pos + delta1 + 1;
                        this.pos += delta1;
                    }
                }
                else
                {
                    base.Token = chA.ToString();
                    base.TokenIndexOfString = this.pos;
                    base.NextIndexOfString = this.pos + delta1 + 1;
                    this.tokenType = TokenType.CJK;
                    this.pos += delta1;
                }
            }
            else
            {
                if (this.pos + 1 < base.InputString.Length &&
                    IsCJK(base.InputString[this.pos + 1]) &&
                    !IsHalfWidthKatakana(base.InputString[this.pos + 1]))
                {
                    base.Token = base.InputString.Substring(this.pos, 2);
                    base.TokenIndexOfString = this.pos;
                    base.NextIndexOfString = this.pos + 1;
                    this.tokenType = TokenType.CJK;

                    if (this.pos + 2 == base.InputString.Length ||
                        !IsCJK(base.InputString[this.pos + 2]) ||
                        IsHalfWidthKatakana(base.InputString[this.pos + 2]))
                    {
                        base.NextIndexOfString = this.pos + 2;
                        this.pos++;
                    }
                }
                else
                {
                    base.Token = base.InputString[this.pos].ToString();
                    base.TokenIndexOfString = this.pos;
                    base.NextIndexOfString = this.pos + 1;
                    this.tokenType = TokenType.CJK;
                }
            }
        }

        private static char[] HankakuKatakana = { 
			'ｦ', 'ｧ', 'ｨ', 'ｩ', 'ｪ', 'ｫ', 'ｬ', 'ｭ', 'ｮ', 'ｯ', 'ｰ', 'ｱ', 'ｲ',
			'ｳ', 'ｴ', 'ｵ', 'ｶ', 'ｷ', 'ｸ', 'ｹ', 'ｺ', 'ｻ', 'ｼ', 'ｽ', 'ｾ', 'ｿ',
			'ﾀ', 'ﾁ', 'ﾂ', 'ﾃ', 'ﾄ', 'ﾅ', 'ﾆ', 'ﾇ', 'ﾈ', 'ﾉ', 'ﾊ', 'ﾋ', 'ﾌ',
			'ﾍ', 'ﾎ', 'ﾏ', 'ﾐ', 'ﾑ', 'ﾒ', 'ﾓ', 'ﾔ', 'ﾕ', 'ﾖ', 'ﾗ', 'ﾘ', 'ﾙ',
			'ﾚ', 'ﾛ', 'ﾜ', 'ﾝ' };
        private static char[] ZenkakuKatakana = { 
			'ヲ', 'ァ', 'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ッ', 'ー', 'ア', 'イ',
			'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ',
			'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ',
			'ヘ', 'ホ', 'マ', 'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル',
			'レ', 'ロ', 'ワ', 'ン' };
        private static char[] DakutenAddable = {
			'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', 'タ', 'チ', 'ツ', 'テ', 'ト', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ'};
        private static char[] DakutenAdded = {
			'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', 'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ'};
        private static char[] HanDakutenAddable = { 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ' };
        private static char[] HanDakutenAdded = { 'パ', 'ピ', 'プ', 'ぺ', 'ポ' };

        private bool IsHalfWidthKatakana(char c)
        {
            return (c >= 0xff66 && c <= 0xff9f);
        }
        #endregion


        #region Helper methods
        private bool IsCJK(char c)
        {
            return (
                (c >= 0x4e00 && c <= 0x9fff) ||		// CJK main
                (c >= 0x3040 && c <= 0x31ff) ||		// Hiragana/Katakana
                (c >= 0x3400 && c <= 0x4dff) ||		// CJK ext. A
                (c >= 0xf900 && c <= 0xfaff) ||		// CJK compatibility
                (c >= 0xff66 && c <= 0xff9f) ||		// HalfWidth Katakana
                (c >= 0xac00 && c <= 0xd7af));		// Korean characters
        }
        private bool IsFullWidthAlphaNum(char c)
        {
            return ((c >= 0xff10 && c <= 0xff19) ||	//'０'～'９'
                (c >= 0xff21 && c <= 0xff3a) ||		//'Ａ'～'Ｚ'
                (c >= 0xff41 && c <= 0xff5a) ||		//'ａ'～'ｚ'
                c == '．');
        }
        private string FilterFullWidthAlphaNums(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (IsFullWidthAlphaNum(str[i]))
                {
                    sb.Append(Convert.ToChar(str[i] - 65248));
                }
                else
                {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }
        #endregion

    }
}
