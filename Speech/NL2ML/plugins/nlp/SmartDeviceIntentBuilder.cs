using NL2ML.consts;
using NL2ML.dbhelper;
using NL2ML.handlers;
using NL2ML.models;
using NL2ML.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.plugins.nlp
{
    public class SmartDeviceIntentBuilder : IntentBuilder
    {
        public Intent[] GetIntents(Context context)
        {
            List<Intent> list = new List<Intent>();

            string[][] tags = context.Tags;
            string raw = context.RawString;

            bool has = HasSmartDevice(context);
            if (!has)
            {
                return list.ToArray();
            }

            string action = GetAction(context);
            if (string.IsNullOrEmpty(action))
            {
                return list.ToArray();
            }

            SmartDeviceData data = new SmartDeviceData();
            string[] deviceInfo = GetDeviceInfo(context);
            data.Target = deviceInfo[0];
            data.TargetClass = deviceInfo[1];
            data.Location = GetLocation(context);
            data.Cmd = action;

            switch (action)
            {
                case "start":
                    {
                        Intent intent = new Intent();
                        intent.Domain = Domains.SmartDevice;
                        intent.Data = data;
                        intent.Action = Actions.Open;
                        list.Add(intent);
                        break;
                    }
                case "stop":
                    {
                        Intent intent = new Intent();
                        intent.Domain = Domains.SmartDevice;
                        intent.Data = data;
                        intent.Action = Actions.Close;
                        list.Add(intent);
                        break;
                    }
                case "lookup":
                    {
                        Intent intent = new Intent();
                        intent.Domain = Domains.SmartDevice;
                        intent.Data = data;
                        intent.Action = Actions.Query;
                        list.Add(intent);
                        break;
                    }
            }

            return list.ToArray();
        }

        private string GetLocation(Context context)
        {
            string target = POSUtils.GetWordByPOS(context.Tags, POSConstants.NounSmartDeviceRoomClass);
            if (!string.IsNullOrEmpty(target))
            {
                return target;
            }

            return "X";
        }

        private string[] GetDeviceInfo(Context context)
        {
            string[] info = new string[2];

            if (POSUtils.HasPOS(context.Tags, POSConstants.NounSmartDeviceAirClass) &&
                POSUtils.HasPOS(context.Tags, POSConstants.NounSmartDeviceStatusClass) &&
                POSUtils.HasPOS(context.Tags, POSConstants.VerbSearch))
            {
                info[0] = POSUtils.GetWordByPOS(context.Tags, POSConstants.NounSmartDeviceAirClass);
                info[1] = POSConstants.NounSmartDeviceAirpurifierClass;

                return info;
            }

            string[] supportedTargets = {POSConstants.NounSmartDeviceAirpurifierClass,
                                        POSConstants.NounSmartDeviceCurtainClass,
                                        POSConstants.NounSmartDeviceLightClass};
            foreach (var item in supportedTargets)
            {
                string target = POSUtils.GetWordByPOS(context.Tags, item);
                if (!string.IsNullOrEmpty(target))
                {
                    info[0] = target;
                    info[1] = item;
                    break;
                }
            }

            return info;
        }

        private bool HasSmartDevice(Context context)
        {
            string[] supportedTargets = {POSConstants.NounSmartDeviceAirClass,
                                        POSConstants.NounSmartDeviceAirpurifierClass,
                                        POSConstants.NounSmartDeviceCurtainClass,
                                        POSConstants.NounSmartDeviceLightClass};
            foreach (var item in supportedTargets)
            {
                bool has = POSUtils.HasPOS(context.Tags, item);
                if (has)
                {
                    return has;
                }
            }

            return false;
        }

        private string GetAction(Context context)
        {
            string[][] tags = context.Tags;
            string[] verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbSmartDevice);
            if (verbs == null || verbs.Length == 0)
            {
                verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbMixed);
            }
            if (verbs == null || verbs.Length == 0)
            {
                verbs = POSUtils.GetWordsByPOS(tags, POSConstants.VerbSearch);
            }
            if (verbs == null || verbs.Length == 0)
            {
                return "";
            }

            string item = verbs[0];
            string command = DBHelperFactory.GetInstance().TranslateCommand(item);
            return command;
        }
    }
}
