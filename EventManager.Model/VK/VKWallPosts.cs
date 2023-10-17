using System.Collections.Generic;

namespace EventManager.Model.VK
{
    class VKWallPosts
    {
        public int count { get; set; }
        public IEnumerable<VKWallPost> items { get; set; }
    }
}
