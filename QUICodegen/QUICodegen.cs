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
			var enumType = new TypeDefinition("QTool.UI", "QUI",
					TypeAttributes.AnsiClass | TypeAttributes.NotPublic | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
					 Get<object>());
		
			foreach (var type in Assembly.MainModule.GetAllTypes().ToArray()) 
			{
				if (!type.IsAbstract && type.BaseType.CanBeResolved() && type.Is<QUIPanel>())
				{
					enumType.Fields.Add(new FieldDefinition(type.Name, FieldAttributes.Static | FieldAttributes.Public | FieldAttributes.Private | FieldAttributes.InitOnly, Get<string>()));
				}
			}
			Assembly.MainModule.Types.Add(enumType);
			return true|| base.ChangeAssembly();
		}
	}
}

