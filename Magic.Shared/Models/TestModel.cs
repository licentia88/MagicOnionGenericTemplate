using Generator.Equals;
using MemoryPack;

namespace Magic.Shared.Models;

[Equatable]
[MemoryPackable]
public partial class TestModel
{
	public int Id { get; set; }

	public string Description { get; set; }

}

