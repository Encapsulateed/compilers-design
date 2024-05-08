from typing import Any
import abc
import parser_edsl as pe
from dataclasses import dataclass
from pprint import pprint
from typing import Any
import enum

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
class Block():
    ident: Ident
    type : Type
    
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

class RecordField(abc.ABC):
    pass

@dataclass 
class Filed(RecordField):
    idents: list[Ident]
    type: Type
    list: list[RecordField]
    

@dataclass
class CaseField(RecordField):
    pass

@dataclass 
class Record(PakedType):
    fields : list[RecordField]
    

