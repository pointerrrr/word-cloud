using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Word_Cloud;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace WordCloudTests
{
    [TestClass]
    public class SettingsTest
    {
        private Dictionary<string, string> resources = new Dictionary<string, string>();

        private const string englishFileName = "english100";
        private const string dutchFileName = "dutch100";

        [TestInitialize]
        public void Initialize()
        {
            var otherAssembly = Assembly.Load("Word-Cloud");
            var resource = otherAssembly.GetManifestResourceStream("Word_Cloud.Properties.Resources.resources");
            if (resource is null)
            {
                throw new NullReferenceException();
            }
            
            using (var reader = new ResourceReader(resource))
            {
                var dict = reader.GetEnumerator();
                while (dict.MoveNext())
                {
                    var key = dict.Key.ToString() ?? throw new NullReferenceException();
                    var value = dict.Value;
                    if (value is string text)
                    {
                        resources.Add(key, text);
                    }
                }
            }
        }

        [TestMethod]
        public void TestLanguageNone()
        {
            var wordCounterSettings = new WordCounterSettings();
            var ignoredWords = wordCounterSettings.GetIgnoredWords();
            Assert.AreEqual(ignoredWords, Array.Empty<string>());
        }

        [TestMethod]
        public void TestLanguageEnglish()
        {
            var wordCounterSettings = new WordCounterSettings { IgnoreLanguage = IgnoreLanguage.English };

            var englishIgnoredWords = Regex.Split(resources[englishFileName], "\r\n|\r|\n");

            var methodResult = wordCounterSettings.GetIgnoredWords();

            Assert.AreEqual(englishIgnoredWords.Length, 100);
            CollectionAssert.AreEqual(methodResult, englishIgnoredWords);
        }

        [TestMethod]
        public void TestLanguageDutch()
        {
            var wordCounterSettings = new WordCounterSettings { IgnoreLanguage = IgnoreLanguage.Dutch };

            var dutchIgnoredWords = Regex.Split(resources[dutchFileName], "\r\n|\r|\n");

            var methodResult = wordCounterSettings.GetIgnoredWords();

            Assert.AreEqual(dutchIgnoredWords.Length, 100);
            CollectionAssert.AreEqual(methodResult, dutchIgnoredWords);
        }
    }
}