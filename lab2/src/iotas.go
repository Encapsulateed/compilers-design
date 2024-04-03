package main

type Weekday int

const (
	W Weekday = iota
	T
)
const (
	A = iota
	B
	C
	D
	E
)

func main() {
	day := W
	if day == W {
		print("Monday")
	}
	if day == T {
		print("Thursday")
	}
}
