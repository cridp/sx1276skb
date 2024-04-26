using System;
using System.Drawing;

namespace Fusionbird.FusionToolkit.FusionTrackBar
{
	public sealed class TrackBarDrawItemEventArgs : EventArgs
	{
		private Rectangle _bounds;
        private TrackBarItemState _state;

		public Rectangle Bounds => _bounds;

        public Graphics Graphics { get; }

        public TrackBarItemState State => _state;

		public TrackBarDrawItemEventArgs(Graphics graphics, Rectangle bounds, TrackBarItemState state)
		{
			Graphics = graphics;
			_bounds = bounds;
			_state = state;
		}
	}
}
