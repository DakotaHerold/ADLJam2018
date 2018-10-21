using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam
{
    public struct PersonTrait
    {
        public string Phrase;
        public Category Category;
        public Group groupType;

        public static bool operator ==(PersonTrait x, PersonTrait y)
        {
            return x.Category == y.Category && x.Phrase == y.Phrase && x.groupType == y.groupType;
        }
        public static bool operator !=(PersonTrait x, PersonTrait y)
        {
            return !(x == y);
        }
    }
}