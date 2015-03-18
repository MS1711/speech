using NL2ML.models;
using NLPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.nlp
{
    public class NLPModule : INL2MLModule
    {
        private CNFactory factory;
        private List<IntentBuilder> builders = new List<IntentBuilder>();

        public NLPModule(string modelPath, params string[] dictPath)
        {
            factory = CNFactory.GetInstance(modelPath);
            if (dictPath != null && dictPath.Length > 0)
            {
                foreach (var item in dictPath)
                {
                    CNFactory.loadDict(item);
                }
            }

            builders.Add(new MediaIntentBuilder());
            builders.Add(new WeatherIntentBuilder());
        }

        public Intent[] Parse(string input)
        {
            Context ctx = new Context();
            ctx.RawString = input;
            string[][] tags = factory.tag(input);
            ctx.Tags = tags;
            List<Intent> intents = new List<Intent>();

            foreach (var item in builders)
            {
                intents.AddRange(item.GetIntents(ctx));
            }

            intents.Sort((o1, o2) => {
                return o2.Score - o1.Score;
            });

            return intents.ToArray();
        }
    }
}
