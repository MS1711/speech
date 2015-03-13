using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class StringTypeTemplet : Templet
    {
        private int id;

        public StringTypeTemplet(int id)
        {
            this.id = id;
        }



        /**
         *  {@inheritDoc}
         */
        public int generateAt(Instance instance, IFeatureAlphabet features, int pos,
                params int[] numLabels)
        {
            string[][] data = (string[][])instance.getData();

            int len = data[0][pos].Length;

            StringBuilder sb = new StringBuilder();

            sb.Append(id);
            sb.Append(':');
            sb.Append(pos);
            sb.Append(':');

            String str = data[0][pos]; //这里数据行列和模板中行列相反
            NLPCore.Chars.StringType type = Chars.getStringType(str);
            string stype = type.ToString();

            if (type == NLPCore.Chars.StringType.M)
            {

                if (str.Length > 4 && str.StartsWith("http:"))
                    stype = "U$";
                else if (str.Length > 4 && str.Contains("@"))
                    stype = "E$";
                else if (str.Contains(":"))
                    stype = "T$";
            }

            sb.Append(stype);

            int index = features.lookupIndex(sb.ToString(), numLabels[0]);
            return index;
        }

        public int getOrder()
        {
            return 0;
        }

        public int[] getVars()
        {
            return new int[] { 0 };
        }

        public int offset(params int[] curs)
        {
            return 0;
        }
    }
}
