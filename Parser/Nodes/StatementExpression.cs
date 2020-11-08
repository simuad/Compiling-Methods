public class StatementExpression : Statement
{
    Expression value;
    
    public StatementExpression(Expression value)
    {
        this.value = value;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("value", value);
    }
}