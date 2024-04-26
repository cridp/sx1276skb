namespace SemtechLib.Controls.HexBoxCtrl
{
	internal struct BytePositionInfo
	{
        private long _index;

        public int CharacterPosition { get; }

        public long Index => _index;

		public BytePositionInfo(long index, int characterPosition)
		{
			_index = index;
			CharacterPosition = characterPosition;
		}
	}
}
