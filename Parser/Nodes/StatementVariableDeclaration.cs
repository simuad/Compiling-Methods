public class StatementVariableDeclaration : Statement
{
    Type type;
    Token name;
    Statement statement;
    
    public StatementVariableDeclaration(Type type, Token name, Statement statement)
    {
        this.type = type;
        this.name = name;
        this.statement = statement;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("type", type);
        p.print("name", name);
        p.print("statement", statement);
    }
}