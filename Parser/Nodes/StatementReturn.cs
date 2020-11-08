public class StatementReturn : Statement
{
    Token returnKW;
    Expression value;
    
    public StatementReturn(Token returnKW, Expression value)
    {
        this.returnKW = returnKW;
        this.value = value;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("return_kw", returnKW);
        p.print("value", value);
    }
}