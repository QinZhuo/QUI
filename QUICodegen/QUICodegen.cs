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
				if (!type.IsAbstract && type.BaseType.CanBeResolved())
				{
					if (type.Is<QUIPanel>())
					{
						UIs.Enqueue(type.Name);
					}
					else if (type.IsType<QUIPanel>())
					{
						if (UIs.Count > 0)
						{
							foreach (var item in UIs)
							{
								Log(Assembly.Name.Name + " " + item);
								type.Fields.Add(new FieldDefinition(item, FieldAttributes.Static | FieldAttributes.Public | FieldAttributes.Private | FieldAttributes.InitOnly, Get<string>()));
							}
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}

