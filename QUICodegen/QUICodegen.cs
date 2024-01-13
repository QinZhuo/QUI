using System.Collections;
using System.Collections.Concurrent;
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
		public static ConcurrentQueue<string> UIs = new ConcurrentQueue<string>();
		public override bool ChangeAssembly()
		{
			foreach (var type in Assembly.MainModule.GetAllTypes().ToArray()) 
			{
				if (!type.IsAbstract && type.BaseType.CanBeResolved() && type.Is<QUIPanel>())
				{
					UIs.Enqueue(type.Name);
					
				}
			}
			var enumType = new TypeDefinition("QTool.UI", "QUI",
				TypeAttributes.AnsiClass | TypeAttributes.NotPublic | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
				 Get<object>());
			foreach (var item in UIs)
			{
				enumType.Fields.Add(new FieldDefinition(item, FieldAttributes.Static | FieldAttributes.Public | FieldAttributes.Private | FieldAttributes.InitOnly, Get<string>()));
			}
			Assembly.MainModule.Types.Add(enumType);
			return true|| base.ChangeAssembly();
		}
	}
}

