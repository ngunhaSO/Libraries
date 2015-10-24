using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public class EnumerableComparer<T> : IComparer<IEnumerable<T>>
    {
        private IComparer<T> comp;

        public EnumerableComparer()
        {
            comp = Comparer<T>.Default;
        }

        public EnumerableComparer(IComparer<T> comparer)
        {
            comp = comparer;
        }

        public int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            using(IEnumerator<T> leftIt = x.GetEnumerator())
            using(IEnumerator<T> rightIt = y.GetEnumerator())
            {
                while(true)
                {
                    bool left = leftIt.MoveNext();
                    bool right = rightIt.MoveNext();
                    if (!(left || right)) return 0;

                    if (!left) return -1;
                    if (!right) return 1;

                    int itemResult = comp.Compare(leftIt.Current, rightIt.Current);
                    if (itemResult != 0) return itemResult;
                }
            }
        }
        
    }
}
