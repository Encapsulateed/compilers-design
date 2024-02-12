% Лабораторная работа № 1.1. Раскрутка самоприменимого компилятора
% 8 февраля 2024 г.
% Митрошкин Алексей, ИУ9-61Б

# Цель работы
Целью данной работы является ознакомление с раскруткой самоприменимых компиляторов
на примере модельного компилятора.

# Индивидуальный вариант
Компилятор BeRo. Добавить синонимы ~, & и | для операторов not, and и or соответственно.
При этом операторы not, and и or остаются допустимыми.

# Реализация
Различие между файлами `btpc1.pas` и `btpc2.pas`:
```diff
--- btpc1.pas   2020-02-15 14:28:08.000000000 +0300
+++ btpc2.pas   2024-02-08 14:28:47.736887200 +0300
@@ -747,6 +747,15 @@
  end else if CurrentChar='*' then begin
   ReadChar;
   CurrentSymbol:=TokMul;
+  end else if CurrentChar='&' then begin
+    ReadChar;
+    CurrentSymbol:=SymAND;
+  end else if CurrentChar='|' then begin
+    ReadChar;
+    CurrentSymbol:=SymOR;
+  end else if CurrentChar='~' then begin
+    ReadChar;
+    CurrentSymbol:=SymNOT;
  end else if CurrentChar='(' then begin
   ReadChar;
   if CurrentChar='*' then begin
```
Различие между файлами `btpc2.pas` и `btpc3.pas`:
```diff
--- btpc2.pas   2024-02-08 14:28:47.736887200 +0300
+++ btpc3.pas   2024-02-08 14:38:16.287029800 +0300
@@ -593,21 +593,21 @@
 var Num:integer;
 begin
  Num:=0;
- if ('0'<=CurrentChar) and (CurrentChar<='9') then begin
-  while ('0'<=CurrentChar) and (CurrentChar<='9') do begin
+ if ('0'<=CurrentChar) & (CurrentChar<='9') then begin
+  while ('0'<=CurrentChar) & (CurrentChar<='9') do begin
    Num:=(Num*10)+(ord(CurrentChar)-ord('0'));
    ReadChar;
   end;
  end else if CurrentChar='$' then begin
   ReadChar;
-  while (('0'<=CurrentChar) and (CurrentChar<='9')) or
-        (('a'<=CurrentChar) and (CurrentChar<='f')) or
-        (('A'<=CurrentChar) and (CurrentChar<='F')) do begin
-   if ('0'<=CurrentChar) and (CurrentChar<='9') then begin
+  while (('0'<=CurrentChar) & (CurrentChar<='9')) or
+        (('a'<=CurrentChar) & (CurrentChar<='f')) or
+        (('A'<=CurrentChar) & (CurrentChar<='F')) do begin
+   if ('0'<=CurrentChar) & (CurrentChar<='9') then begin
     Num:=(Num*16)+(ord(CurrentChar)-ord('0'));
-   end else if ('a'<=CurrentChar) and (CurrentChar<='f') then begin
+   end else if ('a'<=CurrentChar) & (CurrentChar<='f') then begin
     Num:=(Num*16)+(ord(CurrentChar)-ord('a')+10);
-   end else if ('A'<=CurrentChar) and (CurrentChar<='F') then begin
+   end else if ('A'<=CurrentChar) & (CurrentChar<='F') then begin
     Num:=(Num*16)+(ord(CurrentChar)-ord('A')+10);
    end;
    ReadChar;
@@ -621,14 +621,14 @@
     StrEnd,InStr:boolean;
     LastChar:char;
 begin
- while (CurrentChar>#0) and (CurrentChar<=' ') do begin
+ while (CurrentChar>#0) & (CurrentChar<=' ') do begin
   ReadChar;
  end;
- if (('a'<=CurrentChar) and (CurrentChar<='z')) or (('A'<=CurrentChar) and (CurrentChar<='Z')) then begin
+ if (('a'<=CurrentChar) & (CurrentChar<='z')) or (('A'<=CurrentChar) & (CurrentChar<='Z')) then begin
   k:=0;
-  while ((('a'<=CurrentChar) and (CurrentChar<='z')) or
-         (('A'<=CurrentChar) and (CurrentChar<='Z')) or
-         (('0'<=CurrentChar) and (CurrentChar<='9'))) or
+  while ((('a'<=CurrentChar) & (CurrentChar<='z')) |
+         (('A'<=CurrentChar) & (CurrentChar<='Z')) |
+         (('0'<=CurrentChar) & (CurrentChar<='9'))) |
         (CurrentChar='_') do begin
    if k<>MaximalAlfa then begin
     k:=k+1;
@@ -651,7 +651,7 @@
    end;
    s:=s+1;
   end;
- end else if (('0'<=CurrentChar) and (CurrentChar<='9')) or (CurrentChar='$') then begin
+ end else if (('0'<=CurrentChar) and (CurrentChar<='9')) | (CurrentChar='$') then begin
   CurrentSymbol:=TokNumber;
   CurrentNumber:=ReadNumber;
  end else if CurrentChar=':' then begin
@@ -689,7 +689,7 @@
   end else begin
    CurrentSymbol:=TokPeriod
   end;
- end else if (CurrentChar='''') or (CurrentChar='#') then begin
+ end else if (CurrentChar='''') | (CurrentChar='#') then begin
   CurrentStringLength:=0;
   StrEnd:=false;
   InStr:=false;
@@ -708,7 +708,7 @@
      end else begin
       InStr:=false;
      end;
-    end else if (CurrentChar=#13) or (CurrentChar=#10) then begin
+    end else if (CurrentChar=#13) | (CurrentChar=#10) then begin
      Error(100);
      StrEnd:=true;
     end else begin
@@ -799,7 +799,7 @@
   if CurrentChar='/' then begin
    repeat
     ReadChar;
-   until (CurrentChar=#10) or (CurrentChar=#0);
+   until (CurrentChar=#10) | (CurrentChar=#0);
    GetSymbol;
   end else begin
    Error(102);
@@ -838,7 +838,7 @@
   if Identifiers[j].Kind<>IdFUNC then begin
    Error(104);
   end;
-  if (Code[Identifiers[j].FunctionAddress]<>OPJmp) or (Code[Identifiers[j].FunctionAddress+1]>0) then begin
+  if (Code[Identifiers[j].FunctionAddress]<>OPJmp) | (Code[Identifiers[j].FunctionAddress+1]>0) then begin
    Error(105);
   end;
   Identifiers[j].Name[1]:='$';
@@ -863,7 +863,7 @@
    j:=Identifiers[j].Link;
   end;
   i:=i-1;
- until (i<-1) or (j<>0);
+ until (i<-1) | (j<>0);
  if j=0 then begin
   Error(106);
  end;
@@ -902,7 +902,7 @@
    StackPosition:=StackPosition+a;
   end;
  end;
- if not ((((Opcode=OPAddC) or (Opcode=OPAdjS)) and (a=0)) or ((Opcode=OPMulC) and (a=1))) then begin
+ if ~ ((((Opcode=OPAddC) | (Opcode=OPAdjS)) and (a=0)) | ((Opcode=OPMulC) and (a=1))) then begin
   if IsLabeled then begin
    Code[CodePosition]:=Opcode;
    CodePosition:=CodePosition+1;
@@ -1011,7 +1011,7 @@
      Check(TokIdent);
      j:=Types[t].Fields;
      Identifiers[0].Name:=CurrentIdentifer;
-     while not StringCompare(Identifiers[j].Name,CurrentIdentifer) do begin
+     while ~ StringCompare(Identifiers[j].Name,CurrentIdentifer) do begin
       j:=Identifiers[j].Link;
      end;
      if j=0 then begin
```


# Тестирование

Тестовый пример:

```pascal
program Hello;

begin
  WriteLn('Hello, student!');
  if ~(1=2) & (2=2) | (1=1) or(3=4) then
  begin
    WriteLn('2==2!');
  end;
end.
```

Вывод тестового примера на `stdout`

```
Hello, student!
2==2!
```

# Вывод
В ходе данной лабораторной работы, я освоил навык раскрутки 
самоприменимого компилятора для расширения возможностей
и синтаксиса целевого языка
