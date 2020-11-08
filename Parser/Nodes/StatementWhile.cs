using System.Collections.Generic;

public class StatementWhile : Statement
{
    private Expression condition;
    private Body body;

    public StatementWhile(Expression condition, Body body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("Condition", condition);
        p.print("Body", body);
    }
}