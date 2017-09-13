using System;
using System.Linq;
using System.Reflection;

namespace SerialPCAP
{
	public static class AssemblyExtensions
	{
		public static T GetAttribute<T>(this ICustomAttributeProvider assembly, bool inherit = false)
		where T : Attribute
		{
			return assembly
				.GetCustomAttributes(typeof(T), inherit)
				.OfType<T>()
				.FirstOrDefault();
		}

		public static string Description()
		{
			return Assembly.GetExecutingAssembly().GetAttribute<AssemblyDescriptionAttribute>().Description;
		}

		public static Version Version()
		{
			return Assembly.GetExecutingAssembly().GetName().Version;
		}
	}
}

