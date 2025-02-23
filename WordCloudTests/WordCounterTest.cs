using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word_Cloud;

namespace WordCloudTests
{
    [TestClass]
    public class WordCounterTest
    {
        private string sampleText = "In a quiet town, a curious cat wandered along cobblestone streets under the golden sun. The curious cat loved to explore, and every corner of the quiet town held a mystery. Every whisper of the wind told a story, and every story led the curious cat to another corner of the quiet town. The golden sun shined brightly as the wind carried the sounds of laughter. Children ran through the cobblestone streets, their laughter echoing in the quiet town. The curious cat watched, fascinated by the way life unfolded. Day after day, the curious cat returned to the same cobblestone streets, chasing shadows and listening to the wind. The quiet town never seemed to change, yet every day, the curious cat found something new—a hidden path, a secret alley, or a soft whisper in the wind.";

        private string stringWithDifferentCasing = "Test test test Test";

        private string stringWithDiacritics = "crème brûlée creme brulee";

        private string stringSplitWithUnderscores = "a_small_test_but_with_underscores_instead_of_spaces_repeat_repeat_repeat";

        // $^+|<>= are not considered punctuation by c#'s char.IsPunctuation method, so we use them here.
        private string stringWithCustomPunctuation = "This$is^a+sentence|with<custom>punctuation=";

        private string fileLocation = "..\\..\\..\\sample text.txt";

        [TestMethod]
        public void TestDefaultSettingsString()
        {
            var wordCounter = new WordCounter();

            var methodResult = wordCounter.CountFromString(sampleText);
        }

        [TestMethod]
        public void TestDefaultSettingsFromFile()
        {
            var wordCounter = new WordCounter();

            var methodResult = wordCounter.CountFromFile(fileLocation);
            var fileAsText = new StreamReader(fileLocation).ReadToEnd();
            var methodResult2 = wordCounter.CountFromString(fileAsText);

            CollectionAssert.AreEquivalent(methodResult, methodResult2);
        }

        [TestMethod]
        public void TestMinLength()
        {
            int minWordLenght = 3;
            var wordCounterSettings = new WordCounterSettings { MinWordLength = minWordLenght };
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(sampleText);
            foreach (var kvp in methodResult)
            {
                Assert.IsTrue(kvp.Item1.Length >= minWordLenght);
            }
        }

        [TestMethod]
        public void TestMaxLength()
        {
            int maxWordLenght = 8;
            var wordCounterSettings = new WordCounterSettings { MaxWordLength = maxWordLenght };
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(sampleText);
            foreach (var kvp in methodResult)
            {
                Assert.IsTrue(kvp.Item1.Length <= maxWordLenght);
            }
        }

        [TestMethod]
        public void TestMinAndMaxLength()
        {
            int minWordLenght = 3;
            int maxWordLenght = 8;
            var wordCounterSettings = new WordCounterSettings { MinWordLength = minWordLenght, MaxWordLength = maxWordLenght };
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(sampleText);
            foreach (var kvp in methodResult)
            {
                Assert.IsTrue(kvp.Item1.Length <= maxWordLenght);
                Assert.IsTrue(kvp.Item1.Length >= minWordLenght);
            }
        }

        [TestMethod]
        public void TestCustomSplitCharacter()
        {
            var wordCounterSettings = new WordCounterSettings { SplitCharacter = '_' };
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringSplitWithUnderscores);

            Assert.IsTrue(methodResult.Count > 1);
        }

        [TestMethod]
        public void TestNoCustomSplitCharacter()
        {
            var wordCounterSettings = new WordCounterSettings();
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringSplitWithUnderscores);

            Assert.IsTrue(methodResult.Count == 1);
        }

        [TestMethod]
        public void TestIgnoredWords()
        {
            var ignoredWord = "cobblestone";
            var wordCounterSettings = new WordCounterSettings();
            wordCounterSettings.IgnoredWords.Add(ignoredWord);
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(sampleText);
            Assert.IsFalse(methodResult.Any(x => x.Item1 == ignoredWord));
        }

        [TestMethod]
        public void TestNoIgnoredWords()
        {
            var ignoredWord = "cobblestone";
            var wordCounterSettings = new WordCounterSettings();
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(sampleText);
            Assert.IsTrue(methodResult.Any(x => x.Item1 == ignoredWord));
        }

        [TestMethod]
        public void TestDontIgnoreCase()
        {
            var wordCounterSettings = new WordCounterSettings { IgnoreCase = false };
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringWithDifferentCasing);
            Assert.IsTrue(methodResult.Count > 1);
        }

        [TestMethod]
        public void TestIgnoreCase()
        {
            var wordCounterSettings = new WordCounterSettings();
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringWithDifferentCasing);
            Assert.IsTrue(methodResult.Count == 1);
        }

        [TestMethod]
        public void TestIgnoreDiacritics()
        {
            var wordCounterSettings = new WordCounterSettings { IgnoreDiacritics = true};
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringWithDiacritics);
            Assert.IsTrue(methodResult.Count == 2);
        }

        [TestMethod]
        public void TestDoNotIgnoreDiacritics()
        {
            var wordCounterSettings = new WordCounterSettings();
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringWithDiacritics);
            Assert.IsTrue(methodResult.Count == 4);
        }

        [TestMethod]
        public void TestCustomPunctuationMarks()
        {
            var customPunctuationMarks = "$^+|<>=";
            var wordCounterSettings = new WordCounterSettings { UseCustomPunctuationMarks = true };
            wordCounterSettings.CustomPunctuationMarks.AddRange(customPunctuationMarks);
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringWithCustomPunctuation);
            var sb = new StringBuilder();
            foreach (var kvp in methodResult)
            {
                sb.Append(kvp.Item1);
            }
            var result = sb.ToString();
            foreach(var character in customPunctuationMarks)
            {
                Assert.IsFalse(result.Contains(character));
            }
        }

        [TestMethod]
        public void TestNoCustomPunctuationMarks()
        {
            var customPunctuationMarks = "$^+|<>=";
            var wordCounterSettings = new WordCounterSettings();
            wordCounterSettings.CustomPunctuationMarks.AddRange(customPunctuationMarks);
            var wordCounter = new WordCounter(wordCounterSettings);
            var methodResult = wordCounter.CountFromString(stringWithCustomPunctuation);
            var sb = new StringBuilder();
            foreach (var kvp in methodResult)
            {
                sb.Append(kvp.Item1);
            }
            var result = sb.ToString();
            foreach (var character in customPunctuationMarks)
            {
                Assert.IsTrue(result.Contains(character));
            }
        }
    }
}
