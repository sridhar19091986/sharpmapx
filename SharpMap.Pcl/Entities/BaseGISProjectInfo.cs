//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System.Collections.Generic;

namespace SharpMap.Entities
{
    /// <summary>
    /// This class represents a GIS project.
    /// </summary>
    public abstract class BaseGisProjectInfo<T> : BaseGisEntity
        where T:BaseGisEntity
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BaseGisProjectInfo<T>">/>
        /// </summary>
        protected BaseGisProjectInfo()
            : base()
        {
            Layers = new List<T>();
        }

        private double _maxX;

        /// <summary>
        /// Maximum longitude of the project extent.
        /// </summary>
        public double MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        private double _maxY;

        /// <summary>
        /// Maximum latitude of the project extent.
        /// </summary>
        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        private double _minX;

        /// <summary>
        /// Minimum longitude of the project extent.
        /// </summary>
        public double MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }


        private double _minY;

        /// <summary>
        /// Minimum latitude of the project extent.
        /// </summary>
        public double MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        /// <summary>
        /// Returns a layer given its name
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public abstract T GetLayerByName(string layerName);

        /// <summary>
        /// List of layers contained in the project.
        /// </summary>
        public List<T> Layers { get; private set; }


    }
}
