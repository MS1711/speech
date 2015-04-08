using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    public class LabelParser
    {
        public enum Type
        {
            /**
             * 单个字符串
             */
            STRING,
            /**
             * 字符串序列
             */
            SEQ
        }

        public static Predict<string[]> parse(TPredict<int[]> res,
                LabelAlphabet labels, Type t)
        {
            int n = res.size();
            Predict<string[]> pred = null;
            switch (t)
            {
                case Type.SEQ:
                    pred = new Predict<string[]>(n);
                    for (int i = 0; i < n; i++)
                    {
                        int[] preds = (int[])res.getLabel(i);
                        string[] l = labels.lookupString(preds);
                        pred.set(i, l, res.getScore(i));
                    }
                    break;
                case Type.STRING:
                    //pred = new Predict<string>(n);
                    //for (int i = 0; i < n; i++)
                    //{
                    //    if (res.getLabel(i) == null)
                    //    {
                    //        pred.set(i, "null", 0f);
                    //        continue;
                    //    }
                    //    int idx = (int)res.getLabel(i);
                    //    string l = labels.lookupString(idx);
                    //    pred.set(i, l, res.getScore(i));
                    //}
                    return pred;
                default:
                    break;

            }
            return pred;

        }

        public static Predict<string> parse(Predict<int> res,
                LabelAlphabet labels, Type t)
        {
            int n = res.size();
            Predict<string> pred = null;
            switch (t)
            {
                case Type.SEQ:
                    //pred = new Predict<string[]>(n);
                    //for (int i = 0; i < n; i++)
                    //{
                    //    int[] preds = (int[])res.getLabel(i);
                    //    string[] l = labels.lookupString(preds);
                    //    pred.set(i, l, res.getScore(i));
                    //}
                    break;
                case Type.STRING:
                    pred = new Predict<string>(n);
                    for (int i = 0; i < n; i++)
                    {
                        if (res.getLabel(i) == null)
                        {
                            pred.set(i, "null", 0f);
                            continue;
                        }
                        int idx = (int)res.getLabel(i);
                        string l = labels.lookupString(idx);
                        pred.set(i, l, res.getScore(i));
                    }
                    return pred;
                default:
                    break;

            }
            return pred;

        }
    }
}
