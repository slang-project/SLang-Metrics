// return false
// This is an example of things that have a reduce/reduce problems, and are currently under discussion.
unit NotParsingYet
    unit SwitchCase
        use const WeekDay
        day: WeekDay is Monday 
        if day is
            Monday .. Friday: StandardIO.putString (“Work day – go to the office!\n”) 
            Saturday, Sunday: StandardIO.putString (“WeekEnd – do what you like!\n”)
        end
    end
	
    for() do
        while i in 1..n do
            res == res * x
        end
    end
	
	foo() // Not parsing routines without arguments && with require
	require
		true //, 0 < 1
	do
		a is 0
	ensure 
		true //, false || true
	end
end