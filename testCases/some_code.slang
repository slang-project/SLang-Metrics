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

abstract unit Object
    perimeter(): Real is abstract
    area(): Real is abstract
    volume(): Real is abstract
end

unit Ellipse extend Object use B.Math as Math
    center is Point
    radiusX is Real
    radiusY is Real

    init(center: Point,
         radiusX: Real, radiusY: Real) do
        this.center = center
        this.radiusX = radiusX
        this.radiusY = radiusY
    end

    override perimeter(): Real do
        return 2 * Math.pi * 
               Math.sqrt(
                   ( Math.power(radiusX, 2) 
                   + Math.power(radiusY, 2)) / 2
               )
    end

    override area(): Real do
        return Math.pi * radiusX * radiusY
    end

    override volume(): Real do
        return this.area()
    end

    invariant
        radius >=0
end

unit Circle extend Ellipse use B.Math as Math
    init(center: Point, radius: Real) do
        this.center := center
        this.radiusX := radius
        this.radiusY := radius
    end

    override perimeter(): Real do
        return 2 * Math.pi * radiusX
    end

    invariant
        radiusX = radiusY
end

unit Rectangle extend Object use B.Math as Math is
    leftBottomCorner  is Point
    rightBottomCorner is Point
    leftTopCorner     : Point is Point.init()
    rightTopCorner    : Point is Point

    // a is () do return leftTopCorner.distance(rightTopCorner) end
    // b is () do return leftBottomCorner.distance(leftTopCorner) end

    init(lB, rB, lT, rT : Point) do
        leftBottomCorner .init(lB)
        rightBottomCorner.init(rB)
        leftTopCorner    .init(lT)
        rightTopCorner   .init(rT)
    end

    override perimeter(): Real do
        return (a + b) * 2
    end

    override volume(): Real do
        return a * b
    end

    override volume(): Real do
        return this.area()
    end

    invariant
        label: rightBottomCorner > leftBottomCorner &&
               rightTopCorner > leftTopCorner &&
               leftTopCorner > leftBottomCorner &&
               rightTopCorner > rightBottomCorner    // Corners are in right position
end

unit Square extend Rectangle
    invariant 
        sidesAreEqual:
              leftTopCorner.distance(rightTopCorner)
            = leftBottomCorner.distance(leftTopCorner)
end


unit Point extend Object use B.Math as Math
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

    override perimeter() do
        return Math.zero
    end

    override area() do
        return perimeter()
    end

    override volume() do
        return area()
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
