using calculator.src.Tokens;
using lab2._3.src;
using lab3._1.src.Exeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator.src
{
    internal class ExpresionEvaluator
    {
        public  int EvaluateTree(INode tree)
        {
            int Traverse(INode tree)
            {
                if (tree is not InnerNode)
                {
                    int v = ((NumberToken)((Leaf)tree).tok).value;
                    return v;
                }
                else
                {
                    var node = ((InnerNode)tree);
                    switch (node.nterm)
                    {
                        case "e":
                            if (node.children.Count == 2)
                            {
                                var left = Traverse(node.children[0]);
                                var right = Traverse(node.children[1]);

                                return left + right;
                            }
                            else if (node.children.Count == 1)
                            {
                                return Traverse(node.children[0]);
                            }

                            throw new InvalidNonTermLenght(node.nterm);


                        case "e1":
                            if (node.children.Count == 3)
                            {
                                var left = Traverse(node.children[1]);
                                var right = Traverse(node.children[2]);
                                return left + right;
                            }
                            else if (node.children.Count == 2)
                            {
                                return Traverse(node.children[1]);
                            }
                            if(node.children.Count != 0)
                            {
                                throw new InvalidNonTermLenght(node.nterm);

                            }
                            break;

                        case "t":
                            if (node.children.Count == 2)
                            {
                                var left = Traverse(node.children[0]);
                                var right = Traverse(node.children[1]);

                                return left * right;
                            }
                            else if (node.children.Count == 1)
                            {
                                return Traverse(node.children[0]);
                            }
                            throw new InvalidNonTermLenght(node.nterm);

                        case "t1":
                            if (node.children.Count == 3)
                            {
                                var left = Traverse(node.children[1]);
                                var right = Traverse(node.children[2]);
                                return left * right;
                            }
                            else if (node.children.Count == 2)
                            {
                                return Traverse(node.children[1]);
                            }
                            else if (node.children.Count == 0)
                            {
                                return 1;
                            }
                            throw new InvalidNonTermLenght(node.nterm);


                        case "f":
                            if (node.children.Count == 3)
                            {
                                return Traverse(node.children[1]);
                            }
                            else if (node.children.Count == 1)
                            {
                                return Traverse(node.children[0]);

                            }
                            throw new InvalidNonTermLenght(node.nterm);
                        case "":
                            return Traverse(node.children[0]);
                        default:
                            return 0;
                    }
                }


                return 0;
            }
            int res = Traverse(tree); ;

            return res;
        }

    }
}
