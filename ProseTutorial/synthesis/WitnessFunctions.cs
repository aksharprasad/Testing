using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;

namespace ProseTutorial
{
    public class WitnessFunctions : DomainLearningLogic
    {
        public WitnessFunctions(Grammar grammar) : base(grammar)
        {
        }

        [WitnessFunction(nameof(Semantics.Add), 1, Verify = true)]
        public DisjunctiveExamplesSpec WitnessAddStartPosition(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, IEnumerable<object>>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = (Dictionary<uint?, uint?>)inputState[rule.Body[0]];
                var output = example.Value as uint?;
                var occurrences = new List<uint?>();
                foreach (uint? i in input.Keys)
                {
                    if (i < output)
                        occurrences.Add(i);
                }
                if (occurrences.Count == 0) return null;
                result[inputState] = occurrences.Cast<object>();
            }
            return new DisjunctiveExamplesSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Add), 2, DependsOnParameters = new[] { 1 })]
        public ExampleSpec WitnessAddEndPosition(GrammarRule rule, ExampleSpec spec, ExampleSpec startSpec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = (Dictionary<uint?, uint?>)inputState[rule.Body[0]];
                var output = example.Value as uint?;
                var start = (uint?)startSpec.Examples[inputState];
                if (input.ContainsKey(output - start))
                    result[inputState] = output - start;
            }
            return new ExampleSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Multiply), 1, Verify = true)]
        public DisjunctiveExamplesSpec WitnessMultiplyStartPosition(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, IEnumerable<object>>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = (Dictionary<uint?, uint?>)inputState[rule.Body[0]];
                var output = example.Value as uint?;
                var occurrences = new List<uint?>();
                foreach (uint? i in input.Keys)
                {
                    if (i < output)
                        occurrences.Add(i);
                }
                if (occurrences.Count == 0) return null;
                result[inputState] = occurrences.Cast<object>();
            }
            return new DisjunctiveExamplesSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Multiply), 2, DependsOnParameters = new[] { 1 })]
        public ExampleSpec WitnessMultiplyEndPosition(GrammarRule rule, ExampleSpec spec, ExampleSpec startSpec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = (Dictionary<uint?, uint?>)inputState[rule.Body[0]];
                var output = example.Value as uint?;
                var start = (uint?)startSpec.Examples[inputState];
                if (output % start == 0 && input.ContainsKey(output / start))
                    result[inputState] = output / start;
            }
            return new ExampleSpec(result);
        }

        /*[WitnessFunction(nameof(Semantics.Element), 1)]
        public ExampleSpec WitnessK(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as uint?;
                var v = (Dictionary<uint?,uint?>)inputState[rule.Body[0]];
                if (v.ContainsKey(output)) 
                    result[inputState] = v[output];
            }
            return new ExampleSpec(result);
        }*/

        [WitnessFunction(nameof(Semantics.Element), 1)]
        public DisjunctiveExamplesSpec WitnessK(GrammarRule rule, DisjunctiveExamplesSpec spec)
        {
            var kExamples = new Dictionary<State, IEnumerable<object>>();
            foreach (KeyValuePair<State, IEnumerable<object>> example in spec.DisjunctiveExamples)
            {
                State inputState = example.Key;
                var v = (Dictionary<uint?, uint?>)inputState[rule.Body[0]];
                var positions = new List<int>();
                foreach (uint? val in example.Value)
                {
                    if (v.ContainsKey(val))
                        positions.Add((int)v[val]);
                }
                if (positions.Count == 0) return null;
                kExamples[inputState] = positions.Cast<object>();
            }
            return DisjunctiveExamplesSpec.From(kExamples);
        }
    }
}