using System;
using System.Collections.Generic;
using lab2._3.src.Tokens;
using lab2._3.src.Lexer;
using lab2._4.src.Nodes;
using lab2._4.src.Tokens;

namespace lab2._3.src.Parser
{
    class Parser
    {
        public Scanner sc;
        public Token currentToken;

        public Parser(Scanner sc)
        {
            this.sc = sc;
            this.currentToken = sc.NextToken();
        }

        private void Advance()
        {
            currentToken = sc.NextToken();
        }

        private bool Match(params DomainTag[] types)
        {
            foreach (var type in types)
            {
                if (currentToken.Tag == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Consume(DomainTag type, string message)
        {

            if (currentToken.Tag == type)
            {
                var token = currentToken;
                Advance();
                return token;
            }
            if (currentToken.Tag == DomainTag.EOF) 
            {
                return currentToken;
            }
            throw new Exception($"Parse error: {message} at {currentToken.Coords}");
        }

        public Program Parse()
        {
            var blocks = new List<Block>();
            while (currentToken.Tag != DomainTag.EOF)
            {
                blocks.Add(ParseBlock());
            }
            return new Program(blocks);
        }

        private Block ParseBlock()
        {
            if (Match(DomainTag.KW_TYPE))
            {
                return ParseTypeBlock();
            }
            else if (Match(DomainTag.KW_CONST))
            {
                return ParseConstBlock();
            }
            throw new Exception("Expected type or const block");
        }

        private TypeBlock ParseTypeBlock()
        {
            var typeDefs = new List<TypeDef>();
            while (!Check(DomainTag.KW_TYPE) && !Check(DomainTag.KW_CONST) && currentToken.Tag != DomainTag.EOF)
            {
                typeDefs.Add(ParseTypeDef());
            }
            return new TypeBlock(typeDefs);
        }

        private ConstBlock ParseConstBlock()
        {
            var constDefs = new List<ConstDef>();
            while (!Check(DomainTag.KW_CONST) && !Check(DomainTag.KW_TYPE) && currentToken.Tag != DomainTag.EOF)
            {
                constDefs.Add(ParseConstDef());
            }
            return new ConstBlock(constDefs);
        }

        private TypeDef ParseTypeDef()
        {
            var ident = new TypeIdent(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);
            Consume(DomainTag.EQ, "Expected '='");
            var type = ParseType();
            Consume(DomainTag.SC, "Expected ';'");
            return new TypeDef(ident, type);
        }

        private ConstDef ParseConstDef()
        {
            var ident = new ConstantIdent(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);
            Consume(DomainTag.EQ, "Expected '='");
            var constant = ParseConstant();
            Consume(DomainTag.SC, "Expected ';'");
            return new ConstDef(ident, constant);
        }

        private lab2._4.src.Nodes.Type ParseType()
        {
            if (Match(DomainTag.ARROW))
            {
                return new TypeIdent(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);
            }
            else if (Match(DomainTag.KW_ARRAY))
            {
                return ParseArrayType();
            }
            else if (Match(DomainTag.KW_SET))
            {
                return ParseSetType();
            }
            else if (Match(DomainTag.KW_RECORD))
            {
                return ParseRecordType();
            }
            return ParseSimpleType();
        }

        private lab2._4.src.Nodes.Array ParseArrayType()
        {
            Consume(DomainTag.SLB, "Expected '['");
            var simpleTypes = new List<SimpleType> { ParseSimpleType() };
            while (Match(DomainTag.COMMA))
            {
                simpleTypes.Add(ParseSimpleType());
            }
            Consume(DomainTag.SRB, "Expected ']'");
            Consume(DomainTag.KW_OF, "Expected 'OF'");
            var type = ParseType();
            return new lab2._4.src.Nodes.Array(type, simpleTypes);
        }

        private Set ParseSetType()
        {
            Consume(DomainTag.KW_OF, "Expected 'OF'");
            var simpleType = ParseSimpleType();
            return new Set(simpleType);
        }

        private Record ParseRecordType()
        {
            var fields = new List<Field>();
            while (!Check(DomainTag.EOF) )
            {

                fields.Add(ParseField());
                if (Check(DomainTag.KW_END))
                    break;

            }

            Consume(DomainTag.KW_END, "Expected 'end'");
            return new Record(fields);
        }

        private SimpleType ParseSimpleType()
        {
            if (currentToken.Tag == DomainTag.IDENT)
            {
                return new TypeIdent(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);
            }
            else if (Match(DomainTag.LB))
            {
                var idents = new List<Ident> { new Ident(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name) };
                while (Match(DomainTag.COMMA))
                {
                    idents.Add(new Ident(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name));
                }
                Consume(DomainTag.RB, "Expected ')'");
                return new Idents(idents);
            }
            return ParseTwoConstants();
        }

        private TwoConstants ParseTwoConstants()
        {
            var first = ParseConstant();
            Consume(DomainTag.TWO_DOTS, "Expected '..'");
            var second = ParseConstant();
            return new TwoConstants(first, second);
        }

        private Field ParseField()
        {
            if (Match(DomainTag.KW_CASE))
            {
                return ParseCaseField();
            }
            return ParseSimpleField();
        }

        private SimpleFiled ParseSimpleField()
        {

            var idents = new List<Ident> { new Ident(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name) };
            while (Match(DomainTag.COMMA))
            {
                idents.Add(new Ident(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name));
            }
            Consume(DomainTag.COLON, "Expected ':'");
            var type = ParseType();

            if (Check(DomainTag.SC))
                Advance();

            return new SimpleFiled(idents, type);
        }

        private CaseFiled ParseCaseField()
        {
            var ident = new Ident(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);

            Consume(DomainTag.COLON, "Expected ':'");

            var typeIdent = new Ident(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);

            Consume(DomainTag.KW_OF, "Expected 'of'");


            var constantItems = new List<ConstantItem> { ParseConstantItem() };

            while (Match(DomainTag.SC))
            {
                constantItems.Add(ParseConstantItem());
            }
            return new CaseFiled(ident, typeIdent, constantItems);
        }

        private ConstantItem ParseConstantItem()
        {
            var constants = new List<Constant> { ParseConstant() };

            while (Match(DomainTag.COMMA))
            {
                constants.Add(ParseConstant());
            }
            Consume(DomainTag.COLON, "Expected ':'");

            Consume(DomainTag.LB, "Expected '('");

            var fields = new List<Field> { ParseField() };

            while (Match(DomainTag.SC))
            {
                fields.Add(ParseField());
            }
            Consume(DomainTag.RB, "Expected ')'");

            return new ConstantItem(constants, fields);
        }

        private Constant ParseConstant()
        {
            if (currentToken.Tag == DomainTag.INTEGER)
            {
                return new UnsignedNumber(((NumberToken)Consume(DomainTag.INTEGER, "Expected integer")).Value);
            }
            return new ConstantIdent(((IdentToken)Consume(DomainTag.IDENT, "Expected identifier")).Name);
        }

        private bool Check(DomainTag type) => currentToken.Tag == type;
    }
}
