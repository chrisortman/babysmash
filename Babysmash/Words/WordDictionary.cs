using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace BabySmash.Words {
    public class WordDictionary {
        private Subject<string> _wordEntered;
        private LetterNode _currentSearchScope;
        private Dictionary<char,LetterNode> _topLevelLetters = new Dictionary<char, LetterNode>();
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
            if(_currentSearchScope != null) {
                _currentSearchScope = _currentSearchScope.Next(key) ;
                if(_currentSearchScope != null) {
                    _buffer.Append(key);
                } else {
                    _buffer.Clear();
                    _currentSearchScope = _topLevelLetters[key];
                    if(_currentSearchScope != null) {
                        _buffer.Append(key);
                    }
                }
            } else {
                if (_topLevelLetters.ContainsKey(key)) {
                    _buffer.Append(key);
                    _currentSearchScope = _topLevelLetters[key];
                }
            }


            if(_currentSearchScope != null &&
               _currentSearchScope.IsWordEnd()) {
                _wordEntered.OnNext(_buffer.ToString());
                _buffer = new StringBuilder();
                _currentSearchScope = null;
            }
        }

        public void SendKey(string key) {
            SendKey(key[0]);
        }
    }
}