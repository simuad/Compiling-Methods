public class StatementBreak : Statement
{
    Token token;
    
    public StatementBreak(Token token)
    {
        this.token = token;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("break", token);
    }
}