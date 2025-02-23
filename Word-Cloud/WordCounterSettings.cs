using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Word_Cloud
{
    public class WordCounterSettings
    {
        public int MinWordLength { get; set; } = 0;

        public int MaxWordLength { get; set; } = int.MaxValue;

        public char SplitCharacter { get; set; } = ' ';

        public HashSet<string> IgnoredWords { get; set; } = new HashSet<string>();

        public bool IgnoreCase { get; set; } = true;

        public bool IgnoreDiacritics { get; set; } = false;

        public bool UseCustomPunctuationMarks { get; set; } = false;

        public List<char> CustomPunctuationMarks { get; set; } = new List<char>();

        public IgnoreLanguage IgnoreLanguage { get; set; } = IgnoreLanguage.None;

        public string[] GetIgnoredWords()
        {
            if (IgnoreLanguage == IgnoreLanguage.None)
                return Array.Empty<string>();
            if (IgnoreLanguage == IgnoreLanguage.English)
            {
                var resource = Properties.Resources.english100;
                return Regex.Split(resource, "\r\n|\r|\n");
            }
            if(IgnoreLanguage == IgnoreLanguage.Dutch)
            {
                var resource = Properties.Resources.dutch100;
                return Regex.Split(resource, "\r\n|\r|\n");
            }
            throw new ArgumentException();
        }
    }

    public enum IgnoreLanguage { None, Dutch, English }
}
