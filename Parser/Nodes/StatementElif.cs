using System.Collections.Generic;

public class StatementElif : Statement
{
    private Expression condition;
    private List<Statement> body;

    public StatementElif(Expression condition, List<Statement> body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("Condition", condition);
        p.printList("Body", body);
    }
}