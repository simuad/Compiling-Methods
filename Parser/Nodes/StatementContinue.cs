public class StatementContinue : Statement
{
    Token token;
    
    public StatementContinue(Token token)
    {
        this.token = token;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("Continue", token);
    }
}