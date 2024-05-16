Type
  BaseColor = (red, green, blue, highlited);
  Color = set of BaseColor;
  GraphicScreen = array [1..Heigh] of array [ 1..Width] of Color;
  TextScreen = ARRAY [1..Lines] of array [1..Columns] of
    record
      Symbol : CHAR;
      SymColor : Color;
      BackColor : Color
    end;
type
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