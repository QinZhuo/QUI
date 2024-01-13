using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using QTool.Codegen;
namespace QTool.UI.Codegen
{
	public class QUICodegen : QToolCodegen
	{
		public override bool WillChange()
		{
			return IsAssembly("QUI") || ContainsAssembly("QUI");
		}

		public override bool ChangeAssembly()
		{
			Assembly.MainModule.Types.Add(new TypeDefinition("QTool.UI", "QUI",
					TypeAttributes.AnsiClass | TypeAttributes.NotPublic | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
					 Get<short>()));
			foreach (var type in Assembly.MainModule.GetAllTypes().ToArray())
			{
				if (!type.IsAbstract && type.BaseType.CanBeResolved() && type.Is<QUIPanel>())
				{
					//Log(type.Name);
				}
			}
			return true|| base.ChangeAssembly();
		}
	}
}

