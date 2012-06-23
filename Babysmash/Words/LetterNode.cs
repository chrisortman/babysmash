using System;
using System.Collections.Generic;

namespace BabySmash.Words {
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
            if (_nextLetters.ContainsKey(key)) {
                return _nextLetters[key];
            } else {
                return null;
            }

        }
    }
}