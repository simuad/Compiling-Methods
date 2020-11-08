using System.Collections.Generic;

public class StatementIf : Statement
{
    private Expression condition;
    private Body body;
    private List<StatementElif> elif;
    private Body elseBody;

    public StatementIf(Expression condition, Body body, List<StatementElif> elifs = null, Body elseBody = null)
    {
        this.condition = condition;
        this.body = body;
        this.elif = elifs;
        this.elseBody = elseBody;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("Condition", condition);
        p.print("Body", body);
        p.printList("ElifBody", elif);
        p.print("ElseBody", elseBody);
    }
}