using System.Collections.Generic;

public class ExpressionFunctionCall : Expression
{
    private Token name;
    private List<Expression> parameters;

    public ExpressionFunctionCall(Token name, List<Expression> parameters)
    {
        this.name = name;
        this.parameters = parameters;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("Name", name);
        p.printList("Parameters", parameters);
    }
}