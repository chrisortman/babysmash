using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Linq;

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
    }

    public class LetterNode {
        private Char _myLetter;
        private Dictionary<Char, LetterNode> _nextLetters = new Dictionary<char, LetterNode>();
 
        public LetterNode(char myLetter) {
            _myLetter = myLetter;
        }

        public LetterNode AddChild(Char letter) {
            var n = new LetterNode(letter);
            _nextLetters.Add(letter, n);
            return n;
        }

        public bool IsWordEnd() {
            return _nextLetters.Count == 0;
        }

        public LetterNode Next(char key) {
            return _nextLetters[key];
        }
    }
    public class WordDictionary {
        private Subject<string> _wordEntered;
        private LetterNode _currentSearchScope;
        private Dictionary<Char,LetterNode> _topLevelLetters = new Dictionary<char, LetterNode>();
        private StringBuilder _buffer = new StringBuilder();

        public WordDictionary() {
            _wordEntered = new Subject<string>();
        }

        public void AddWord(string word) {
            LetterNode start;
            var firstLetter = Char.ToLower(word[0]);
            if(_topLevelLetters.ContainsKey(firstLetter)) {
                start = _topLevelLetters[firstLetter];
            } else {
                start = new LetterNode(firstLetter);
                _topLevelLetters.Add(firstLetter, start);
            }
            
            foreach(var letter in word.Skip(1)) {
                var newNode = start.AddChild(Char.ToLower(letter));
                start = newNode;
            }
        }

        public IObservable<string> WordEntered {
            get { return _wordEntered; }
        }

        public void SendKey(Char key) {
            key = Char.ToLower(key);
            _buffer.Append(key);
            if(_currentSearchScope == null) {
                _currentSearchScope = _topLevelLetters[key];
            } else {
                _currentSearchScope = _currentSearchScope.Next(key);
            }

            if(_currentSearchScope.IsWordEnd()) {
                _wordEntered.OnNext(_buffer.ToString());
                _buffer = new StringBuilder();
                _currentSearchScope = null;
            }
        }
    }
}