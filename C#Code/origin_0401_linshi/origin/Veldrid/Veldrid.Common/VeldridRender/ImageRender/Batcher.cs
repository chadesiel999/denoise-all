using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    public class Batcher<TTexture> where TTexture : notnull
    {
        private Stack<BatchGroup> _batchGroups;
        private Dictionary<TTexture, BatchGroup> _batchItems;

        public Batcher()
        {
            _batchItems = new Dictionary<TTexture, BatchGroup>();
            _batchGroups = new Stack<BatchGroup>();
        }

        public ref BatchItem Add(TTexture texture)
        {
            if (!_batchItems.TryGetValue(texture, out var group))
            {
                group = GetBatchGroup();
                group.Clear();
                _batchItems[texture] = group;
            }

            return ref group.Add();
        }

        public void Clear()
        {
            foreach (var group in this)
                ReturnBatchGroup(group.Value);
            _batchItems.Clear();
        }

        public Dictionary<TTexture, BatchGroup>.Enumerator GetEnumerator() =>
            _batchItems.GetEnumerator();

        private BatchGroup GetBatchGroup()
        {
            if (!_batchGroups.TryPop(out var group))
                group = new BatchGroup();
            return group;
        }

        private void ReturnBatchGroup(BatchGroup group) => _batchGroups.Push(group);
    }
}
