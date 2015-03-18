using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.mediaProvider
{
    class MediaDataProviderFactory
    {
        private static IMediaContentProvider provider = new BaiduMediaContentProvider();

        public static IMediaContentProvider GetProvider()
        {
            return provider;
        }
    }
}
