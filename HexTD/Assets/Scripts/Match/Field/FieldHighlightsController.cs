using System.Collections.Generic;
using HexSystem;
using Tools;

namespace Match.Field
{
	public class FieldHighlightsController: BaseDisposable
	{
		public struct Context
		{
			public HexObjectsContainer HexObjectsContainer;
			
	        public Context(
		        HexObjectsContainer hexObjectsContainer)
            {
                HexObjectsContainer = hexObjectsContainer;
            }
        }

        private readonly Context _context;

        public FieldHighlightsController(Context context)
        {
	        _context = context;
        }

        public void HighlightHexes(IReadOnlyCollection<Hex2d> hexes)
        {
	        foreach (Hex2d hex in hexes)
	        {
		        _context.HexObjectsContainer.HexObjects[hex.GetHashCode()].SetIsHighlighted(true);
	        }
        }
        
        public void RemoveAllHighlights()
        {
	        foreach (var hexObject in _context.HexObjectsContainer.HexObjects.Values)
	        {
		        hexObject.SetIsHighlighted(false);
	        }
        }
	}
}