using System;
using System.Collections;
using System.Collections.Generic;

namespace MoleculeParse
{
    class MoleculeParser
    {
        public static Dictionary<string, ushort> Parse(string structure)
        {
            // var tokenized = new MoleculeStructure(structure);
            var tokenized = Tokenize(structure);
            var flattenedStructure = new List<Token>();
            var enumerator = tokenized.GetEnumerator();
            Token _val;
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item.val == ")")
                {
                    enumerator.MoveNext();
                    var multiplier = ushort.Parse(enumerator.Current.val);
                    int index = flattenedStructure.Count;
                    while (flattenedStructure[--index].val != "(")
                    {
                        if (flattenedStructure[index].type == Token.Type.Number)
                        {
                            _val = new Token();
                            _val.type = Token.Type.Number;
                            _val.val = (ushort.Parse(flattenedStructure[index].val) * multiplier).ToString();
                            flattenedStructure[index] = _val;
                        }
                    }
                    flattenedStructure.RemoveAt(index);
                }
                else
                {
                    flattenedStructure.Add(item);
                }
            }
            var atoms = new Dictionary<string, ushort>();
            for (int i = 0; i < flattenedStructure.Count; i += 2)
            {
                if (atoms.ContainsKey(flattenedStructure[i].val))
                    atoms[flattenedStructure[i].val] += ushort.Parse(flattenedStructure[i + 1].val);
                else
                    atoms.Add(flattenedStructure[i].val, ushort.Parse(flattenedStructure[i + 1].val));
            }
            return atoms;
        }
        static IEnumerable<Token> Tokenize(string structure)
        {
            int index = -1;
            Token last = new Token();
            last.type = Token.Type.NotDef;
            while (true)
            {
                index++;
                // if the last value exists and the last value was an atom token.
                // and if done or the current value is not a number than return a 1
                if (index >= 0 && (last.type == Token.Type.Atom || (last.type == Token.Type.Char && (last.val == ")")))
                    && (index >= structure.Length || Char.IsNumber(structure[index]) == false))
                {
                    last = new Token();
                    last.val = "1";
                    last.type = Token.Type.Number;
                    index--;
                }
                else if (index >= structure.Length)
                {
                    yield break;
                }
                else if (Char.IsNumber(structure[index]))
                {
                    var val = "" + structure[index];
                    while ((index + 1) < structure.Length && Char.IsNumber(structure[index + 1]))
                    {
                        index++;
                        val += structure[index];
                    }
                    last = new Token();
                    last.val = val;
                    last.type = Token.Type.Number;
                }
                else if (Char.IsLetter(structure[index]))
                {
                    string val = structure[index].ToString();
                    if ((index + 1) < structure.Length
                        && Char.IsLetter(structure[index + 1])
                        && Char.IsLower(structure[index + 1]))
                    {
                        index++;
                        val += structure[index];
                    }
                    last = new Token();
                    last.type = Token.Type.Atom;
                    last.val = val;
                }
                else
                { // special character (+ and - characters are not yet implemented)
                    if (structure[index] != '(' && structure[index] != ')')
                        continue; // skip unregnized character, hopefully just a space or +/- character
                    last = new Token();
                    last.type = Token.Type.Char;
                    last.val = "" + structure[index];
                }
                yield return last;
            }
        }

        private struct Token
        {
            public enum Type { NotDef, Number, Atom, Char }
            public Type type;
            public string val;
        }
    }
}
