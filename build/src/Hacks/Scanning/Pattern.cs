using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

using Harmony;

namespace Azxc.Hacks.Scanning
{
    // Requires Transpiler
    public class Pattern
    {
        public List<CodeInstruction> instructions;

        private List<string> _pattern;
        public List<string> pattern
        {
            get { return _pattern; }
        }

        public Pattern()
        {
            // In that way set instructions myself, using public var
            instructions = new List<CodeInstruction>();
            _pattern = new List<string>();
        }

        public Pattern(List<CodeInstruction> instructions)
        {
            this.instructions = instructions;
            _pattern = new List<string>();
        }

        private Format Format(string instruction, string pattern)
        {
            bool test = false;

            string text = pattern;
            if (pattern.EndsWith("?"))
            {
                text = pattern.Remove(pattern.Length - 1);
                test = true;
            }

            bool contains = instruction.Contains(text);
            if (contains && !test)
                return Scanning.Format.True;
            else if (test)
            {
                if (contains)
                    return Scanning.Format.True;
                else
                    return Scanning.Format.Skip;
            }
            return Scanning.Format.False;
        }

        public List<Tuple<int, int>> Search()
        {
            List<Tuple<int, int>> coincidences = new List<Tuple<int, int>>();
            int conclusion = 0;
            int index = 0;
            while (index < instructions.Count)
            {
                Format output = Format(instructions[index].ToString(), pattern[conclusion]);
                if (output == Scanning.Format.True)
                    conclusion += 1;
                else if (output == Scanning.Format.Skip)
                {
                    conclusion += 1;
                    continue;
                }
                else
                    conclusion = 0;

                if (conclusion == pattern.Count)
                {
                    coincidences.Add(Tuple.Create(index - pattern.Count + 1, index));
                    conclusion = 0;
                }

                index += 1;
            }
            return coincidences;
        }

        public string AddInstruction(string code)
        {
            _pattern.Add(code);
            return code;
        }

        public string[] AddInstructions(string[] codes)
        {
            _pattern.AddRange(codes);
            return codes;
        }
    }
}