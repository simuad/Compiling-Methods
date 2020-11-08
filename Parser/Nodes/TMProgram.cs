using System.Collections.Generic;

public class TMProgram : TMNode
{
    List<Declaration> decls;
    public TMProgram(List<Declaration> decls)
    {
        this.decls = decls;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("decls", this.decls);
    }
}