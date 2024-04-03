package main

import (
	"bytes"
	"fmt"
	"go/ast"
	"go/format"
	"go/parser"
	"go/token"
	"os"
	"sort"
	"strconv"
)

var (
	constantCounter int
	constantMap     map[string]string
)

type KeyValue struct {
	Key   string
	Value int
}

func main() {

	if len(os.Args) != 2 {
		fmt.Printf("usage: transform <filename.go>\n")
		return
	}

	constantCounter = 0
	constantMap = make(map[string]string)

	// Создаем хранилище данных об исходных файлах
	fset := token.NewFileSet()

	// Вызываем парсер
	if file, err := parser.ParseFile(
		fset,                 // данные об исходниках
		os.Args[1],           // имя файла с исходником программы
		nil,                  // пусть парсер сам загрузит исходник
		parser.ParseComments, // приказываем сохранять комментарии
	); err == nil {
		transform(file)
		// Восстановление исходного текста программы
		if err := printFile(file); err != nil {
			fmt.Printf("Error: %v", err)
		}
	} else {
		// в противном случае, выводим сообщение об ошибке
		fmt.Printf("Error: %v", err)
	}
}

// Функция для преобразования синтаксического дерева
func transform(file *ast.File) {
	iota_map := map[string]int{}

	ast.Inspect(file, func(node ast.Node) bool {
		// Для каждого узла дерева
		if lit, ok := node.(*ast.GenDecl); ok {
			var IOTA_FLAG bool = false
			for _, spec := range lit.Specs {

				if vSpec, ok := spec.(*ast.ValueSpec); ok {
					for _, Expr := range vSpec.Values {
						if ident, ok := Expr.(*ast.Ident); ok {
							if ident.Name == "iota" {

								IOTA_FLAG = true
							}
						}
					}
					if IOTA_FLAG {
						for _, ident := range vSpec.Names {
							obj := ident.Obj
							if obj == nil {
								continue
							}
							if val, ok := obj.Data.(int); ok {

								vSpec.Values = nil
								vSpec.Values = append(vSpec.Values, &ast.BasicLit{
									Kind:  token.INT,
									Value: strconv.Itoa(obj.Data.(int)),
								})
								iota_map[obj.Name] = val

							}
						}

					}
				}

			}

		}

		return true
	})

	// Получаем список имен констант из карты
	var constNames []string
	for name := range constantMap {
		constNames = append(constNames, name)
	}

	// Сортируем имена констант
	sort.Strings(constNames)

	// Добавляем объявления констант в начало файла
	constDeclList := make([]ast.Decl, 0)
	for name, value := range constantMap {
		constDecl := &ast.GenDecl{
			Tok: token.CONST,
			Specs: []ast.Spec{
				&ast.ValueSpec{
					Names:  []*ast.Ident{ast.NewIdent(name)},
					Values: []ast.Expr{&ast.BasicLit{Kind: token.STRING, Value: value}},
				},
			},
		}
		constDeclList = append(constDeclList, constDecl)
	}
	file.Decls = append(constDeclList, file.Decls...)
}

// Функция для восстановления исходного текста программы
func printFile(file *ast.File) error {
	// Создаем буфер для записи
	var buf bytes.Buffer
	// Записываем в буфер исходный текст программы
	if err := format.Node(&buf, token.NewFileSet(), file); err != nil {
		return err
	}
	// Выводим буфер
	fmt.Println(buf.String())
	return nil
}
