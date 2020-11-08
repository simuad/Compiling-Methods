using System.Collections.Generic;

public class Parser
{
    private List<Token> tokens;
    private int offset;
    Token currToken;
    TokenType[] assignOp = new TokenType[]
    {
        TokenType.OP_ASSIGN,
        TokenType.OP_ASSIGN_OP,
        TokenType.OP_ASSIGN_PROD,
        TokenType.OP_ASSIGN_QUOT,
        TokenType.OP_ASSIGN_REM
    };

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.offset = 0;
    }

    private Token accept(TokenType type)
    {
        this.currToken = this.tokens[this.offset];
        if (this.currToken.Type == type)
        {
            this.offset++;
            return this.currToken;
        }
        return null;
    }

    private Token expect(TokenType type)
    {
        this.currToken = this.tokens[this.offset];
        if(this.currToken.Type == type)
        {
            this.offset++;
            return this.currToken;
        }
        else
        {
            return null;
        }
    }

    private Token peek(TokenType type)
    {
        Token currToken = tokens[offset];
        if(currToken.Type == type)
        {
            return currToken;
        }
        else
        {
            return null;
        }
    }

    private bool peek2(TokenType type0, TokenType type1)
    {
        Token token0 = tokens[offset];
        Token token1 = tokens[offset + 1];
        return token0.Type == type0 && token1.Type == type1;
    }

    private TokenType getTokenType()
    {
        return this.tokens[this.offset].Type;
    }

    public TMProgram parseProgram()
    {
        List<Declaration> decls = new List<Declaration>();
        while(true)
        {
            if(this.getTokenType() == TokenType.EOF)
            {
                break;
            }
            else
            {
                decls.Add(parseDeclaration());
            }
        }
        return new TMProgram(decls);
    }

    private Declaration parseDeclaration()
    {
        return parseFnDeclaration();
    }

    private FnDeclaration parseFnDeclaration()
    {
        Type retType = parseType();
        Token name = expect(TokenType.IDENT);
        List<Parameter> parameter = parseParams();
        Body body = new Body(parseStatementBlock());
        return new FnDeclaration(retType, name, parameter, body);
    }

    private List<Parameter> parseParams()
    {
        List<Parameter> parameters = new List<Parameter>();

        expect(TokenType.OP_PAREN_O);

        if(peek(TokenType.OP_PAREN_C) != null)
        {
            accept(TokenType.OP_PAREN_C);
            return parameters;
        }
        else
        {
            parameters.Add(parseParam());
        }

        while (accept(TokenType.OP_PAREN_C) == null)
        {
            expect(TokenType.OP_COMMA);
            parameters.Add(parseParam());
        }
        return parameters;
    }

    private Parameter parseParam()
    {
        Type type = parseType();
        Token name = expect(TokenType.IDENT);
        return new Parameter(type, name);
    }

    private Type parseType()
    {
        switch(getTokenType())
        {
            case TokenType.BOOL:
                expect(TokenType.BOOL);
                return new TypePrim(currToken);
            case TokenType.STRING:
                expect(TokenType.STRING);
                return new TypePrim(currToken);
            case TokenType.INT:
                expect(TokenType.INT);
                return new TypePrim(currToken);
            case TokenType.FLOAT:
                expect(TokenType.FLOAT);
                return new TypePrim(currToken);
            default:
                System.Console.WriteLine("An error has occered when parsing type");
                return null;
        }
    }

    private List<Statement> parseStatementBlock()
    {
        List<Statement> stmts = new List<Statement>();

        expect(TokenType.OP_BRACE_O);

        while (accept(TokenType.OP_BRACE_C) == null)
        {
            stmts.Add(parseStatement());
        }

        return stmts;
    }

    //<STATEMENT> ::= <ASSIGNMENT_STATEMENT> | <BREAK_STATEMENT> | <DECLARATION_STATEMENT> | <EXPRESSION_STATEMENT> | <IF_STATEMENT> | <IO_STATEMENT> | <RETURN_STATEMENT> | <STRUCT_DECLARATION> | <WHILE_STATEMENT> | <FOR_STATEMENT>
    private Statement parseStatement()
    {
        if(!(peek(TokenType.IDENT) == null))
        {
            if(peek2(TokenType.IDENT, TokenType.OP_PAREN_O))
            {
                return parseStatementExpression();
            }
            else
            {
                foreach(TokenType op in assignOp){
                    if(peek2(TokenType.IDENT, op))
                    {
                        return parseStatementAssign();
                    }
                }
            }

        }

        switch(getTokenType())
        {
            case TokenType.KW_BREAK:
                return parseStatementBreak();
            case TokenType.KW_CONTINUE:
                return parseStatementContinue();
            case TokenType.KW_RETURN:
                return parseStatementReturn();
            case TokenType.KW_IF:
                return parseStatementIf();
            case TokenType.KW_WHILE:
                return parseStatementWhile();
            case TokenType.KW_FOR:
                return parseStatementFor();
            case TokenType.INT:
            case TokenType.FLOAT:
            case TokenType.STRING:
            case TokenType.BOOL:
                return parseStatementVariableDeclaration();
            default:
                return parseStatementExpression();
        }
    }

    //<ASSIGNMENT_STATEMENT> ::= <VARIABLE> <ASSIGNMENT_OPERATOR> <EXPRESSION> | <ARRAY_ASSIGNMENT> | <STRUCT_ASSIGNMENT>
    //<ASSIGNMENT_OPERATOR> ::= "=" | "-=" | "+=" | "*=" | "/="
    private Statement parseStatementAssign()
    {
        Token var = expect(TokenType.IDENT);
        AssignmentKind op;

        if(!(peek(TokenType.OP_ASSIGN) == null))
        {
            expect(TokenType.OP_ASSIGN);
            op = AssignmentKind.ASSIGN;
        }
        else if(!(peek(TokenType.OP_ASSIGN_DIFF) == null))
        {
            expect(TokenType.OP_ASSIGN_DIFF);
            op = AssignmentKind.DIFF;
        }
        else if(!(peek(TokenType.OP_ASSIGN_OP) == null))
        {
            expect(TokenType.OP_ASSIGN_OP);
            op = AssignmentKind.SUM;
        }
        else if(!(peek(TokenType.OP_ASSIGN_PROD) == null))
        {
            expect(TokenType.OP_ASSIGN_PROD);
            op = AssignmentKind.PROD;
        }
        else if(!(peek(TokenType.OP_ASSIGN_QUOT) == null))
        {
            expect(TokenType.OP_ASSIGN_QUOT);
            op = AssignmentKind.QUOT;
        }
        else if(!(peek(TokenType.OP_ASSIGN_REM) == null))
        {
            expect(TokenType.OP_ASSIGN_REM);
            op = AssignmentKind.REM;
        }
        else
        {
            System.Console.WriteLine("Error: {0}", getTokenType());
            return null;
        }

        Expression expr = parseExpression();
        expect(TokenType.SEMICOLON);
        
        return new StatementAssign(op, var, expr);
    }

    //<EXPRESSION> ::= <LOGICAL_OR>
    private Expression parseExpression()
    {
        return parseExpressionOr();
    }

    //<LOGICAL_OR> ::= <LOGICAL_AND> | <LOGICAL_OR> "||" <LOGICAL_AND>
    private Expression parseExpressionOr()
    {
        Expression left = parseExpressionAnd();

        while(true)
        {
            if(!(accept(TokenType.OP_L_OR) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_L_OR, left, parseExpressionAnd());
            }
            else
            {
                break;
            }
        }
        return left;
    }

    //<LOGICAL_AND> ::= <RELATION_GROUP1> | <LOGICAL_AND> "&&" <RELATION_GROUP1>
    private Expression parseExpressionAnd()
    {
        Expression left = parseExpressionRelGroup1();

        while(true)
        {
            if(!(accept(TokenType.OP_L_AND) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_L_AND, left, parseExpressionRelGroup1());
            }
            else
            {
                break;
            }
        }
        return left;
    }

    //<RELATION_GROUP1> ::= <RELATION_GROUP2> | <RELATION_GROUP1> <RELATIONAL_OPERATOR_GROUP1> <RELATION_GROUP2>
    //<RELATIONAL_OPERATOR_GROUP1> ::= "==" | "!="
    private Expression parseExpressionRelGroup1()
    {
        Expression left = parseExpressionRelGroup2();

        while(true)
        {
            if(!(accept(TokenType.OP_EQ) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_L_EQ, left, parseExpressionRelGroup2());
            }
            else if(!(accept(TokenType.OP_L_NEQ) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_L_NEQ, left, parseExpressionRelGroup2());
            }
            else
            {
                break;
            }
        }
        return left;
    }

    //<RELATION_GROUP2> ::= <ADDITION> | <RELATION_GROUP2> <RELATIONAL_OPERATOR_GROUP2> <ADDITION>
    //<RELATIONAL_OPERATOR_GROUP2> ::= "<" | ">" | "<=" |  ">="
    private Expression parseExpressionRelGroup2()
    {
        Expression left = parseAddition();

        while(true)
        {
            if(!(accept(TokenType.OP_G) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_G, left, parseAddition());
            }
            else if(!(accept(TokenType.OP_L) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_L, left, parseAddition());
            }
            else if(!(accept(TokenType.OP_G_EQ) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_G_EQ, left, parseAddition());
            }
            else if(!(accept(TokenType.OP_L_EQ) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_L_EQ, left, parseAddition());
            }
            else
            {
                break;
            }
        }
        return left;
    }

    //<ADDITION> ::= <MULTIPLICATION> | <ADDITION> <ADDITION_OPERATOR> <MULTIPLICATION>
    //<ADDITION_OPERATOR> ::= "+" | "-"
    private Expression parseAddition()
    {
        Expression left = parseMultiplication();

        while(true)
        {
            if(!(accept(TokenType.OP_ADD) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_ADD, left, parseMultiplication());
            }
            else if(!(accept(TokenType.OP_SUB) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_SUB, left, parseMultiplication());
            }
            else
            {
                break;
            }
        }
        return left;
    }

    //<MULTIPLICATION> ::= <UNARY> | <MULTIPLICATION> <MULTIPLICATION_OPERATOR> <UNARY>
    //<MULTIPLICATION_OPERATOR> ::= "*" | "/" | "%"
    private Expression parseMultiplication()
    {
        Expression left = parseUnary();

        while(true)
        {
            if(!(accept(TokenType.OP_MULT) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_MULT, left, parseUnary());
            }
            else if(!(accept(TokenType.OP_DIV) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_DIV, left, parseUnary());
            }
            else if(!(accept(TokenType.OP_MOD) == null))
            {
                left = new ExpressionBinary(BinaryOpKind.OP_MOD, left, parseUnary());
            }
            else
            {
                break;
            }
        }
        return left;
    }

    //<UNARY> ::= <PRIMARY> | <UNARY_OPERATOR> <UNARY>
    //<UNARY_OPERATOR> ::= "+" | "-" | "!" | "++" | "--"

    private Expression parseUnary()
    {
        if(peek(TokenType.OP_L_NOT) != null)
        {
            return parseUnaryOperator();
        }
        else if (peek(TokenType.OP_PRE_INC) != null)
        {
            return parseUnaryOperator();
        }
        else if (peek(TokenType.OP_PRE_DEC) != null)
        {
            return parseUnaryOperator();
        }
        else
        {
            return parseExpressionTerm();
        }

    }

    private Expression parseUnaryOperator()
    {
        if(!(accept(TokenType.OP_L_NOT) == null))
        {
            if(!(peek(TokenType.OP_L_NOT) == null))
            {
                return new ExpressionUnary(UnaryOpKind.OP_L_NOT, parseUnaryOperator());
            }
            else
            {
                return new ExpressionUnary(UnaryOpKind.OP_L_NOT, parseExpressionTerm());
            }
        }
        else if(!(accept(TokenType.OP_PRE_INC) == null))
        {
            expect(TokenType.OP_PRE_INC);
            return new ExpressionUnary(UnaryOpKind.OP_PRE_INC, parseExpressionTerm());
        }
        else if(!(accept(TokenType.OP_PRE_DEC) == null))
        {
            expect(TokenType.OP_PRE_DEC);
            return new ExpressionUnary(UnaryOpKind.OP_PRE_DEC, parseExpressionTerm());
        }

        return parseUnaryOperator();
    }

    //<PRIMARY> ::= <REAL_NUMBER> | <VARIABLE> | <ARRAY> | <FUNCTION_CALL> | <STRING> | <BOOLEAN>
    private Expression parseExpressionTerm()
    {
        Token value;

        if(!(peek(TokenType.IDENT) == null))
        {
            if(peek2(TokenType.IDENT, TokenType.OP_PAREN_O))
            {
                return parseFunctionCall();
            }
            else
            {
                value = expect(TokenType.IDENT);
                return new ExpressionVariable(value);
            }
        }

        switch(getTokenType())
        {
            case TokenType.LIT_INT:
                value = expect(TokenType.LIT_INT);
                return new ExpressionConstant(value);
            case TokenType.LIT_STR:
                value = expect(TokenType.LIT_STR);
                return new ExpressionConstant(value);
            case TokenType.LIT_BOOL:
                value = expect(TokenType.LIT_BOOL);
                return new ExpressionConstant(value);
            case TokenType.LIT_FLOAT:
                value = expect(TokenType.LIT_FLOAT);
                return new ExpressionConstant(value);
            default:
                return null;
        }
    }

    private Statement parseStatementExpression()
    {
        Expression expression = parseExpression();
        expect(TokenType.SEMICOLON);
        return new StatementExpression(expression);
    }

    private Expression parseFunctionCall()
    {
        Token name = expect(TokenType.IDENT);
        expect(TokenType.OP_PAREN_O);
        List<Expression> arguments = parseArguments();
        expect(TokenType.OP_PAREN_C);
        expect(TokenType.SEMICOLON);
        return new ExpressionFunctionCall(name, arguments);
    }

    private List<Expression> parseArguments()
    {
        List<Expression> arguments = new List<Expression>();
        if(peek(TokenType.OP_PAREN_C) != null)
        {
            accept(TokenType.OP_PAREN_C);
            return arguments;
        }
        else
        {
            arguments.Add(parseExpression());
        }

        while (accept(TokenType.OP_PAREN_C) == null)
        {
            expect(TokenType.OP_COMMA);
            arguments.Add(parseExpression());
        }

        return arguments;
    }

    private Statement parseStatementBreak()
    {
        Token breakToken = expect(TokenType.KW_BREAK);
        expect(TokenType.SEMICOLON);
        return new StatementBreak(breakToken);
    }

    private Statement parseStatementContinue()
    {
        Token breakToken = expect(TokenType.KW_CONTINUE);
        expect(TokenType.SEMICOLON);
        return new StatementContinue(breakToken);
    }

    private Statement parseStatementReturn()
    {
        Token returnKW = expect(TokenType.KW_RETURN);
        Expression value = getTokenType() == TokenType.SEMICOLON ? null : parseExpression();
        expect(TokenType.SEMICOLON);
        return new StatementReturn(returnKW, value);
    }

    private Statement parseStatementIf()
    {
        expect(TokenType.KW_IF);
        expect(TokenType.OP_PAREN_O);
        Expression condition = parseExpression();
        expect(TokenType.OP_PAREN_C);
        Body body = new Body(parseStatementBlock());
        List<StatementElif> elif = new List<StatementElif>();
        Body elseBody = null;

        while(!(accept(TokenType.KW_ELSE) == null))
        {
            if(!(accept(TokenType.KW_IF) == null))
            {
                elif.Add(parseStatementElif());
            }
            else
            {
                elseBody = new Body(parseStatementBlock());
            }
        }

        return new StatementIf(condition, body, elif, elseBody);
    }

    private StatementElif parseStatementElif()
    {
        expect(TokenType.OP_PAREN_O);
        Expression elifCondition = parseExpression();
        expect(TokenType.OP_PAREN_C);
        expect(TokenType.OP_BRACE_O);
        List<Statement> elifBody = parseStatementBlock();
        expect(TokenType.OP_BRACE_C);
        return new StatementElif(elifCondition, elifBody);
    }

    private Statement parseStatementWhile()
    {
        expect(TokenType.KW_WHILE);
        expect(TokenType.OP_PAREN_O);
        Expression condition = parseExpression();
        expect(TokenType.OP_PAREN_C);
        Body body = new Body(parseStatementBlock());
        return new StatementWhile(condition, body);
    }

    private Statement parseStatementFor()
    {
        expect(TokenType.KW_FOR);
        expect(TokenType.OP_PAREN_O);
        Statement initialization = parseStatementVariableDeclaration();
        expect(TokenType.SEMICOLON);
        Expression condition = parseExpression();
        expect(TokenType.SEMICOLON);
        Expression increment = parseUnary();
        expect(TokenType.OP_PAREN_C);
        Body body = new Body(parseStatementBlock());
        return new StatementFor(initialization, condition, increment, body);
    }

    private Statement parseStatementVariableDeclaration()
    {
        Type type = parseType();
        Token name;
        Statement statement = null;
        
        if(peek2(TokenType.IDENT, TokenType.OP_ASSIGN))
        {
            name = expect(TokenType.IDENT);
            offset--;
            statement = parseStatementAssign();
        }
        else if(peek2(TokenType.IDENT, TokenType.OP_ASSIGN_DIFF))
        {
            name = expect(TokenType.IDENT);
            offset--;
            statement = parseStatementAssign();
        }
        else if(peek2(TokenType.IDENT, TokenType.OP_ASSIGN_OP))
        {
            name = expect(TokenType.IDENT);
            offset--;
            statement = parseStatementAssign();
        }
        else if(peek2(TokenType.IDENT, TokenType.OP_ASSIGN_PROD))
        {
            name = expect(TokenType.IDENT);
            offset--;
            statement = parseStatementAssign();
        }
        else if(peek2(TokenType.IDENT, TokenType.OP_ASSIGN_QUOT))
        {
            name = expect(TokenType.IDENT);
            offset--;
            statement = parseStatementAssign();
        }
        else if(peek2(TokenType.IDENT, TokenType.OP_ASSIGN_REM))
        {
            name = expect(TokenType.IDENT);
            offset--;
            statement = parseStatementAssign();
        }
        else
        {
            name = expect(TokenType.IDENT);
            offset--;
            name = expect(TokenType.SEMICOLON);
        }
        
        return new StatementVariableDeclaration(type, name, statement);
    }
}
