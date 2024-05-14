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
class RecordBlock(Block):
    ident: Ident
    type_ident: BaseType
    #type : Type
  #  after_eq: Ident
@dataclass 
class ConstantBlock:
    const_ident: Ident
    const_type: BaseType
    #const_list: list[ConstDef]
    
@dataclass 
class Program():
    blocks : list[Block]


@dataclass 
class SimpleType(Type):
    identes : list[Ident]


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
IDENT = pe.Terminal('IDENT', '[A-Za-z][A-Za-z0-9]*', str)
STRING = pe.Terminal('STRING', '\'.*\'', str)

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
STRING = pe.Terminal('STRING', '\'.*\'', str)

def make_keyword(image):
    return pe.Terminal(image, image, lambda name: None, priority=10)

KW_PAKED, KW_FILE, KW_SET,KW_ARRAY,KW_RECORD,KW_TYPE,KW_END,KW_CASE,KW_OF,KW_CONST =\
map(make_keyword,'paked file set array record type end case of const'.split())

KW_INTEGER, KW_REAL, KW_BOOLEAN, KW_CHAR = \
    map(make_keyword, 'integer real boolean char'.split())

NProgram, NBlocks, NBlock  = \
map(pe.NonTerminal, 'program blocks block'.split()) 

NProgram |= NBlocks, Program

NBlocks |= lambda: []
NBlocks |= NBlocks, NBlock , lambda blocks, block: blocks + [block]

NBlock |= NRecordBlock
NBlock |= NConstBlock

NRecordBlock |= KW_TYPE, NIdent, '=', NType, RecordBlock
NConstBlock |= KW_CONST, NConstIdent, 'is', NType, ConstantBlock

NConstIdent |= NIdent
NTypeIdent |= IDENT
NIdent |= IDENT

NType |= KW_INTEGER, lambda: BaseType.Integer
NType |= KW_REAL, lambda: BaseType.Real
NType |= KW_BOOLEAN, lambda: BaseType.Boolean




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

