% Лабораторная работа № 2.2 «Абстрактные синтаксические деревья»
% 16 мая 2024 г.
% Алексей Митрошкин, ИУ9-61Б

# Цель работы
Целью данной работы является получение навыков составления грамматик 
и проектирования синтаксических деревьев.
# Индивидуальный вариант
Объявления типов и констант в Паскале:

В record’е точка с запятой разделяет поля и после case дополнительный end не ставится.

``` Paskal
Type
  Coords = Record x, y: INTEGER end;
Const
  MaxPoints = 100;
type
  CoordsVector = array 1..MaxPoints of Coords;

(* графический и текстовый дисплеи *)
const
  Heigh = 480;
  Width = 640;
  Lines = 24;
  Columns = 80;
type
  BaseColor = (red, green, blue, highlited);
  Color = set of BaseColor;
  GraphicScreen = array 1..Heigh of array 1..Width of Color;
  TextScreen = array 1..Lines of array 1..Columns of
    record
      Symbol : CHAR;
      SymColor : Color;
      BackColor : Color
    end;

{ определения токенов }
TYPE
  Domain = (Ident, IntNumber, RealNumber);
  Token = record
    fragment : record
      start, following : record
        row, col : INTEGER
      end
    end;
    case tokType : Domain of
      Ident : (
        name : array 1..32 of CHAR
      );
      IntNumber : (
        intval : INTEGER
      );
      RealNumber : (
        realval : REAL
      )
  end;

  Year = 1900..2050;

  List = record
    value : Token;
    next : ^List
  end;
```
# Реализация

## Абстрактный синтаксис
Программа состоит из списка блоков 
```
program -> block+
```

Блоки представляют собой либо объявление типа, либо объявление констант
```
block -> TYPE type_block | CONST const_block
```

Cам блок содержит список определений 
```
type_block -> type_def+
const_block -> const_def+
```

Определения представляют собой идентификатор и значение
```
type_def -> ident '=' type ';'
constant_def -> ident '=' constant ';' .
```

## Лексическая структура и конкретный синтаксис
Язык содержит следующие терминальные домены
```
INTEGER = pe.Terminal('INTEGER', '[0-9]+', int, priority=7)
REAL = pe.Terminal('REAL', '[0-9]+(\\.[0-9]*)?(e[-+]?[0-9]+)?', float)
IDENT = pe.Terminal('IDENT', '[A-Za-z][A-Za-z0-9]*', str)
STRING = pe.Terminal('STRING', '\'.*\'', str.upper)
```

Перейдём к конкретной грамматике 
```program -> blocks .
                                            
blocks -> block blocks.
blocks -> block .


block -> TYPE record_block .
block -> CONST const_block  .
         
record_block -> types .

const_block -> consts .

constas -> constant_def consts .
constas -> constant_def .

types -> type_def types .
types -> type_def .

type_def -> ident '=' type ';'

constant_def -> ident '=' constant ';' .

type -> simple_type .
type -> '^' poiter_ident .
type ->  paked_type .
type ->  pakable .

paked_type -> PAKED pakable .

pakable ->  array .
pakable ->  file .
pakable ->  set .
pakable ->  record .

array -> ARRAY '[' simple_types ']' OF type .

simple_types -> simple_types ',' simple_type  .

simple_types -> simple_type .

file -> FILE OF type .

set -> SET OF simple_type .

record ->  RECORD fields END .

simple_type -> type_ident .
simple_type -> '(' idents ')' .
simple_type -> two_constants .  
                   
two_constants -> constant '..' constant .

fields -> field fields .
fields -> field .

field -> idents ':' type ';' .
field -> CASE ident ':' type_ident OF constant_items .

constant_items -> const_item ';' constant_items .
constant_items -> const_item .

const_item ->  constants ':' '(' fields ')' .

constants -> constants ',' constant .
constants -> constant .


constant -> number_constant .
constant -> character_constant .


number_constant -> constant_ident .
number_constant -> unsigned_number .


character_constant -> STRING .

unsigned_number -> unsigned_int  .
unsigned_number -> unsigned_float .

             
unsigned_float -> REAL .

unsigned_int  -> INTEGER . 


idents -> ident .
idents -> idents ',' ident .
```
## Программная реализация

```python
from typing import Any
import abc
import parser_edsl as pe
from dataclasses import dataclass
from typing import Any
import enum
from pprint import pprint
import sys
import re
class BaseType(enum.Enum):
    Integer = 'INTEGER'
    Real = 'REAL'
    Boolean = 'BOOLEAN'
    String = 'STRING'

class Type(abc.ABC):
    pass

@dataclass 
class Ident():
    name: str
    
@dataclass
class TypeIdent(Ident):
    pass

@dataclass 
class Pointer(Ident):
    pass

@dataclass
class ConstIdent(Ident):
    pass


@dataclass 
class Constant(abc.ABC):
    pass

@dataclass
class UnsignedNumber(abc.ABC):
    pass

@dataclass 
class UnsignedInt(UnsignedNumber):
    value: int

@dataclass 
class UnsignedFloat(UnsignedNumber):
    value: float

@dataclass 
class NumberConstant(Constant):
    ident: Ident
    value: UnsignedNumber

@dataclass 
class CharacterConstant(Constant):
    value: str

class Filed(abc.ABC):
    pass

@dataclass 
class SimpleFiled(Filed):
    idents: list[Ident]
    type: Type

    
@dataclass 
class ConstItem():
    constants: list[Constant]
    fileds: list[Filed]

@dataclass
class CaseField(Filed):
    ident: Ident
    type_ident: TypeIdent
    constants : list[ConstItem]
@dataclass
class PakedType(Type):
    pass

@dataclass 
class Record(PakedType):
    fields : list[Filed]
    

@dataclass 
class Block(abc.ABC):
    pass

@dataclass 
class ConstDef():
    ident: Ident
    value: Constant


@dataclass 
class TypeDef():
    ident: Ident
    type: Type
@dataclass 
class RecordBlock(Block):
    types: list[TypeDef]

    
    
@dataclass 
class ConstantBlock:
    const_list: list[ConstDef]
    
@dataclass 
class Program():
    blocks : list[Block]


@dataclass 
class SimpleType(Type):
    pass

@dataclass
class Array(PakedType):
    simple_types : list[SimpleType]
    arr_type: Type

@dataclass 
class Set(PakedType):
    simple_type: SimpleType

@dataclass
class File(PakedType):
    type: Type

@dataclass 
class TwoConstants(SimpleType):
    fistr: Constant
    second: Constant

@dataclass 
class Idents(SimpleType):
    idents: list[Ident]

INTEGER = pe.Terminal('INTEGER', '[0-9]+', int, priority=7)
REAL = pe.Terminal('REAL', '[0-9]+(\\.[0-9]*)?(e[-+]?[0-9]+)?', float)
IDENT = pe.Terminal('IDENT', '[A-Za-z][A-Za-z0-9]*', str)
STRING = pe.Terminal('STRING', '\'.*\'', str)

def make_keyword(image):
    return pe.Terminal(image, image, lambda name: None,
                       re_flags=re.IGNORECASE, priority=10)

KW_PAKED, KW_FILE, KW_SET,KW_ARRAY,KW_RECORD,KW_TYPE,KW_END,KW_CASE,KW_OF,KW_CONST =\
map(make_keyword,'paked file set array record type end case of const'.split())

KW_INTEGER, KW_REAL, KW_BOOLEAN, KW_CHAR = \
    map(make_keyword, 'integer real boolean char'.split())

NProgram, NBlocks, NBlock,NpointerIdent = \
map(pe.NonTerminal, 'program blocks block poiter_ident'.split()) 

NRecordBlock, NConstBlock = \
map(pe.NonTerminal, 'record_block const_block'.split())

NIdent, NType, NConsts, NConstDef,NConstant, NTypeIdent,NConstIdent, NTypes, NTypeDef = \
map(pe.NonTerminal,'ident type consts constantd_def constant\
                    type_ident const_ident types type_def'.split())

NSimpleType, NPakedType, NPakable = \
map(pe.NonTerminal,'simple_type paked_type pakable'.split())

NArray, NFile, NSet, NRecord = \
map(pe.NonTerminal, 'array file set record'.split())

NIdents, NTwoConstants =\
map(pe.NonTerminal, 'idents two_constants'.split())

NFields, NField, NSimpleField, NCaseField =\
map(pe.NonTerminal, 'fields field simple_field case_field'.split())

NConstantItems, NConstantItem = \
map(pe.NonTerminal, 'constant_items const_item'.split())

NNumberConst, NCharacterConst =\
map(pe.NonTerminal,'number_constant character_constant'.split())

NUnsigned_number, NUnsignedInt, NUnsignedFloat =\
map(pe.NonTerminal,'unsigned_number unsigned_int unsigned_float'.split())

NSimpleTypes, NConstants,NSign =\
    map(pe.NonTerminal, 'simple_types constants sing'.split())



INTEGER = pe.Terminal('INTEGER', '[0-9]+', int, priority=7)
REAL = pe.Terminal('REAL', '[0-9]+(\\.[0-9]*)?(e[-+]?[0-9]+)?', float)
IDENT = pe.Terminal('IDENT', '[A-Za-z][A-Za-z0-9]*', str)
STRING = pe.Terminal('STRING', '\'.*\'', str.upper)

def make_keyword(image):
   return pe.Terminal(image, image, lambda name: None,
                       re_flags=re.IGNORECASE, priority=10)

KW_PAKED, KW_FILE, KW_SET,KW_ARRAY,KW_RECORD,KW_TYPE,KW_END,KW_CASE,KW_OF,KW_CONST =\
map(make_keyword,'paked file set array record type end case of const'.split())

KW_INTEGER, KW_REAL, KW_BOOLEAN, KW_CHAR = \
    map(make_keyword, 'integer real boolean char'.split())

NProgram, NBlocks, NBlock  = \
map(pe.NonTerminal, 'program blocks block'.split()) 

NProgram |= NBlocks, Program

NBlocks |= lambda: []
NBlocks |= NBlocks, NBlock , lambda blocks, block: blocks + [block]

NBlock |= KW_TYPE, NRecordBlock
NBlock |= KW_CONST, NConstBlock

NRecordBlock |= NTypes, RecordBlock
NConstBlock |=  NConsts, ConstantBlock


NTypeDef |= NIdent, '=', NType, ';', TypeDef
NConstDef |= NIdent, '=', NConstant, ';' ,  ConstDef

NType |= NSimpleType
NType |= NPakedType
NType |= NPakable
NType |= '^', NpointerIdent

NPakedType |= KW_PAKED, NPakable 


NPakable |= NArray
NPakable |= NSet
NPakable |= NFile
NPakable |= NRecord

NSet |= KW_SET, KW_OF, NSimpleType, Set
NFile |= KW_FILE, KW_OF, NType, File
NArray |= KW_ARRAY, '[', NSimpleTypes,']', KW_OF, NType, Array 

NSimpleTypes |= NSimpleType, lambda st: [st]
NSimpleTypes |= NSimpleTypes, ',' , NSimpleType , lambda sts, st: sts + [st]


NRecord |= KW_RECORD, NFields, KW_END, Record


NFields |= NField, lambda fld: [fld]
NFields |= NFields, NField, lambda flds, fld: flds + [fld]

NField |= NCaseField
NField |= NSimpleField

NSimpleField |= NIdents , ':', NType, ';', SimpleFiled
NSimpleField |= NIdents , ':', NType , SimpleFiled

NCaseField |= KW_CASE, NIdent, ':', NTypeIdent, KW_OF, NConstantItems, CaseField

NConstantItems |= NConstantItem, lambda it: [it]
NConstantItems |= NConstantItems, ';', NConstantItem, lambda its, it: its + [it]

NConstantItem |= NConstants ,':' ,'(' , NFields, ')', ConstItem


#######################################################

NSimpleType |= NTwoConstants
NSimpleType |='(',  NIdents, ')', Idents
NSimpleType |= NTypeIdent 

NConsts |= lambda:[]
NConsts |= NConsts, NConstDef, lambda cds, cd: cds+[cd]

NTypes |= lambda:[]
NTypes |= NTypes, NTypeDef, lambda types, t: types+[t]

NConstants |= NConstant, lambda c: [c]
NConstants |= NConstants, ',' , NConstant, lambda cs, c: cs + [c]

NIdents |= NIdent, lambda id: [id]
NIdents |= NIdents, ',' , NIdent, lambda ids, id: ids + [id]


NConstant |= NNumberConst
NConstant |= NCharacterConst

NNumberConst |= NConstIdent
NNumberConst |= NUnsigned_number

NUnsigned_number |= NUnsignedInt
NUnsignedFloat |= NUnsignedFloat

NUnsignedInt |= INTEGER, UnsignedInt
NUnsignedFloat |= REAL, UnsignedFloat
NCharacterConst |=  STRING, CharacterConstant

NTwoConstants |= NConstant, '..' , NConstant, TwoConstants
NConstIdent |= IDENT, ConstIdent
NTypeIdent |= IDENT, TypeIdent
NIdent |= IDENT, Ident
NpointerIdent |= IDENT, Pointer
p = pe.Parser(NProgram)

assert p.is_lalr_one()

p.add_skipped_domain('\\s')
p.add_skipped_domain('(\\(\\*|\\{).*?(\\*\\)|\\})')

for filename in sys.argv[1:]:
    try:
        with open(filename) as f:
            tree = p.parse(f.read())
            pprint(tree)
    except pe.Error as e:
        print(f'Ошибка {e.pos}: {e.message}')
    except Exception as e:
        print(e)


```

# Тестирование

## Входные данные

```
Type
  Coords = Record x, y: INTEGER end;
Const
  MaxPoints = 100;
type
  CoordsVector = array [1..MaxPoints] of Coords;

(* графический и текстовый дисплеи *)
const
  Heigh = 480;
  Width = 640;
  Lines = 24;
  Columns = 80;
type
  BaseColor = (red, green, blue, highlited);
  Color = set of BaseColor;
  GraphicScreen = array [1..Heigh] of array [1..Width] of Color;
  TextScreen = array [1..Lines] of array [1..Columns] of
    record
      Symbol : CHAR;
      SymColor : Color;
      BackColor : Color
    end;

{ определения токенов }
TYPE
  Domain = (Ident, IntNumber, RealNumber);
  Token = record
    fragment : record
      start, following : record
        row, col : INTEGER
      end
    end;
    case tokType : Domain of
      Ident : (
        name : array [1..32] of CHAR
      );
      IntNumber : (
        intval : INTEGER
      );
      RealNumber : (
        realval : REAL
      )
  end;

  Year = 1900..2050;

  List = record
    value : Token;
    next : ^List
  end;

```

## Вывод на `stdout`

<!-- ENABLE LONG LINES -->

```
Program(blocks=[RecordBlock(types=[TypeDef(ident=Ident(name='Coords'),
                                           type=Record(fields=[SimpleFiled(idents=[Ident(name='x'),
                                                                                   Ident(name='y')],
                                                                           type=TypeIdent(name='INTEGER'))]))]),
                ConstantBlock(const_list=[ConstDef(ident=Ident(name='MaxPoints'),
                                                   value=UnsignedInt(value=100))]),
                RecordBlock(types=[TypeDef(ident=Ident(name='CoordsVector'),
                                           type=Array(simple_types=[TwoConstants(fistr=UnsignedInt(value=1),
                                                                                 second=ConstIdent(name='MaxPoints'))],
                                                      arr_type=TypeIdent(name='Coords')))]),
                ConstantBlock(const_list=[ConstDef(ident=Ident(name='Heigh'),
                                                   value=UnsignedInt(value=480)),
                                          ConstDef(ident=Ident(name='Width'),
                                                   value=UnsignedInt(value=640)),
                                          ConstDef(ident=Ident(name='Lines'),
                                                   value=UnsignedInt(value=24)),
                                          ConstDef(ident=Ident(name='Columns'),
                                                   value=UnsignedInt(value=80))]),
                RecordBlock(types=[TypeDef(ident=Ident(name='BaseColor'),
                                           type=Idents(idents=[Ident(name='red'),
                                                               Ident(name='green'),
                                                               Ident(name='blue'),
                                                               Ident(name='highlited')])),
                                   TypeDef(ident=Ident(name='Color'),
                                           type=Set(simple_type=TypeIdent(name='BaseColor'))),
                                   TypeDef(ident=Ident(name='GraphicScreen'),
                                           type=Array(simple_types=[TwoConstants(fistr=UnsignedInt(value=1),
                                                                                 second=ConstIdent(name='Heigh'))],
                                                      arr_type=Array(simple_types=[TwoConstants(fistr=UnsignedInt(value=1),
                                                                                                second=ConstIdent(name='Width'))],
                                                                     arr_type=TypeIdent(name='Color')))),
                                   TypeDef(ident=Ident(name='TextScreen'),
                                           type=Array(simple_types=[TwoConstants(fistr=UnsignedInt(value=1),
                                                                                 second=ConstIdent(name='Lines'))],
                                                      arr_type=Array(simple_types=[TwoConstants(fistr=UnsignedInt(value=1),
                                                                                                second=ConstIdent(name='Columns'))],
                                                                     arr_type=Record(fields=[SimpleFiled(idents=[Ident(name='Symbol')],
                                                                                                         type=TypeIdent(name='CHAR')),
                                                                                             SimpleFiled(idents=[Ident(name='SymColor')],
                                                                                                         type=TypeIdent(name='Color')),
                                                                                             SimpleFiled(idents=[Ident(name='BackColor')],
                                                                                                         type=TypeIdent(name='Color'))]))))]),
                RecordBlock(types=[TypeDef(ident=Ident(name='Domain'),
                                           type=Idents(idents=[Ident(name='Ident'),
                                                               Ident(name='IntNumber'),
                                                               Ident(name='RealNumber')])),
                                   TypeDef(ident=Ident(name='Token'),
                                           type=Record(fields=[SimpleFiled(idents=[Ident(name='fragment')],
                                                                           type=Record(fields=[SimpleFiled(idents=[Ident(name='start'),
                                                                                                                   Ident(name='following')],
                                                                                                           type=Record(fields=[SimpleFiled(idents=[Ident(name='row'),
                                                                                                                                                   Ident(name='col')],
                                                                                                                                           type=TypeIdent(name='INTEGER'))]))])),
                                                               CaseField(ident=Ident(name='tokType'),
                                                                         type_ident=TypeIdent(name='Domain'),
                                                                         constants=[ConstItem(constants=[ConstIdent(name='Ident')],
                                                                                              fileds=[SimpleFiled(idents=[Ident(name='name')],
                                                                                                                  type=Array(simple_types=[TwoConstants(fistr=UnsignedInt(value=1),
                                                                                                                                                        second=UnsignedInt(value=32))],
                                                                                                                             arr_type=TypeIdent(name='CHAR')))]),
                                                                                    ConstItem(constants=[ConstIdent(name='IntNumber')],
                                                                                              fileds=[SimpleFiled(idents=[Ident(name='intval')],
                                                                                                                  type=TypeIdent(name='INTEGER'))]),     
                                                                                    ConstItem(constants=[ConstIdent(name='RealNumber')],
                                                                                              fileds=[SimpleFiled(idents=[Ident(name='realval')],        
                                                                                                                  type=TypeIdent(name='REAL'))])])])),   
                                   TypeDef(ident=Ident(name='Year'),
                                           type=TwoConstants(fistr=UnsignedInt(value=1900),
                                                             second=UnsignedInt(value=2050))),
                                   TypeDef(ident=Ident(name='List'),
                                           type=Record(fields=[SimpleFiled(idents=[Ident(name='value')],
                                                                           type=TypeIdent(name='Token')),
                                                               SimpleFiled(idents=[Ident(name='next')],
                                                                           type=Pointer(name='List'))]))])])
```

# Вывод
В ходе данной лабораторной работы был получен навык составления грамматик и проектирования
 синтаксических деревьев.

Самым трудным и вместе с тем интересным в данной лабораторной работе мне показалось
составление описания
грамматики языка, после этого разобраться с интерфейсом библиотеки уже не составило труда.