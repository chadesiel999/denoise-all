namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using ScopeX.ComModel;

    public class SearchInfo
    {
        private readonly List<SearchArgs> _Contents;

        public SearchInfo(List<SearchArgs> res)
        {
            _Contents = res;
        }

        public ImmutableList<SearchArgs> Contents => _Contents.ToImmutableList();

        public sealed record SearchArgs(SearchType Type, Double Location, String Description);
    }
}
