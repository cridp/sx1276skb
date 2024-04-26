using System;

namespace SemtechLib.General.Interfaces
{
	public sealed class DocumentationChangedEventArgs : EventArgs
	{
        private string docName;

        public string DocFolder { get; }

        public string DocName => docName;

		public DocumentationChangedEventArgs(string docFolder, string docName)
		{
			DocFolder = docFolder;
			this.docName = docName;
		}
	}
}
