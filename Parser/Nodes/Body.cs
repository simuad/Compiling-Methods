using System.Collections.Generic;

public class Body : TMNode
{
    private List<Statement> statements;

    public Body(List<Statement> statements)
    {
        this.statements = statements;
    }

    public override void printNode(ASTPrinter p)
    {
        p.printList("Statement", statements);
    }
}