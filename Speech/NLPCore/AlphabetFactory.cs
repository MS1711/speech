using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPCore
{
    [Serializable]
    public class AlphabetFactory
    {
        public enum Type
        {
            String,
            Integer
        }
        public static Type defaultFeatureType = Type.String;

        public const string DefalutFeatureName = "FEATURES";
        public const string DefalutLabelName = "LABELS";
        private Dictionary<string, IAlphabet> maps = null;

        private AlphabetFactory()
        {
            maps = new Dictionary<String, IAlphabet>();
        }

        private AlphabetFactory(Dictionary<String, IAlphabet> maps)
        {
            this.maps = maps;
        }

        /**
         * 构造词典管理器
         * @return 词典工厂
         */
        public static AlphabetFactory buildFactory()
        {
            return new AlphabetFactory();
        }


        public LabelAlphabet DefaultLabelAlphabet()
        {
            IAlphabet alphabet = null;
            if (!maps.ContainsKey(DefalutLabelName))
            {
                maps[DefalutLabelName] = new LabelAlphabet();
                alphabet = maps[DefalutLabelName];
            }
            else
            {
                alphabet = maps[DefalutLabelName];
                if (!(alphabet is LabelAlphabet))
                {
                    throw new Exception();
                }
            }
            return (LabelAlphabet)alphabet;
        }

        public IFeatureAlphabet DefaultFeatureAlphabet()
        {
            return DefaultFeatureAlphabet(defaultFeatureType);
        }

        public IFeatureAlphabet DefaultFeatureAlphabet(Type type)
        {
            return buildFeatureAlphabet(DefalutFeatureName, type);
        }

        public IFeatureAlphabet buildFeatureAlphabet(string name, Type type)
        {
            IAlphabet alphabet = null;
            if (!maps.ContainsKey(name))
            {
                IFeatureAlphabet fa;
                if (type == Type.String)
                    fa = new StringFeatureAlphabet();
                else if (type == Type.Integer)
                    fa = new HashFeatureAlphabet();
                else
                    return null;
                maps[name] = fa;
                alphabet = maps[name];
            }
            else
            {
                alphabet = maps[name];
                if (!(alphabet is IFeatureAlphabet))
                {
                    throw new Exception();
                }
            }
            return (IFeatureAlphabet)alphabet;
        }
    }
}
