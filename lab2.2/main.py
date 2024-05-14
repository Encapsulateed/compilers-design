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

NProgram |= NBlocks

NBlocks |= NBlock, NBlocks, lambda blocks, block: blocks + [block]
NBlocks |= lambda: []

NBlock |= NRecord
NBlock |= NConstBlock

NRecord |= KW_TYPE, NIdent, '=', NType, ';'
NConstBlock |= KW_CONST, NConsts

NConsts |= NConstDef, NConsts, lambda const_def, consts: consts + [const_def]
NConsts |= lambda: []
NConstDef |= NIdent, '=' , NConstant 

NType |= NSimpleType
NType |= '^' , NTypeIdent
NType |= NPakedType
NType |= NPakable

NPakedType|= KW_PAKED, NPakable

NPakable |= NArray
NPakable |= NFile
NPakable |= NSet
NPakable |= NRecord

NArray |= KW_ARRAY, '[', NSimpleType, NSimpleTypes, ']' , KW_OF, NType 

NSimpleTypes |= ',',NSimpleType, NSimpleTypes
NSimpleTypes |= lambda: []

NFile |= KW_FILE, KW_OF, NType

NSet |= KW_SET, KW_OF, NSimpleType

NRecord |= KW_RECORD, NFields, KW_END

NSimpleType |= NTypeIdent
NSimpleType |= NRepetedIdent
NSimpleType |= NTwoConstants

NTwoConstants |= NConstant, '..', NConstant

NFields |= lambda: []
NFields |= NField, NFields

NField |= NSimpleField
NField |= NCaseField

NSimpleField |= NRepetedIdent, ':', NType
NSimpleField |= NRepetedIdent, ':', NType, ';', NFields

NCaseField |= KW_CASE, NIdent, ':', NTypeIdent, KW_OF, NConstantItems

NConstantItems |= NConstantItem, NConstantItems
NConstantItems |= lambda: []

NConstantItem |= NConstant, NConstants, ':', '(',NFields,')'

NConstants |= ',', NConstant, NConstants
NConstants |= lambda: []

NConstant |= NNumberConst
NConstant |= NCharacterConst

NCharacterConst |= '\'', STRING, '\''

NNumberConst |= NSign, NConstIdent 
NNumberConst |= NSign, NUnsigned_number

NSign |= '+'
NSign |= '-'
NSign |= lambda:[]

NConstIdent |= NIdent

NUnsigned_number |= NUnsignedInt
NUnsigned_number |= NUnsignedFloat

NUnsignedInt |= INTEGER
NUnsignedFloat |= NUnsignedInt, '.', INTEGER, NSign, NUnsignedInt
NUnsignedFloat |= NUnsignedInt, 'E', NSign, NUnsignedInt



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

