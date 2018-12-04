// result true
val unit Integer extend Integer [Platform.IntegerBitsCount]
	init (value: as this) do
		data := value.data
	end
	init() do
		/// Default init with zero
		data := Bit [Platform.IntegerBitsCount]
	end
end