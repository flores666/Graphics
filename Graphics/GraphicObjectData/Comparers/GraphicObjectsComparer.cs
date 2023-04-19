namespace GraphicsBase.GraphicObjectData.Comparers
{
    public class GraphicObjectsComparer : IComparer<GraphicObject>
    {
        public int Compare(GraphicObject? a, GraphicObject? b)
        {
			if (a.TextureId > b.TextureId)
			{
				return 1;
			}
			else if (a.TextureId < b.TextureId)
			{
				return -1;
			}
			else
			{
				if (a.MeshId > b.MeshId)
				{
					return 1;
				}
				else if (a.MeshId < b.MeshId)
				{
					return -1;
				}
			}
			return 0;
		}
    }
}
