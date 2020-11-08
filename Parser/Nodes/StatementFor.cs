using System.Collections.Generic;

public class StatementFor : Statement
{
    private Statement initialization;
    private Expression condition;
    private Expression increment;
    private Body body;

    public StatementFor(Statement initialization, Expression condition, Expression increment, Body body)
    {
        this.initialization = initialization;
        this.condition = condition;
        this.increment = increment;
        this.body = body;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("initialization", initialization);
        p.print("condition", condition);
        p.print("increment", increment);
        p.print("Body", body);
    }
}