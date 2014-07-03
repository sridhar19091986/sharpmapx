//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class manages an icon in the legend.
    /// </summary>
    public class LegendIcon : INamedEntity
    {
        private string _SymbolUri;
        private string _SymbolImage;

        /// <summary>
        /// Creates a new instance of the <see cref="LegendIcon"/>
        /// </summary>
        public LegendIcon()
            : base()
        {
            Clear();
        }

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
        /// Creates a new instance of the <see cref="LegendIcon"/>
        /// </summary>
        /// <param name="sName">Name of the symbol</param>
        /// <param name="sUri">URI of the symbol</param>
        public LegendIcon(string sName, string sUri)
            : this()
        {
            LoadValues(sName, sUri);
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
        }

        /// <summary>
        /// Name of the legend icon.
        /// </summary>
        protected string name;
        /// <summary>
        /// Name of the legend icon.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }

    /// <summary>
    /// This class manages a list of LegendIcon objects.
    /// </summary>
    public class LegendIcons : System.Collections.Generic.List<LegendIcon>
    {
        /// <summary>
        /// Returns a legend icon given its name.
        /// </summary>
        /// <param name="symbolName">Name of the symbol</param>
        /// <returns>LegendIcon object</returns>
        public LegendIcon GetSymbolByName(string symbolName)
        {
            foreach (LegendIcon obj in this)
            {
                if (obj.Name == symbolName) return obj;
            }
            return null;
        }
    }
}
