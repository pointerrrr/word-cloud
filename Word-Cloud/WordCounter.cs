using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Word_Cloud
{
    public class WordCounter
    {
        private WordCounterSettings wordCounterSettings;

        public WordCounter()
        {
            wordCounterSettings = new WordCounterSettings();
        }

        public WordCounter(WordCounterSettings settings) 
        {
            wordCounterSettings = settings;
        }

        /// <summary>
        /// Count the words in the string text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<(string, int)> CountFromString(string text)
        {
            // replace new lines with spaces
            var finalText = Regex.Replace(text, "\r\n|\r|\n", " ");
            if (wordCounterSettings.IgnoreCase)
                finalText = finalText.ToLower();

            if(wordCounterSettings.IgnoreDiacritics)
                finalText = RemoveDiacritics(finalText);

            if(wordCounterSettings.UseCustomPunctuationMarks)
                finalText = string.Join(" ", finalText.Split(wordCounterSettings.CustomPunctuationMarks.ToArray()));
            
            finalText = StripPunctuation(finalText, wordCounterSettings.SplitCharacter);

            IEnumerable<string> uncountedWords = finalText.Split(wordCounterSettings.SplitCharacter).Where(word => !wordCounterSettings.IgnoredWords.Contains(word));

            uncountedWords = uncountedWords.Where(word => !string.IsNullOrEmpty(word));

            if (wordCounterSettings.MinWordLength > 0)
                uncountedWords = uncountedWords.Where(word => word.Length >= wordCounterSettings.MinWordLength);
            if (wordCounterSettings.MaxWordLength < int.MaxValue)
                uncountedWords = uncountedWords.Where(word => word.Length <= wordCounterSettings.MaxWordLength);

            var wordCount = uncountedWords
                .GroupBy(p => p, p => p,
                (p1, p2) => new {Key = p1, Count = p2.Count()})
                .OrderByDescending(word => word.Count)
                .Select(word => (word.Key, word.Count)).ToList();

            return wordCount;
        }

        /// <summary>
        /// Load file at filePath as a string and count the words in it.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<(string, int)> CountFromFile(string filePath)
        {
            try
            {
                string text;
                using (var reader = new StreamReader(filePath))
                {
                    text = reader.ReadToEnd();
                }
                return CountFromString(text);
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Encountered exception {1}", ex.Message);
                throw;
            }            
        }

        // Taken from https://stackoverflow.com/questions/421616/how-can-i-strip-punctuation-from-a-string
        // Removes punctuation from input. The split character is there to ensure it does not get removed if it is also a punctuation mark.
        private static string StripPunctuation(string input, char splitCharacter)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (!char.IsPunctuation(c) || c == splitCharacter)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        // Taken from https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        // Removes all the diacritics from the string text, eg. turns crême into creme.
        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
