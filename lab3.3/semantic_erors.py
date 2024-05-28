from typing import Any
import abc
import parser_edsl as pe
from dataclasses import dataclass
from typing import Any
import enum
from pprint import pprint
import sys
import re

class SemanticError(pe.Error):
    pass

class RepeatedVariable(SemanticError):
    def __init__(self, pos, ident):
        self.pos = pos
        self.ident = ident

    @property
    def message(self):
        return f'Повторная переменная {self.ident}'
    
class RepeatedConstant(SemanticError):
    def __init__(self, pos, const_ident):
        self.pos = pos
        self.ident = const_ident

    @property
    def message(self):
        return f'Повторная константа {self.ident}'

class RepeatedType(SemanticError):
    def __init__(self, pos, type_ident):
        self.pos = pos
        self.ident = type_ident

    @property
    def message(self):
        return f'Повторное объявление типа {self.ident}'

class UndeclaredType(SemanticError):
    def __init__(self, pos, type_ident):
        self.pos = pos
        self.ident = type_ident

    @property
    def message(self):
        return f'Указанный тип не объявлен {self.ident}'
    
class UndeclaredConstant(SemanticError):
    def __init__(self, pos, const_ident):
        self.pos = pos
        self.ident = const_ident

    @property
    def message(self):
        return f'Указанная константа не объявлена {self.ident}'