﻿using log4net;
using NL2ML.models;
using NL2ML.utils;
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
        private static ILog logger = LogManager.GetLogger("common");
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
            builders.Add(new WeatherIntentBuilder());
            builders.Add(new SmartDeviceIntentBuilder());
            builders.Add(new RobotIntentBuilder());
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

            return intents.ToArray();
        }
    }
}
