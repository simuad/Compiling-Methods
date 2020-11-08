public class ExpressionVariable : Expression
{
    private Token name;

    public ExpressionVariable(Token name)
    {
        this.name = name;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("name", name);
    }

}