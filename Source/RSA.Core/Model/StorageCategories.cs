namespace RSA.Core
{
    using JetBrains.Annotations;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Libarary class")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Libarary class")]
    public static partial class SearchCategories
    {
        private static readonly IDictionary<string, SearchTerm> CachedTerms;

        public static SearchTerm TermFor([NotNull] string category)
        {
            SearchTerm term;
            if (!CachedTerms.TryGetValue(category, out term))
            {
                CachedTerms.Add(category, term = new SearchTerm(category));
            }

            return term;
        }
    }
}