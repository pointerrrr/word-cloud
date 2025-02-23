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
        // Ignore any words shorter than this.
        public int MinWordLength { get; set; } = 0;

        // Ignore words longer than this.
        public int MaxWordLength { get; set; } = int.MaxValue;

        // The character to split words on, defaults to space.
        public char SplitCharacter { get; set; } = ' ';

        // Custom words to ignore for the word counter.
        public HashSet<string> IgnoredWords { get; set; } = new HashSet<string>();

        // Words with upper case letters are treated the same as words without.
        public bool IgnoreCase { get; set; } = true;

        // Treat characters with diacritics as if they have none, eg. crême would be equal to creme.
        public bool IgnoreDiacritics { get; set; } = false;

        // Treat custom characters as punctuations.
        public bool UseCustomPunctuationMarks { get; set; } = false;

        public List<char> CustomPunctuationMarks { get; set; } = new List<char>();

        // Ignore the 100 most common words of selected language.
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
