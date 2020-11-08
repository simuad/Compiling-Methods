using System.Collections.Generic;
using System.IO;

public class Lexer
{
    string buffer;
    private string file;
    private string fileName;
    private int lineNumber = 1;
    private int offset = 0;
    private int lineColumn = 0;
    bool running;
    private State state = State.START;
    private int tokenStart;
    private int tokenStartLine;
    private List<Token> tokens = new List<Token>();

    public Lexer(string file)
    {
        this.file = System.IO.File.ReadAllText(file);
        this.fileName = file;
        running = new FileInfo(file).Length == 0 ? false : true;
    }

    private void Add(char currChar)
    {
        buffer += currChar;
    }

    private void BeginToken(State newState)
    {
        tokenStart = offset;
        tokenStartLine = lineNumber;
        state = newState;
    }

    private void CompleteToken(TokenType tokenType, bool advance = true)
    {
        if(!advance)
        {
            offset--;
        }

        switch(state)
        {
            case State.LIT_INT:
            tokens.Add(new Token(tokenStartLine, tokenType, System.Convert.ToInt32(buffer)));
            break;
            case State.LIT_FLOAT:
            tokens.Add(new Token(tokenStartLine, tokenType, float.Parse(buffer, System.Globalization.CultureInfo.InvariantCulture)));
            break;
            case State.LIT_BOOL:
            tokens.Add(new Token(tokenStartLine, tokenType, bool.Parse(buffer)));
            break;
            default:
            tokens.Add(new Token(tokenStartLine, tokenType, buffer));
            break;
        }

        buffer = "";
        state = State.START;
    }

    public void Lex()
    {
        char currChar;

        while(running && offset < file.Length)
        {
            currChar = file[offset];
            LexChar(currChar);
            offset++;
            lineColumn++;
        }

        if(running && offset == file.Length)
        {
            currChar = file[offset-1];
            LexChar(currChar);
        }
        
        BeginToken(State.EOF);
        CompleteToken(TokenType.EOF);
    }

    private void LexChar(char currChar)
    {
        switch(state)
        {
            case State.IDENT:
                LexIdent(currChar);
                break;
            case State.LIT_INT:
                LexLitInt(currChar);
                break;
            case State.LIT_FLOAT:
                LexLitFloat(currChar);
                break;
            case State.FLOAT_SC:
                LexLitFloatSc(currChar);
                break;
            case State.LIT_STR:
                LexLitStr(currChar);
                break;
            case State.LIT_STR_ESCAPE:
                LexLitStrEscape(currChar);
                break;
            case State.OP_ADD:
                LexOpAdd(currChar);
                break;
            case State.OP_PRE_INC:
                LexOpPreInc(currChar);
                break;
            case State.OP_SUB:
                LexOpSub(currChar);
                break;
            case State.OP_PRE_DEC:
                LexOpPreDec(currChar);
                break;
            case State.OP_DIV:
                LexOpDiv(currChar);
                break;
            case State.OP_MULT:
                LexOpMult(currChar);
                break;
            case State.OP_MOD:
                LexOpMod(currChar);
                break;
            case State.OP_G:
                LexOpG(currChar);
                break;
            case State.OP_L:
                LexOpL(currChar);
                break;
            case State.OP_ASSIGN:
                LexOpAssign(currChar);
                break;
            case State.OP_L_NOT:
                LexOPLNot(currChar);
                break;
            case State.COMMENT:
                LexComment(currChar);
                break;
            case State.MULTI_COMMENT:
                LexMultiComment(currChar);
                break;
            case State.MULTI_COMMENT_END:
                LexMultiCommentEnd(currChar);
                break;
            case State.OP_L_AND:
                LexOpLAnd(currChar);
                break;
            case State.OP_L_OR:
                LexOpLOr(currChar);
                break;
            case State.START:
                LexStart(currChar);
                break;
            default:
                System.Console.WriteLine("{0}:{1}:{2}: error: unrecognised state '{3}'", fileName, lineNumber, lineColumn, state);
                running = false;
                break;
        }
    }

    private void LexComment(char currChar)
    {
        if(currChar == '\n')
        {
            lineNumber++;
            lineColumn = 0;
            state = State.START;
        }
    }

    private void LexIdent(char currChar)
    {
        if((char.IsLetterOrDigit(currChar) || currChar == '_') && offset != file.Length)
        {
            Add(currChar);
        }
        else
        {
            switch(buffer)
            {
                case "int":
                    buffer = "";
                    CompleteToken(TokenType.INT, false);
                    break;
                case "float":
                    buffer = "";
                    CompleteToken(TokenType.FLOAT, false);
                    break;
                case "bool":
                    buffer = "";
                    CompleteToken(TokenType.BOOL, false);
                    break;
                case "string":
                    buffer = "";
                    CompleteToken(TokenType.STRING, false);
                    break;
                case "void":
                    buffer = "";
                    CompleteToken(TokenType.LIT_VOID, false);
                    break;
                case "struct":
                    buffer = "";
                    CompleteToken(TokenType.STRUCT, false);
                    break;
                case "if":
                    buffer = "";
                    CompleteToken(TokenType.KW_IF, false);
                    break;
                case "else":
                    buffer = "";
                    CompleteToken(TokenType.KW_ELSE, false);
                    break;
                case "while":
                    buffer = "";
                    CompleteToken(TokenType.KW_WHILE, false);
                    break;
                case "for":
                    buffer = "";
                    CompleteToken(TokenType.KW_FOR, false);
                    break;
                case "break":
                    buffer = "";
                    CompleteToken(TokenType.KW_BREAK, false);
                    break;
                case "continue":
                    buffer = "";
                    CompleteToken(TokenType.KW_CONTINUE, false);
                    break;
                case "return":
                    buffer = "";
                    CompleteToken(TokenType.KW_RETURN, false);
                    break;
                case "true":
                    CompleteToken(TokenType.LIT_BOOL, false);
                    break;
                case "false":
                    CompleteToken(TokenType.LIT_BOOL, false);
                    break;
                default:
                    CompleteToken(TokenType.IDENT, false);
                    break;
            }
        }
    }

    private void LexLitFloat(char currChar)
    {
        if(char.IsNumber(currChar))
        {
            Add(currChar);
        }
        else
        {
            if(buffer.Length > 1)
            {
                CompleteToken(TokenType.LIT_FLOAT, false);
            }
            else
            {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected \".\"", fileName, lineNumber, lineColumn);
            running = false;
            }
        }
    }

    private void LexLitFloatSc(char currChar)
    {
        if(char.IsNumber(currChar))
        {
            Add(currChar);
        }
        else if (currChar == '-' && buffer.Length == 2)
        {
            Add(currChar);
        }
        else
        {
            CompleteToken(TokenType.LIT_FLOAT);
        }
    }

    private void LexLitInt(char currChar)
    {
        if(char.IsNumber(currChar) && offset != file.Length)
        {
            Add(currChar);
        }
        else if(currChar == '.')
        {
            Add(currChar);
            state = State.LIT_FLOAT;
        }
        else if(currChar == 'e')
        {
            if(buffer.Length == 1){
                Add(currChar);
                state = State.FLOAT_SC;
            }
            else if(buffer.Length == 2 && buffer[0] == '-')
            {
                Add(currChar);
                state = State.FLOAT_SC;
            }
            else
            {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected \"e\"", fileName, lineNumber, lineColumn);
            running = false;
            }
        }
        else if (char.IsLetter(currChar))
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: invalid integer literal", fileName, lineNumber, lineColumn);
            running = false;
        }
        else
        {
            CompleteToken(TokenType.LIT_INT, false);
        }
    }

    private void LexLitStr(char currChar)
    {
        if(currChar == '"')
        {
            CompleteToken(TokenType.LIT_STR);
        }
        else if(currChar == '\\')
        {
            state = State.LIT_STR_ESCAPE;
        }
        else if(currChar == '\n')
        {
            lineNumber++;
            lineColumn = 0;
            Add(currChar);
        }
        else
        {
            Add(currChar);
        }
    }

    private void LexLitStrEscape(char currChar)
    {
        if(currChar == '"')
        {
            buffer += "\"";
        }
        else if(currChar == 'n')
        {
            buffer += "\n";
        }
        else if(currChar == 't')
        {
            buffer += "\t";
        }
        else if(currChar == '\\')
        {
            buffer += "\\";
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }

        state = State.LIT_STR;
    }

    private void LexMultiComment(char currChar)
    {
        if(currChar == '\n')
        {
            lineNumber++;
            lineColumn = 0;
        }
        else if(currChar == '*')
        {
            state = State.MULTI_COMMENT_END;
        }
    }

    private void LexMultiCommentEnd(char currChar)
    {
        if(currChar == '/')
        {
            state = State.START;
        }
        else if(currChar == '*')
        {
            state = State.MULTI_COMMENT_END;
        }
        else if(currChar == '\n')
        {
            state = State.MULTI_COMMENT;
            lineNumber++;
            lineColumn = 0;
        }
        else
        {
            state = State.MULTI_COMMENT;
        }
    }

    private void LexOpAdd(char currChar)
    {
        if(currChar == '=')
        {
            buffer = "";
            CompleteToken(TokenType.OP_ASSIGN_OP);
        }
        else if(currChar == '+')
        {
            buffer = "";
            state = State.OP_PRE_INC;
        }
        else
        {
            buffer = "";
            CompleteToken(TokenType.OP_ADD, false);
        }
    }

    private void LexOpPreInc(char currChar)
    {
        if(char.IsLetterOrDigit(currChar) || currChar == '_')
        {
            CompleteToken(TokenType.OP_PRE_INC, false);
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }
    }

    private void LexOpSub(char currChar)
    {
        if(currChar == '=')
        {
            buffer = "";
            CompleteToken(TokenType.OP_ASSIGN_DIFF);
        }
        else if(currChar == '-')
        {
            buffer = "";
            state = State.OP_PRE_DEC;
        }
        else if(char.IsNumber(currChar))
        {
            Add(currChar);
            state = State.LIT_INT;
        }
        else
        {
            CompleteToken(TokenType.OP_SUB);
        }
    }

    private void LexOpPreDec(char currChar)
    {
        if(char.IsLetterOrDigit(currChar) || currChar == '_')
        {
            CompleteToken(TokenType.OP_PRE_DEC, false);
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }
    }

    private void LexOpDiv(char currChar)
    {
        if(currChar == '/')
        {
            if (offset == file.Length - 1)
            {
                state = State.START;
            }
            else
            {
                state = State.COMMENT;
            }
        }
        else if(currChar == '*')
        {
            if (offset == file.Length - 1)
            {
                System.Console.WriteLine("{0}:{1}:{2}: error: expected multi comment end", fileName, lineNumber, lineColumn);
                running = false;
            }
            state = State.MULTI_COMMENT;
        }
        else if(currChar == '=')
        {
            BeginToken(State.OP_ASSIGN_QUOT);
            CompleteToken(TokenType.OP_ASSIGN_QUOT);
        }
        else
        {
            BeginToken(state);
            CompleteToken(TokenType.OP_DIV, false);
        }
    }

    private void LexOpMult(char currChar)
    {
        if(currChar == '=')
        {
            BeginToken(State.OP_ASSIGN_PROD);
            CompleteToken(TokenType.OP_ASSIGN_PROD);
        }
        else
        {
            BeginToken(state);
            CompleteToken(TokenType.OP_MULT, false);
        }
    }

    private void LexOpMod(char currChar)
    {
        if(currChar == '=')
        {
            BeginToken(State.OP_ASSIGN_REM);
            CompleteToken(TokenType.OP_ASSIGN_REM);
        }
        else
        {
            CompleteToken(TokenType.OP_MOD, false);
        }
    }

    private void LexOpAssign(char currChar)
    {
        if(currChar == '=')
        {
            BeginToken(State.OP_EQ);
            CompleteToken(TokenType.OP_EQ);
        }
        else
        {
            BeginToken(state);
            CompleteToken(TokenType.OP_ASSIGN, false);
        }
    }

    private void LexOpG(char currChar)
    {
        if(currChar == '=')
        {
            BeginToken(State.OP_G_EQ);
            CompleteToken(TokenType.OP_G_EQ);
        }
        else
        {
            BeginToken(state);
            CompleteToken(TokenType.OP_G, false);
        }
    }

    private void LexOpL(char currChar)
    {
        if(currChar == '=')
        {
            BeginToken(State.OP_L_EQ);
            CompleteToken(TokenType.OP_L_EQ);
        }
        else
        {
            BeginToken(state);
            CompleteToken(TokenType.OP_L, false);
        }
    }

    private void LexOpLAnd(char currChar)
    {
        if(currChar == '&')
        {
            CompleteToken(TokenType.OP_L_AND);
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }
    }

    private void LexOpLOr(char currChar)
    {
        if(currChar == '|')
        {
            CompleteToken(TokenType.OP_L_OR);
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }
    }

    private void LexOPLNot(char currChar)
    {
        if(currChar == '=')
        {
            BeginToken(State.OP_L_NEQ);
            CompleteToken(TokenType.OP_L_NEQ);
        }
        else if(char.IsLetter(currChar) || currChar == '_' || currChar == '!' || currChar == '-')
        {
            BeginToken(state);
            CompleteToken(TokenType.OP_L_NOT, false);
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }
    }

    private void LexStart(char currChar)
    {
        if(offset == file.Length)
        {
            running = false;
        }
        else if(char.IsLetter(currChar) || currChar == '_')
        {
            Add(currChar);
            BeginToken(State.IDENT);
        }
        else if(char.IsNumber(currChar))
        {
            Add(currChar);
            BeginToken(State.LIT_INT);
        }
        else if(currChar == ' ' || currChar == '\t')
        {
        }
        else if(currChar == ';' && offset != file.Length)
        {
            BeginToken(State.SEMICOLON);
            CompleteToken(TokenType.SEMICOLON);
        }
        else if(currChar == '"')
        {
            BeginToken(State.LIT_STR);
        }
        else if(currChar == '\n')
        {
            lineNumber++;
            lineColumn = 0;
        }
        else if(currChar == '(')
        {
            BeginToken(State.OP_PAREN_O);
            CompleteToken(TokenType.OP_PAREN_O);
        }
        else if(currChar == ')')
        {
            BeginToken(State.OP_PAREN_C);
            CompleteToken(TokenType.OP_PAREN_C);
        }
        else if(currChar == '{')
        {
            BeginToken(State.OP_BRACE_O);
            CompleteToken(TokenType.OP_BRACE_O);
        }
        else if(currChar == '}')
        {
            BeginToken(State.OP_BRACE_C);
            CompleteToken(TokenType.OP_BRACE_C);
        }
        else if(currChar == '[')
        {
            BeginToken(State.OP_BRACK_O);
            CompleteToken(TokenType.OP_BRACK_O);
        }
        else if(currChar == ']')
        {
            BeginToken(State.OP_BRACK_C);
            CompleteToken(TokenType.OP_BRACK_C);
        }
        else if(currChar == ',')
        {
            BeginToken(State.OP_COMMA);
            CompleteToken(TokenType.OP_COMMA);
        }
        else if(currChar == '.')
        {
            Add(currChar);
            BeginToken(State.LIT_FLOAT);
        }
        else if(currChar == '+')
        {
            Add(currChar);
            BeginToken(State.OP_ADD);
        }
        else if(currChar == '-')
        {
            Add(currChar);
            BeginToken(State.OP_SUB);
        }
        else if(currChar == '/')
        {
            BeginToken(State.OP_DIV);
        }
        else if(currChar == '*')
        {
            BeginToken(State.OP_MULT);
        }
        else if(currChar == '%')
        {
            BeginToken(State.OP_MOD);
        }
        else if(currChar == '=')
        {
            BeginToken(State.OP_ASSIGN);
        }
        else if(currChar == '>')
        {
            BeginToken(State.OP_G);
        }
        else if(currChar == '<')
        {
            BeginToken(State.OP_L);
        }
        else if(currChar == '!')
        {
            BeginToken(State.OP_L_NOT);
        }
        else if(currChar == '&')
        {
            BeginToken(State.OP_L_AND);
        }
        else if(currChar == '|')
        {
            BeginToken(State.OP_L_OR);
        }
        else
        {
            System.Console.WriteLine("{0}:{1}:{2}: error: unexpected '{3}'", fileName, lineNumber, lineColumn, currChar);
            running = false;
        }
    }

    public void Output()
    {
        System.IO.StreamWriter outputFile = new System.IO.StreamWriter(".\\Lexer\\lexems.txt");
        outputFile.WriteLine("|{0,3}|{1,3}| {2,-16}| {3,-10}", "ID", "LN", "TYPE", "VALUE");
        foreach(var token in tokens){
            outputFile.WriteLine("|{0,3}|{1,3}| {2,-16}| {3,-10}", tokens.IndexOf(token), token.Line, token.Type, token.Value);
        }
        outputFile.Close();
    }

    public List<Token> GetTokens()
    {
        return tokens;
    }
}