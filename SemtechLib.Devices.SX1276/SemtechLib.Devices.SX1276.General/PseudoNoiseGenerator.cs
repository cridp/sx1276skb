using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.General
{
	public sealed class PseudoNoiseGenerator
	{
		private ushort lfsr;

		private uint period;

		private ushort initialValue = ushort.MaxValue;

		private PnSequence pn; // = PnSequence.PN15;

		public ushort InitialValue
		{
			get => initialValue;
			set
			{
				initialValue = value;
				lfsr = initialValue;
				period = 0u;
			}
		}

		public PnSequence Pn
		{
			get => pn;
			set
			{
				pn = value;
				lfsr = initialValue;
				period = 0u;
			}
		}

		public PseudoNoiseGenerator()
		{
			lfsr = ushort.MaxValue;
			period = 0u;
			pn = PnSequence.PN15;
		}

		public PseudoNoiseGenerator(ushort initalValue, PnSequence pnSequence)
		{
			InitialValue = initialValue;
			lfsr = InitialValue;
			period = 0u;
			pn = pnSequence;
		}

		public byte NextBit()
		{
			var result = (byte)(lfsr & 1u);
			switch (pn)
			{
			case PnSequence.PN9:
				lfsr = (ushort)((ulong)(lfsr >> 1) ^ ((0uL - (uint)(lfsr & 1)) & 0x220));
				break;
			case PnSequence.PN15:
				lfsr = (ushort)((ulong)(lfsr >> 1) ^ ((0uL - (uint)(lfsr & 1)) & 0xC000));
				break;
			case PnSequence.PN16:
				lfsr = (ushort)((ulong)(lfsr >> 1) ^ ((0uL - (uint)(lfsr & 1)) & 0xB400));
				break;
			}
			period++;
			return result;
		}

		public byte NextByte()
		{
			byte b = 0;
			for (var i = 0; i < 8; i++)
			{
				b |= NextBit();
				b <<= 1;
			}
			return b;
		}
	}
}
