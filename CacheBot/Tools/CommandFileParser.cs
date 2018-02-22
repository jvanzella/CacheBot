using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CacheBot.Tools
{
    public class CommandFileParser
    {
        public TreeNode ParseFile(string filename)
        {
            using (var file = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                var jsonString = file.ReadToEnd();

                return ParseString(jsonString);
            }
        }

        public static TreeNode Parse()
        {
            var parent = new TreeNode
            {
                Name = "@CacheBot",
                CommandType = ParserCommandType.Bot,
                Children = new[]
                {
                    new TreeNode
                    {
                        Name = "clearAll",
                        CommandType = ParserCommandType.Command,
                        Children = new []
                        {
                            new TreeNode
                            {
                                Name = "redis",
                                CommandType = ParserCommandType.Cache,
                                Children = new []
                                {
                                    new TreeNode
                                    {
                                        Name = "dataBaseId",
                                        CommandType = ParserCommandType.Variable
                                    }
                                }
                            },
                            new TreeNode
                            {
                                Name = "htmlTemplate",
                                CommandType = ParserCommandType.Cache,
                                Children = new TreeNode[0]
                            }
                        }
                    },
                    new TreeNode
                    {
                        Name = "remove",
                        CommandType = ParserCommandType.Command,
                        Children = new []
                        {
                            new TreeNode
                            {
                                Name = "redis",
                                CommandType = ParserCommandType.Cache,
                                Children = new []
                                {
                                    new TreeNode
                                    {
                                        Name = "dataBaseId",
                                        CommandType = ParserCommandType.Variable
                                    }
                                }
                            },
                            new TreeNode
                            {
                                Name = "htmlTemplate",
                                CommandType = ParserCommandType.Cache,
                                Children = new TreeNode[0]
                            }
                        }
                    },
                }
            };
            return parent;
        }

        public TreeNode ParseString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            var jObject = JObject.Parse(jsonString);
            var cacheBot = jObject.First;
            var name = ((JProperty) cacheBot).Name;
            var firstObject = cacheBot.First;
            var commandType = firstObject["commandType"].ToString();
            

            var parent = new TreeNode
            {
                Name = "@ChatBot",
                CommandType = ParserCommandType.Command,
                Children = new []
                {
                    new TreeNode
                    {
                        Name = "clearAll",
                        CommandType = ParserCommandType.Command,
                        Children = new []
                        {
                            new TreeNode
                            {
                                Name = "redis",
                                CommandType = ParserCommandType.Command,
                                Children = new []
                                {
                                    new TreeNode
                                    {
                                        Name = "dataBaseId",
                                        CommandType = ParserCommandType.Variable
                                    }
                                }
                            }
                        }
                    }, 
                }
            };
            return CreateTreeNode(parent, firstObject);
        }

        private static TreeNode CreateTreeNode(TreeNode parent, JToken node)
        {
            var command = ((JProperty) node.First).Name;
            
            return null;
        }
    }
}