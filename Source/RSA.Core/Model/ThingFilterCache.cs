using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RSA.Core.Model
{
	public static class ThingFilterCache
	{
		public static void Set(string key, ThingFilter filter)
        {
			CachedFilters[key] = filter;
		}

		public static string KeyByFilter([NotNull] ThingFilter filter)
		{
			var res = CachedFilters.Where(x => x.Value == filter).ToList();
			if (res.Count > 0)
				return res[0].Key;

			return null;
		}

		private static readonly IDictionary<string, ThingFilter> CachedFilters = new Dictionary<string, ThingFilter>{
			{ SearchCategories.CategoryID_Storage, null},
			{ SearchCategories.CategoryID_Bill, null},
			{ SearchCategories.CategoryID_Outfit, null},
			{ SearchCategories.CategoryID_Preview, null},
		};
	}
}
