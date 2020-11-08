public class ExpressionConstant : Expression
{
    private Token lit;

    public ExpressionConstant(Token lit)
    {
        this.lit = lit;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("lit", lit);
    }

}