using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Compiler;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Learning.Strategies;
using Microsoft.ProgramSynthesis.Specifications;
using Microsoft.ProgramSynthesis.VersionSpace;

namespace ProseTutorial
{
    internal class Program
    {
        private static readonly Grammar Grammar = DSLCompiler.Compile(new CompilerOptions
        {
            InputGrammarText = File.ReadAllText("synthesis/grammar/substring.grammar"),
            References = CompilerReference.FromAssemblyFiles(typeof(Program).GetTypeInfo().Assembly)
        }).Value;

        private static SynthesisEngine _prose;

        private static readonly Dictionary<State, object> Examples = new Dictionary<State, object>();
        private static ProgramNode _topProgram;

        private static void Main(string[] args)
        {
            _prose = ConfigureSynthesis();
            var menu = @"Select one of the options: 
1 - provide new example
2 - run top synthesized program on a new input
3 - exit";
            var option = 0;
            while (option != 3)
            {
                Console.Out.WriteLine(menu);
                try
                {
                    option = short.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.Out.WriteLine("Invalid option. Try again.");
                    continue;
                }

                try
                {
                    RunOption(option);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Something went wrong...");
                    Console.Error.WriteLine("Exception message: {0}", e.Message);
                }
            }
        }

        private static void RunOption(int option)
        {
            switch (option)
            {
                case 1:
                    LearnFromNewExample();
                    break;
                case 2:
                    RunOnNewInput();
                    break;
                default:
                    Console.Out.WriteLine("Invalid option. Try again.");
                    break;
            }
        }

        private static void LearnFromNewExample()
        {
            Console.Out.Write("Enter 4 elements of the input:\n");
            try
            {
                Dictionary<uint?, uint?> d = new Dictionary<uint?, uint?>();
                for (int t = 0; t < 4; t++)
                {
                    //Console.WriteLine("Please enter integer" + (t+1) +": ");
                    d.Add((uint)Convert.ToInt32(Console.ReadLine()), (uint)t);
                }

                Console.WriteLine("Please enter desired output: ");
                uint output = (uint)Convert.ToInt32(Console.ReadLine());

                State inputState = State.CreateForExecution(Grammar.InputSymbol, d);
                Examples.Add(inputState, output);
            }
            catch (Exception)
            {
                throw new Exception("Invalid example");
            }

            var spec = new ExampleSpec(Examples);
            Console.Out.WriteLine("Learning a program for examples:");
            foreach (KeyValuePair<State, object> example in Examples)
            {
                Dictionary<uint?, uint?> temp = (Dictionary<uint?, uint?>)example.Key.Bindings.First().Value;
                Console.Write("[");
                foreach (uint? m in temp.Keys)
                    Console.Write(m + ",");
                Console.Write("] ");
                Console.Write("-> \"{0}\"\n", example.Value);
            }

            var scoreFeature = new RankingScore(Grammar);
            ProgramSet topPrograms = _prose.LearnGrammarTopK(spec, scoreFeature, 4, null);
            if (topPrograms.IsEmpty)
                throw new Exception("No program was found for this specification.");

            _topProgram = topPrograms.RealizedPrograms.First();
            Console.Out.WriteLine("Top 4 learned programs:");
            var counter = 1;
            foreach (ProgramNode program in topPrograms.RealizedPrograms)
            {
                if (counter > 4) break;
                Console.Out.WriteLine("==========================");
                Console.Out.WriteLine("Program {0}: ", counter);
                Console.Out.WriteLine(program.PrintAST(ASTSerializationFormat.HumanReadable));
                counter++;
            }
        }

        private static void RunOnNewInput()
        {
            if (_topProgram == null)
                throw new Exception("No program was synthesized. Try to provide new examples first.");
            Console.Out.WriteLine("Top program: {0}", _topProgram);
            try
            {
                Dictionary<uint?, uint?> d = new Dictionary<uint?, uint?>();
                Console.Out.Write("Enter 4 elements of the input:\n");
                for (int t = 0; t < 4; t++)
                {
                    //Console.WriteLine("Please enter integer" + (t+1) +": ");
                    d.Add((uint)Convert.ToInt32(Console.ReadLine()), (uint)t);
                }

                State inputState = State.CreateForExecution(Grammar.InputSymbol, d);
                Console.Out.WriteLine("RESULT: {0}", _topProgram.Invoke(inputState));

            }
            catch (Exception)
            {
                throw new Exception("The execution of the program on this input thrown an exception");
            }
        }

        public static SynthesisEngine ConfigureSynthesis()
        {
            var witnessFunctions = new WitnessFunctions(Grammar);
            var deductiveSynthesis = new DeductiveSynthesis(witnessFunctions);
            var synthesisExtrategies = new ISynthesisStrategy[] { deductiveSynthesis };
            var synthesisConfig = new SynthesisEngine.Config { Strategies = synthesisExtrategies };
            var prose = new SynthesisEngine(Grammar, synthesisConfig);
            return prose;
        }
    }
}