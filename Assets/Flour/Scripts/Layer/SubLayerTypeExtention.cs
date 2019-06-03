
namespace Flour.Layer
{
	public static class SubLayerTypeExtention
	{
		static JapaneaseAttributeCache<SubLayerType> jpnCache;

		static SubLayerTypeExtention()
		{
			jpnCache = new JapaneaseAttributeCache<SubLayerType>();
		}
		public static string ToJpnName(this SubLayerType type)
		{
			return jpnCache.GetJpnName(type);
		}
	}
}
