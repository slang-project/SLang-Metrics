// return true
unit A.Math
	pure power(x: Real, n: Integer): Real 
        require n >= 0 do
        res is 1
        if n = 0 then
            res = 1
        else
            i is 1
            while i < n do // TODO: Rewrite to for() when it will be implemented
                res := res * x
                i := i + 1
            end
        end
        return res
        ensure n = 0 && res = 1
	end

    pure sin(x: Real): Real is foreign
end

unit B.Math extend A.Math
    pi is Real.3.14
    zero is Integer.0
    oneMillion is 1_000_000

    override power(x: Real, n: Integer): Real 
        require else true do
        res = super()
        if n < 0 then
            i is n
            while i < -1 do  // TODO: Rewrite to for() when it will be implemented
                res := res / x
                i = i + 1
            end
        end
        return res
        ensure then n > 0 && res = super()
    end
end

unit A.B.C.D
    use A.Math as Math1, B.Math as Math2
end

abstract unit Object
    volume(): Real is abstract
end

unit Circle extend Object use B.Math as Math
    center is Point
    radius is Point
    init(center, radius: Point) do
        this.center := center
        this.radius := radius
    end

    override volume(): Real do
        return 2 * Math.pi * radius
    end

    invariant
        radius >=0
end

unit Rectangle extend Object use A.Math as Math is
    leftBottomCorner  is Point
    rightBottomCorner is Point
    leftTopCorner     : Point is Point.init()
    rightTopCorner    : Point is Point

    init(lB, rB, lT, rT : Point) do
        leftBottomCorner .init(lB)
        rightBottomCorner.init(rB)
        leftTopCorner    .init(lT)
        rightTopCorner   .init(rT)
    end

    override volume(): Real do
        return leftTopCorner.distance(rightTopCorner) *
                leftBottomCorner.distance(leftTopCorner)
    end
    invariant
        label: rB > lB && rT > lT &&
               lT > lB && rT > rB    // Corners are in right position
end


unit Point
    // Position
    x, y: Real

    // Constructors
    init() do
        x := 0.0
        y := 0.0
    end

    init(a, b : Real) do
        x := a
        y := b
    end

    init(p : Point) do
        x := p.x
        y := p.y
    end

    // Euclid distance to other point.
    distance(p : Point) do
        return ( Math.power(x - p.x, 2) 
               + Math.power(y - p.y, 2))
    end

    moveHor(d: Real) => x := x+d
    moveVer(d: Real) => y := y+d

    // Equality
    >(p : Point) do
        return x > p.x && y > p.y 
    end

    =(p : Point) do
        return x = p.x && y = p.y
    end

    >=(p : Point) do
        return this > p || this = p
    end

    <(p : Point) do
        return ~ this >= p
    end

    <=(p : Point) do
        return ~ this > p
    end
end
