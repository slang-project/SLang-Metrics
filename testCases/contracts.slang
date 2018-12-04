var unit A.B
end

ref unit B.A
end

abstract unit C.D
end

concurrent unit D.Process

end

unit TestUnits
	use A.B as B, B.A as A, D.Process
	foo(a: A, b: B): C.D
	require
		true
	do
		None
	ensure
		true
	end

	bar()
	require
		true, 0 < 1
	do
		a is 0
	ensure
		true, false || true
	end

	invariant
		true,
		label: 42 = 42
end
