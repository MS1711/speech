﻿using log4net;
using NL2ML.dbhelper;
using NL2ML.handlers;
using NL2ML.models;
using NL2ML.plugins;
using NL2ML.plugins.nlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.api
{
    public class NL2ML
    {
        private static ILog logger = LogManager.GetLogger("common");
        private static ILog nlplogger = LogManager.GetLogger("nl2ml");
        private static NL2ML instance = new NL2ML();
        private List<INL2MLModule> modules = new List<INL2MLModule>();
        public static bool EnableVirtualSmartDevice = true;
        public static bool EnableOnlineErrorFix = false;

        private NL2ML()
        {
            LoadModules();
        }

        public static NL2ML Instance
        {
            get { return NL2ML.instance; }
        }

        private void LoadModules()
        {
            logger.Debug("load nlp module");
            modules.Add(new NLPModule(@"C:/workspace/nlpdictdata/", @"C:/workspace/nlpdictdata/mydict2.txt", @"C:/workspace/nlpdictdata/genredict.txt",
                                    @"C:/workspace/nlpdictdata/verbdict2.txt", @"C:/workspace/nlpdictdata/lastdict2.txt", @"C:/workspace/nlpdictdata/devicedict2.txt"));

        }

        public Result Process(string input)
        {
            logger.Debug("processing input string: " + input);
            List<Intent> intents = new List<Intent>();
            foreach (var item in modules)
            {
                intents.AddRange(item.Parse(input));
            }

            intents.Sort((o1, o2) => {
                if (o1.Domain == Domains.QA || o2.Domain == Domains.QA)
                {
                    return (o1.Domain == Domains.QA ? 1 : 0) - (o1.Domain == Domains.QA ? 1 : 0);
                }
                else
                {
                    int d = o1.Action - o2.Action;
                    if (d == 0)
                    {
                        d = o2.Score - o1.Score;
                    }
                    return d;
                }
            });

            nlplogger.Debug("intent created: " + string.Join(", ", intents));
            return IntentManager.Instance.Process(intents.ToArray());
        }
    }
}
