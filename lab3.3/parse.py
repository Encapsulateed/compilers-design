from typing import Any
import abc
import parser_edsl as pe
from dataclasses import dataclass
from typing import Any
from pprint import pprint
import sys
import re
import semantic_erors as se
import maps as my_map



class SemanticError(pe.Error, abc.ABC):
    pass

class Type(abc.ABC):
    pass

@dataclass 
class Ident():
    name: str
    name_pos : pe.Position

    @pe.ExAction
    def create(attrs, coords, res_coord):
        name = attrs
        return Ident(name,coords[0].start)
    
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
    
    def check(self):
        ids = []
        
        for id in self.idents:
            if id.name in ids:
                raise se.RepeatedVariable(id.name_pos,id.name[0]) 
            ids.append(id.name)
        
        
        type = self.type 
        if isinstance(type, Ident):
            type_name = type.name[0]
            if not type_name in my_map.TYPE_IDENTS.keys():
                raise se.UndeclaredType(type.name_pos,type_name)
        else:
            self.type.check()

    
@dataclass 
class ConstItem():
    constants: list[Constant]
    fileds: list[Filed]

    def check(self):
        for c in self.constants:
            # значит это не конкретная констан
            # проверяем, что она определена ранее 
            if isinstance(c,Ident):
                const_name = c.name[0]
                if not const_name in my_map.CONSTANT_IDENTS.keys():
                    raise se.UndeclaredConstant(c.name_pos,const_name)
            else:
                c.check()
        for f in self.fileds:
            f.check()

@dataclass
class CaseField(Filed):
    ident: Ident
    type_ident: TypeIdent
    constants : list[ConstItem]

    def check(self):
        for c in self.constants:
            c.check()
        
        if self.ident.name[0]  in my_map.TYPE_IDENTS:
            raise se.RepeatedType(self.ident.name_pos,self.ident.name[0]) 

        if self.type_ident.name[0] not in my_map.TYPE_IDENTS:
            raise se.UndeclaredType(self.type_ident.name_pos,self.type_ident.name[0]) 
        
@dataclass
class PakedType(Type):
    pass

@dataclass 
class Record(PakedType):
    fields : list[Filed]
    def check(self):
        for f in self.fields:
            f.check()
                    

    

@dataclass 
class Block(abc.ABC):
    pass

@dataclass 
class ConstDef():
    ident: Ident
    value: Constant


    def check(self):
        id : Ident = self.ident
        val : Constant = self.value

        const_name = id.name[0]
        const_val = None
        if isinstance(val,UnsignedInt):
                const_val = val.value
        if isinstance(val,UnsignedFloat):
                const_val = val.value

        if const_name in my_map.CONSTANT_IDENTS.keys():
            raise se.RepeatedConstant(id.name_pos,const_ident=const_name)
        my_map.CONSTANT_IDENTS[const_name] = const_val


@dataclass 
class TypeDef():
    ident: Ident
    type: Type
    
    @pe.ExAction
    def create(attrs, coord,res):
        id,type = attrs
        # значит задали перечислимый тип => надо определить константы

        return TypeDef(id,type)
        

    def check(self):
        id: Ident = self.ident

        type_name = id.name[0]
        
        if type_name in my_map.TYPE_IDENTS.keys():
            raise se.RepeatedType(id.name_pos,id.name[0])

        my_map.TYPE_IDENTS[type_name] = 0 
        self.type.check()


        if isinstance(self.type, Idents):
            ident_lst = self.type.idents

            for id in ident_lst:
                if id.name[0] in my_map.CONSTANT_IDENTS.keys():
                    raise se.RepeatedConstant(id.name_pos,id.name[0])
        

            for i in range(len(ident_lst)):
                    my_map.CONSTANT_IDENTS[ident_lst[i].name[0]] = i


@dataclass 
class RecordBlock(Block):
    types: list[TypeDef]
    def check(self):
        for td in self.types:
            td.check()
    
    
@dataclass 
class ConstantBlock(Block):
    const_list: list[ConstDef]
    def check(self):
        for cd in self.const_list:
            cd.check()
    
@dataclass 
class Program():
    blocks : list[Block]

    def check(self):
        for block in self.blocks:
            block.check()


@dataclass 
class SimpleType(Type):
    pass

@dataclass
class Array(PakedType):
    simple_types : list[SimpleType]
    arr_type: Type


    def check(self):
        for st in self.simple_types:
            if (isinstance(st,Ident)):
                type_name = st.name[0]
                if not type_name in my_map.TYPE_IDENTS.keys():
                    raise se.UndeclaredType(st.name_pos,type_name)
            elif isinstance(st,TwoConstants):
                st.check()
            else :
                self.simple_type.check()

        if isinstance(self.arr_type,Ident):
            type_name = self.arr_type.name[0]
            if type_name not in my_map.TYPE_IDENTS.keys():
                raise se.UndeclaredType(self.arr_type.name_pos,type_name)
        else:
            self.arr_type.check()
        

@dataclass 
class Set(PakedType):
    simple_type: SimpleType
    
    def check(self):
        st = self.simple_type
        if (isinstance(st,Ident)):
            type_name = st.name[0]
            if not type_name in my_map.TYPE_IDENTS.keys():
                raise se.UndeclaredType(st.name_pos,type_name)
        elif isinstance(st,TwoConstants):
            st.check()
        else :
            self.simple_type.check()

@dataclass
class File(PakedType):
    type: Type

@dataclass 
class TwoConstants(SimpleType):
    fistr: Constant
    second: Constant

    def check(self):
        st = self
        if isinstance(st.fistr,Ident):
            f_name = st.fistr.name[0]
            if not f_name in my_map.CONSTANT_IDENTS.keys():
                raise se.UndeclaredConstant(st.fistr.name_pos,f_name) 
        if isinstance(st.second,Ident):
            f_name = st.second.name[0]
            if not f_name in my_map.CONSTANT_IDENTS.keys():
                raise se.UndeclaredConstant(st.second.name_pos,f_name) 

@dataclass 
class Idents(SimpleType):
    idents: list[Ident]

    def check(self):
        idents = []
        for id in self.idents:
            if id.name in idents:
                raise se.RepeatedVariable(id.name_pos,id.name[0])
            idents.append(id.name)

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


NTypeDef |= NIdent, '=', NType, ';', TypeDef.create
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
NConstIdent |= IDENT, ConstIdent.create
NTypeIdent |= IDENT, TypeIdent.create
NIdent |= IDENT, Ident.create
NpointerIdent |= IDENT, Pointer.create

p = pe.Parser(NProgram)

assert p.is_lalr_one()

p.add_skipped_domain('\\s')
p.add_skipped_domain('(\\(\\*|\\{).*?(\\*\\)|\\})')


if True:
    with open('tree.txt', 'w',encoding="utf8") as f:
        original_stdout = sys.stdout
        try:
            sys.stdout = f
            for filename in sys.argv[1:]:
                try:
                    with open(filename) as f:
                        tree = p.parse(f.read())
                        pprint(tree)
                except pe.Error as e:
                    print(f'Ошибка {e.pos}: {e.message}')

        finally:
            sys.stdout = original_stdout

for filename in sys.argv[1:]:
    try:
        with open(filename) as f:
            tree = p.parse(f.read())
            #pprint(tree)
            tree.check()
    except pe.Error as e:
        print(f'Ошибка {e.pos}: {e.message}')

#print(my_map.ALL_IDENTS)
print(my_map.TYPE_IDENTS)
print(my_map.CONSTANT_IDENTS)

#print(my_map.POINTER_IDENTS)