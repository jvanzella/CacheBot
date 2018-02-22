using System.Collections.Generic;

namespace CacheBot.Tools
{
    public class TreeNode
    {
        public string Name { get; set; }
        public ParserCommandType CommandType { get; set; }
        public IEnumerable<TreeNode> Children { get; set; }
    }
}