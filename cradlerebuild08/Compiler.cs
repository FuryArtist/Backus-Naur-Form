using System;
using System.IO;
using Сradle.lexer;
using Сradle.parser;

namespace Cradle
{
    class Compiler
    {
        static void Main(string[] args)
        {
            try
            {
                StreamReader input = new StreamReader(@"test\TestCode.txt");
                Lexer lexer = new Lexer(input);
                Parser parser = new Parser(lexer);
                parser.Program();
                Console.WriteLine(parser.output);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
            Console.ReadKey();
        }
    }
}
