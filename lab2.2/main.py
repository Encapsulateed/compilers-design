from typing import Any
import abc
import parser_edsl as pe
from dataclasses import dataclass
from typing import Any
import enum
from pprint import pprint
import sys


class BaseType(enum.Enum):
    Integer = 'INTEGER'
    Real = 'REAL'
    Boolean = 'BOOLEAN'

class Type(abc.ABC):
    pass


@dataclass 
class Ident():
    name: str
    
@dataclass
class TypeIdent(Ident):
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
    sign : str

@dataclass 
class CharacterConstant(Constant):
    value: str

class Filed(abc.ABC):
    pass

@dataclass 
class SimpleFiled(Filed):
    idents: list[Ident]
    type: Type
    list: list[Filed]
    
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
class RecordBlock(Block):
    ident: Ident
    type : Type
@dataclass 
class ConstantBlock:
    const_list: list[ConstDef]
    
@dataclass 
class Program():
    blocks : list[Block]


@dataclass 
class SimpleType(Type):
    identes : list[Ident]

@dataclass
class PakedType(Type):
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

INTEGER = pe.Terminal('INTEGER', '[0-9]+', int, priority=7)
REAL = pe.Terminal('REAL', '[0-9]+(\\.[0-9]*)?(e[-+]?[0-9]+)?', float)
IDENT = pe.Terminal('IDENTIFIRE', '[A-Za-z][A-Za-z0-9]*', str)
STRING = pe.Terminal('STRING', '\'.*\'', str)
CHAR = pe.Terminal('CHAR', '\"\p{L}?\"', str)

def make_keyword(image):
    return pe.Terminal(image, image, lambda name: None, priority=10)

KW_PAKED, KW_FILE, KW_SET,KW_ARRAY,KW_RECORD,KW_TYPE,KW_END,KW_CASE,KW_OF,KW_CONST =\
map(make_keyword,'paked file set array record type end case of const'.split())

KW_INTEGER, KW_REAL, KW_BOOLEAN, KW_CHAR = \
    map(make_keyword, 'integer real boolean char'.split())

NProgram, NBlocks, NBlock  = \
map(pe.NonTerminal, 'program blocks block'.split()) 

NRecordBlock, NConstBlock = \
map(pe.NonTerminal, 'record_block const_block'.split())

NIdent, NType, NConsts, NConstDef,NConstant, NTypeIdent,NConstIdent = \
map(pe.NonTerminal,'ident type consts constantd_def constant\
                    type_ident const_ident'.split())

NSimpleType, NPakedType, NPakable = \
map(pe.NonTerminal,'simple_type paked_type pakable'.split())

NArray, NFile, NSet, NRecord = \
map(pe.NonTerminal, 'array file set record'.split())

NRepetedIdent, NTwoConstants =\
map(pe.NonTerminal, 'repeted_ident two_constants'.split())

NFields, NField, NSimpleField, NCaseField =\
map(pe.NonTerminal, 'fields field simple_field case_field')

NConstantIdems, NConstantItem = \
map(pe.NonTerminal, 'constant_items const_item'.split())

NNumberConst, NCharacterConst =\
map(pe.NonTerminal,'number_constant character_constant'.split())

NUnsigned_number, NUnsignedInt, NUnsignedFloat =\
map(pe.NonTerminal,'unsigned_number unsigned_int unsigned_float')


NProgram |= NBlocks

NBlocks |= NBlock, NBlocks, lambda blocks, block: blocks + [block]
NBlocks |= lambda: []

NBlock |= NRecord
NBlock |= NConstBlock

NRecord |= KW_TYPE, NIdent, '=', NType, ';'
NConstBlock |= KW_CONST, NConsts

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

