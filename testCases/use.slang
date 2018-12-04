// return true
unit A.B.C.D
end

unit D.C.B.A
end

unit A.Math
end

unit WorkDay
    const is
        Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
    end
end

unit Rrr use  A.B.C.D as A, D.C.B.A as D, const WorkDay
end