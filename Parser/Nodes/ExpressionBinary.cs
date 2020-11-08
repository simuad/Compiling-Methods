public class ExpressionBinary : Expression
{
    private BinaryOpKind op;
    private Expression left;
    private Expression right;

    public ExpressionBinary(BinaryOpKind op, Expression left, Expression right)
    {
        this.op = op;
        this.left = left;
        this.right = right;
    }

    public override void printNode(ASTPrinter p)
    {
        string opString = op.ToString();
        p.printText("op", opString);
        p.print("left", left);
        p.print("right", right);
    }

}