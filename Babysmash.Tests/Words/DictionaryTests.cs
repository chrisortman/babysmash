using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Babysmash.Tests.Words {
    [TestClass]
    public class DictionaryTests {
        [TestMethod]
        public void Can_detect_a_word_from_a_sequence_of_keys() {
            var dict = new WordDictionary();
            dict.AddWord("Cat");
            string enteredWord = "";
            dict.WordEntered.Subscribe(word => {
                enteredWord = word;
            });

            dict.SendKey('C');
            dict.SendKey('a');
            dict.SendKey('t');

            enteredWord.ShouldBe("cat");

        }

        [TestMethod]
        public void Can_detect_the_word_when_it_is_in_the_middle_of_keys() {
            var dict = new WordDictionary();
            dict.AddWord("Cat");
            string enteredWord = "";
            dict.WordEntered.Subscribe(word =>
            {
                enteredWord = word;
            });

            dict.SendKey('b');
            dict.SendKey('C');
            dict.SendKey('a');
            dict.SendKey('t');
            dict.SendKey('t');

            enteredWord.ShouldBe("cat");
        }

        [TestMethod]
        public void Picks_the_right_word_when_it_starts_down_the_wrong_path() {
            var dict = new WordDictionary();
            dict.AddWord("Cat");
            dict.AddWord("dog");

            string enteredWord = "";
            dict.WordEntered.Subscribe(word =>
            {
                enteredWord = word;
            });

            dict.SendKey('b');
            dict.SendKey('C');
            dict.SendKey('a');
            dict.SendKey('d');
            dict.SendKey('o');
            dict.SendKey('g');
            dict.SendKey('t');
            dict.SendKey('t');

            enteredWord.ShouldBe("dog");
        }
    }
}