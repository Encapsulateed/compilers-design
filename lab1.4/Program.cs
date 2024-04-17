using lab1._3;
using lab1._3.src;

var lines = File.ReadAllLines("input.txt");
string prg = "";

foreach (var line in lines)
{
    prg += line + '\n';
}

var cp = new Compiler();
var sc = new Scanner(prg, cp);

Console.WriteLine(prg);

var tok = sc.NextToken();
while (tok.Tag != DomainTag.EOF)
{
  
    var st = tok.ToString();
    if (tok.Tag == DomainTag.IDENT)
    {
        st += $" IDENT NAME: {cp.GetName(((IdentToken)tok).Code)}";
    }
    Console.WriteLine(st);
    tok = sc.NextToken();

    if (tok.Tag == DomainTag.EOF)
    {
        Console.WriteLine(tok.ToString());
    }

}
Console.WriteLine();
cp.OutPutMessages();