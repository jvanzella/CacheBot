﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MessageHandler
{
    public class Sample
    {
        public const string BasicQueueName = "BasicQueue";
        public const string PartitionedQueueName = "PartitionedQueue";
        public const string DupdetectQueueName = "DupdetectQueue";
        public const string BasicTopicName = "BasicTopic";
        public const string SessionQueueName = "SessionQueue";
        public const string BasicQueue2Name = "BasicQueue2";
        static readonly string samplePropertiesFileName = "azure-msg-config.properties";
#if STA
        [STAThread]
#endif
        [DebuggerStepThrough]
        public void RunSample(string[] args, Func<string, Task> run)
        {
            var properties = new Dictionary<string, string>
            {
                {"SB_SAMPLES_CONNECTIONSTRING", null},
            };

            // read the settings file created by the ./setup.ps1 file
            var settingsFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                samplePropertiesFileName);
            if (File.Exists(settingsFile))
            {
                using (var fs = new StreamReader(settingsFile))
                {
                    while (!fs.EndOfStream)
                    {
                        var readLine = fs.ReadLine();
                        if (readLine != null)
                        {
                            var propl = readLine.Trim();
                            var cmt = propl.IndexOf('#');
                            if (cmt > -1)
                            {
                                propl = propl.Substring(0, cmt).Trim();
                            }
                            if (propl.Length > 0)
                            {
                                var propi = propl.IndexOf('=');
                                if (propi == -1)
                                {
                                    continue;
                                }
                                var propKey = propl.Substring(0, propi).Trim();
                                var propVal = propl.Substring(propi + 1).Trim();
                                if (properties.ContainsKey(propKey))
                                {
                                    properties[propKey] = propVal;
                                }
                            }
                        }
                    }
                }
            }

            // get overrides from the environment
            foreach (var prop in properties.Keys.ToArray())
            {
                var env = Environment.GetEnvironmentVariable(prop);
                if (env != null)
                {
                    properties[prop] = env;
                }
            }

            run(properties["SB_SAMPLES_CONNECTIONSTRING"]).GetAwaiter().GetResult();
        }
    }
}
