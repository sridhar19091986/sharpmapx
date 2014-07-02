//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class manages a symbol in the legend.
    /// </summary>
    public class LegendSymbol : BaseGisEntity
    {
        private string _SymbolUri;
        private string _SymbolImage;

        /// <summary>
        /// Creates a new instance of the <see cref="LegendSymbol"/>
        /// </summary>
        public LegendSymbol()
            : base()
        {
            Clear();
        }

        /// <summary>
        /// Name of the icon
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Width of the icon
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the icon
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// URL of the icon.
        /// </summary>
        public string SymbolUri
        {
            get { return _SymbolUri; }
            set { _SymbolUri = value; }
        }

        /// <summary>
        /// URL of the image
        /// </summary>
        public string SymbolImage
        {
            get { return _SymbolImage; }
            set { _SymbolImage = value; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LegendSymbol"/>
        /// </summary>
        /// <param name="sName">Name of the symbol</param>
        /// <param name="sUri">URI of the symbol</param>
        public LegendSymbol(string sName, string sUri)
            : this()
        {
            LoadValues(sName, sUri);
        }

        private void loadimage(string imageUri)
        {
            Uri aUri = new Uri(imageUri);
            //TODO: converti in bitmap            _SymbolImage = new BitmapImage(aUri);
        }

        /// <summary>
        /// Clears the object.
        /// </summary>
        public void Clear()
        {
            name = string.Empty;
            _SymbolImage = null;
            _SymbolUri = string.Empty;
        }

        /// <summary>
        /// Load values
        /// </summary>
        /// <param name="sName">Name</param>
        /// <param name="sUri">Uri</param>
        public void LoadValues(string sName, string sUri)
        {
            Clear();
            name = sName;
            _SymbolUri = sUri;
            loadimage(sUri);
        }
    }

    /// <summary>
    /// This class manages a list of LegendSymbol objects.
    /// </summary>
    public class LegendSymbols : System.Collections.Generic.List<LegendSymbol>
    {
        /// <summary>
        /// Returns a symbol object given its name.
        /// </summary>
        /// <param name="symbolName">Name of the symbol</param>
        /// <returns>LegendSymbol object</returns>
        public LegendSymbol GetSymbolByName(string symbolName)
        {
            foreach (LegendSymbol obj in this)
            {
                if (obj.Name == symbolName) return obj;
            }
            return null;
        }
    }
}
