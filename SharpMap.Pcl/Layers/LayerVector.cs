//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using SharpMap.Entities;

namespace SharpMap.Layers
{
    public abstract class LayerVector: LayerBaseDrawable
    {
        protected LayerVector()
        {
            Fields = new LayerFields();
            Items = new GisShapeCollection();
        }

        /// <summary>
        /// Default shape type of the layer.
        /// </summary>
        public GisShapeType DefaultShapeType { get; set; }

        /// <summary>
        /// Creates a GIS shape from the layer
        /// </summary>
        /// <returns>The created GIS shape</returns>
        public GisShapeBase CreateShape()
        {
            switch (DefaultShapeType)
            {
                case GisShapeType.GisShapeTypePoint:
                    return new GisShapePoint(this);
                case GisShapeType.GisShapeTypeMultiPoint:
                    return new GisShapeMultiPoint(this);
                case GisShapeType.GisShapeTypeArc:
                    return new GisShapeArc(this);
                case GisShapeType.GisShapeTypePolygon:
                    return new GisShapePolygon(this);
                default:
                    throw new Exception("Default shape type not implemented");
            }
        }

        /// <summary>
        /// Fields of the layer.
        /// </summary>
        public LayerFields Fields { get; private set; }

        /// <summary>
        /// Shapes of the layer
        /// </summary>
        public GisShapeCollection Items { get; private set; }

        public abstract bool IsOpened { get; }

        public abstract GisShapeBase FindFirst();

        public abstract GisShapeBase FindFirst(Extent extent);

        public abstract GisShapeBase FindNext();

        public abstract void Open();
        public abstract void Close();

        public virtual bool IsReadOnly
        {
            get;
            set;
        }

        public virtual bool IsSelectable
        {
            get;
            set;
        }
    }
}
