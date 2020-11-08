class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("Please pass only one argument");
            System.Environment.Exit(1);
        }

        Lexer lexer = new Lexer(args[0]);
        lexer.Lex();
        lexer.Output();
        
        Parser parser = new Parser(lexer.GetTokens());
        TMProgram root = parser.parseProgram();
        ASTPrinter printer = new ASTPrinter();
        printer.print("root", root);
    }
}

