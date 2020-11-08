public class StatementAssign : Statement
{
    AssignmentKind op;
    Token var;
    Expression value;
    
    public StatementAssign(AssignmentKind op, Token var, Expression value)
    {
        this.op = op;
        this.var = var;
        this.value = value;
    }

    public override void printNode(ASTPrinter p)
    {
        string opkind = op.ToString();
        p.printText("op", opkind);
        p.print("left", var);
        p.print("right", value);
    }
}