public class ExpressionUnary : Expression
{
    private UnaryOpKind op;
    private Expression value;

    public ExpressionUnary(UnaryOpKind op, Expression value)
    {
        this.op = op;
        this.value = value;
    }

    public override void printNode(ASTPrinter p)
    {
        string opString = op.ToString();
        p.printText("op", opString);
        p.print("value", value);
    }

}