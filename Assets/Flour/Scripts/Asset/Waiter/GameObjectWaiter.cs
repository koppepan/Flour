using UnityEngine;

namespace Flour.Asset
{
	public class GameObjectWaiter : AssetCacheWaiter<GameObject>
	{
		public GameObjectWaiter(string key, int compressCount) : base(key, compressCount) { }
	}
}
