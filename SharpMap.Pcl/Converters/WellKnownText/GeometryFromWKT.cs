// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace SharpMap.Converters.WellKnownText
{
    /// <summary>
    ///  Converts a Well-known Text representation to a <see cref="SharpMap.Geometries.Geometry"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>The Well-Known Text (WKT) representation of Geometry is designed to exchange geometry data in ASCII form.</para>
    /// Examples of WKT representations of geometry objects are:
    /// <list type="table">
    /// <listheader><term>Geometry </term><description>WKT Representation</description></listheader>
    /// <item><term>A Point</term>
    /// <description>POINT(15 20)<br/> Note that point coordinates are specified with no separating comma.</description></item>
    /// <item><term>A LineString with four points:</term>
    /// <description>LINESTRING(0 0, 10 10, 20 25, 50 60)</description></item>
    /// <item><term>A Polygon with one exterior ring and one interior ring:</term>
    /// <description>POLYGON((0 0,10 0,10 10,0 10,0 0),(5 5,7 5,7 7,5 7, 5 5))</description></item>
    /// <item><term>A MultiPoint with three Point values:</term>
    /// <description>MULTIPOINT(0 0, 20 20, 60 60)</description></item>
    /// <item><term>A MultiLineString with two LineString values:</term>
    /// <description>MULTILINESTRING((10 10, 20 20), (15 15, 30 15))</description></item>
    /// <item><term>A MultiPolygon with two Polygon values:</term>
    /// <description>MULTIPOLYGON(((0 0,10 0,10 10,0 10,0 0)),((5 5,7 5,7 7,5 7, 5 5)))</description></item>
    /// <item><term>A GeometryCollection consisting of two Point values and one LineString:</term>
    /// <description>GEOMETRYCOLLECTION(POINT(10 10), POINT(30 30), LINESTRING(15 15, 20 20))</description></item>
    /// </list>
    /// </remarks>
    public class GeometryFromWKT
    {

        /// <summary>
        /// Converts a Well-known text representation to a <see cref="SharpMap.Geometries.Geometry"/>.
        /// </summary>
        /// <param name="wellKnownText">A <see cref="SharpMap.Geometries.Geometry"/> tagged text string ( see the OpenGIS Simple Features Specification.</param>
        /// <returns>Returns a <see cref="SharpMap.Geometries.Geometry"/> specified by wellKnownText.  Throws an exception if there is a parsing problem.</returns>
        public static Geometry Parse(string wellKnownText)
        {
            // throws a parsing exception is there is a problem.
            StringReader reader = new StringReader(wellKnownText);
            return Parse(reader);
        }

        /// <summary>
        /// Converts a Well-known Text representation to a <see cref="SharpMap.Geometries.Geometry"/>.
        /// </summary>
        /// <param name="reader">A Reader which will return a Geometry Tagged Text
        /// string (see the OpenGIS Simple Features Specification)</param>
        /// <returns>Returns a <see cref="SharpMap.Geometries.Geometry"/> read from StreamReader. 
        /// An exception will be thrown if there is a parsing problem.</returns>
        public static Geometry Parse(TextReader reader)
        {
            WktStreamTokenizer tokenizer = new WktStreamTokenizer(reader);

            return ReadGeometryTaggedText(tokenizer);
        }

        /// <summary>
        /// Returns the next array of Coordinates in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text format.  The
        /// next element returned by the stream should be "(" (the beginning of "(x1 y1, x2 y2, ..., xn yn)" or
        /// "EMPTY".</param>
        /// <returns>The next array of Coordinates in the stream, or an empty array of "EMPTY" is the
        /// next element returned by the stream.</returns>
        private static Collection<Point> GetCoordinates(WktStreamTokenizer tokenizer)
        {
            Collection<Point> coordinates = new Collection<Point>();
            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken == "EMPTY")
                return coordinates;

            double X = GetNextNumber(tokenizer);
            double Y = GetNextNumber(tokenizer);

            Point externalCoordinate = new Point(X,Y);
            coordinates.Add(externalCoordinate);
            nextToken = GetNextCloserOrComma(tokenizer);
            while (nextToken == ",")
            {
                var Xi = GetNextNumber(tokenizer);
                var Yi = GetNextNumber(tokenizer);
                var internalCoordinate = new Point(Xi,Yi);
                coordinates.Add(internalCoordinate);
                nextToken = GetNextCloserOrComma(tokenizer);
            }
            return coordinates;
        }


        /// <summary>
        /// Returns the next number in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known text format.  The next token
        /// must be a number.</param>
        /// <returns>Returns the next number in the stream.</returns>
        /// <remarks>
        /// ParseException is thrown if the next token is not a number.
        /// </remarks>
        private static double GetNextNumber(WktStreamTokenizer tokenizer)
        {
            tokenizer.NextToken();
            return tokenizer.GetNumericValue();
        }

        /// <summary>
        /// Returns the next "EMPTY" or "(" in the stream as uppercase text.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be "EMPTY" or "(".</param>
        /// <returns>the next "EMPTY" or "(" in the stream as uppercase
        /// text.</returns>
        /// <remarks>
        /// ParseException is thrown if the next token is not "EMPTY" or "(".
        /// </remarks>
        private static string GetNextEmptyOrOpener(WktStreamTokenizer tokenizer)
        {
            tokenizer.NextToken();
            string nextWord = tokenizer.GetStringValue();
            if (nextWord == "EMPTY" || nextWord == "(")
                return nextWord;

            throw new Exception("Expected 'EMPTY' or '(' but encountered '" + nextWord + "'");
        }

        /// <summary>
        /// Returns the next ")" or "," in the stream.
        /// </summary>
        /// <param name="tokenizer">tokenizer over a stream of text in Well-known Text
        /// format. The next token must be ")" or ",".</param>
        /// <returns>Returns the next ")" or "," in the stream.</returns>
        /// <remarks>
        /// ParseException is thrown if the next token is not ")" or ",".
        /// </remarks>
        private static string GetNextCloserOrComma(WktStreamTokenizer tokenizer)
        {
            tokenizer.NextToken();
            string nextWord = tokenizer.GetStringValue();
            if (nextWord == "," || nextWord == ")")
            {
                return nextWord;
            }
            throw new Exception("Expected ')' or ',' but encountered '" + nextWord + "'");
        }

        /// <summary>
        /// Returns the next ")" in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be ")".</param>
        /// <returns>Returns the next ")" in the stream.</returns>
        /// <remarks>
        /// ParseException is thrown if the next token is not ")".
        /// </remarks>
        private static string GetNextCloser(WktStreamTokenizer tokenizer)
        {
            string nextWord = GetNextWord(tokenizer);
            if (nextWord == ")")
                return nextWord;

            throw new Exception("Expected ')' but encountered '" + nextWord + "'");
        }

        /// <summary>
        /// Returns the next word in the stream as uppercase text.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        /// format. The next token must be a word.</param>
        /// <returns>Returns the next word in the stream as uppercase text.</returns>
        /// <remarks>
        /// Exception is thrown if the next token is not a word.
        /// </remarks>
        private static string GetNextWord(WktStreamTokenizer tokenizer)
        {
            TokenType type = tokenizer.NextToken();
            string token = tokenizer.GetStringValue();
            if (type == TokenType.Number)
                throw new Exception("Expected a number but got " + token);
            else if (type == TokenType.Word)
                return token.ToUpper();
            else if (token == "(")
                return "(";
            else if (token == ")")
                return ")";
            else if (token == ",")
                return ",";

            throw new Exception("Not a valid symbol in WKT format.");
        }

        /// <summary>
        /// Creates a Geometry using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        /// format. The next tokens must form a &lt;Geometry Tagged Text&gt;.</param>
        /// <returns>Returns a Geometry specified by the next token in the stream.</returns>
        /// <remarks>
        /// Exception is thrown if the coordinates used to create a Polygon
        /// shell and holes do not form closed linestrings, or if an unexpected
        /// token is encountered.
        /// </remarks>
        private static Geometry ReadGeometryTaggedText(WktStreamTokenizer tokenizer)
        {
            tokenizer.NextToken();
            string type = tokenizer.GetStringValue().ToUpper();
            Geometry geometry = null;
            switch (type)
            {
                case "POINT":
                    geometry = ReadPointText(tokenizer);
                    break;
                case "LINESTRING":
                    geometry = ReadLineStringText(tokenizer);
                    break;
                case "MULTIPOINT":
                    geometry = ReadMultiPointText(tokenizer);
                    break;
                case "MULTILINESTRING":
                    geometry = ReadMultiLineStringText(tokenizer);
                    break;
                case "POLYGON":
                    geometry = ReadPolygonText(tokenizer);
                    break;
                case "MULTIPOLYGON":
                    geometry = ReadMultiPolygonText(tokenizer);
                    break;
                case "GEOMETRYCOLLECTION":
                    geometry = ReadGeometryCollectionText(tokenizer);
                    break;
                default:
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, "Geometrytype '{0}' is not supported.",
                                                      type));
            }
            return geometry;
        }

        /// <summary>
        /// Creates a <see cref="MultiPolygon"/> using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer">tokenizer over a stream of text in Well-known Text
        /// format. The next tokens must form a MultiPolygon.</param>
        /// <returns>a <code>MultiPolygon</code> specified by the next token in the 
        /// stream, or if if the coordinates used to create the <see cref="Polygon"/>
        /// shells and holes do not form closed linestrings.</returns>
        private static MultiPolygon ReadMultiPolygonText(WktStreamTokenizer tokenizer)
        {
            var arrpolys = new List<IPolygon>();
            
            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken == "EMPTY")
                return new MultiPolygon(arrpolys.ToArray());

            Polygon polygon = ReadPolygonText(tokenizer);
            arrpolys.Add(polygon);
            nextToken = GetNextCloserOrComma(tokenizer);
            while (nextToken == ",")
            {
                polygon = ReadPolygonText(tokenizer);
                arrpolys.Add(polygon);
                nextToken = GetNextCloserOrComma(tokenizer);
            }

            MultiPolygon polygons = new MultiPolygon(arrpolys.ToArray());
            return polygons;
        }

        /// <summary>
        /// Creates a Polygon using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        ///  format. The next tokens must form a &lt;Polygon Text&gt;.</param>
        /// <returns>Returns a Polygon specified by the next token
        ///  in the stream</returns>
        ///  <remarks>
        ///  ParseException is thown if the coordinates used to create the Polygon
        ///  shell and holes do not form closed linestrings, or if an unexpected
        ///  token is encountered.
        ///  </remarks>
        private static Polygon ReadPolygonText(WktStreamTokenizer tokenizer)
        {
            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken == "EMPTY")
                return new Polygon(new LinearRing(new List<Coordinate>().ToArray()));

            var points = GetCoordinates(tokenizer);
            var arrexteriorring = new Coordinate[points.Count];
            for (int i = 0; i < arrexteriorring.Length; i++)
                arrexteriorring[i] = new Coordinate(points[i].X, points[i].Y);

            var exteriorRing = new LinearRing(arrexteriorring);
            nextToken = GetNextCloserOrComma(tokenizer);

            var interiorRings = new List<ILinearRing>();

            while (nextToken == ",")
            {
                var holes = GetCoordinates(tokenizer);
                var arrholes = new Coordinate[holes.Count];
                for (int i = 0; i < arrholes.Length; i++)
                    arrholes[i] = new Coordinate(holes[i].X, holes[i].Y);

                //Add holes
                interiorRings.Add(new LinearRing(arrholes));
                nextToken = GetNextCloserOrComma(tokenizer);
            }

            Polygon pol = new Polygon(exteriorRing, interiorRings.ToArray());
            return pol;
        }


        /// <summary>
        /// Creates a Point using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        /// format. The next tokens must form a &lt;Point Text&gt;.</param>
        /// <returns>Returns a Point specified by the next token in
        /// the stream.</returns>
        /// <remarks>
        /// ParseException is thrown if an unexpected token is encountered.
        /// </remarks>
        private static Point ReadPointText(WktStreamTokenizer tokenizer)
        {
            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken == "EMPTY")
                return new Point(0,0);
            var X = GetNextNumber(tokenizer);
            var Y = GetNextNumber(tokenizer);

            Point p = new Point(X,Y);
            GetNextCloser(tokenizer);
            return p;
        }

        /// <summary>
        /// Creates a Point using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text
        /// format. The next tokens must form a &lt;Point Text&gt;.</param>
        /// <returns>Returns a Point specified by the next token in
        /// the stream.</returns>
        /// <remarks>
        /// ParseException is thrown if an unexpected token is encountered.
        /// </remarks>
        private static MultiPoint ReadMultiPointText(WktStreamTokenizer tokenizer)
        {
            var arrpoints = new List<IPoint>();

            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken == "EMPTY")
                return new MultiPoint(arrpoints.ToArray());
            
            arrpoints.Add(new Point(GetNextNumber(tokenizer), GetNextNumber(tokenizer)));
            nextToken = GetNextCloserOrComma(tokenizer);
            while (nextToken == ",")
            {
                arrpoints.Add(new Point(GetNextNumber(tokenizer), GetNextNumber(tokenizer)));
                nextToken = GetNextCloserOrComma(tokenizer);
            }

            MultiPoint mp = new MultiPoint(arrpoints.ToArray());
            return mp;
        }

        /// <summary>
        /// Creates a <see cref="MultiLineString"/> using the next token in the stream. 
        /// </summary>
        /// <param name="tokenizer">tokenizer over a stream of text in Well-known Text format. The next tokens must form a MultiLineString Text</param>
        /// <returns>a <see cref="MultiLineString"/> specified by the next token in the stream</returns>
        private static MultiLineString ReadMultiLineStringText(WktStreamTokenizer tokenizer)
        {
            var arrlines = new List<ILineString>();

            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken == "EMPTY")
                return new MultiLineString(arrlines.ToArray());

            arrlines.Add(ReadLineStringText(tokenizer));
            nextToken = GetNextCloserOrComma(tokenizer);
            while (nextToken == ",")
            {
                arrlines.Add(ReadLineStringText(tokenizer));
                nextToken = GetNextCloserOrComma(tokenizer);
            }

            MultiLineString lines = new MultiLineString(arrlines.ToArray());
            return lines;
        }

        /// <summary>
        /// Creates a LineString using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer">Tokenizer over a stream of text in Well-known Text format.  The next
        /// tokens must form a LineString Text.</param>
        /// <returns>Returns a LineString specified by the next token in the stream.</returns>
        /// <remarks>
        /// ParseException is thrown if an unexpected token is encountered.
        /// </remarks>
        private static LineString ReadLineStringText(WktStreamTokenizer tokenizer)
        {
            var points = GetCoordinates(tokenizer);
            var arrcoordinate = new Coordinate[points.Count];
            for(int i=0; i<arrcoordinate.Length; i++)
            {
                arrcoordinate[i] = new Coordinate(points[i].X, points[i].Y);
            }

            return new LineString(arrcoordinate);
        }

        /// <summary>
        /// Creates a <see cref="GeometryCollection"/> using the next token in the stream.
        /// </summary>
        /// <param name="tokenizer"> Tokenizer over a stream of text in Well-known Text
        /// format. The next tokens must form a GeometryCollection Text.</param>
        /// <returns>
        /// A <see cref="GeometryCollection"/> specified by the next token in the stream.</returns>
        private static GeometryCollection ReadGeometryCollectionText(WktStreamTokenizer tokenizer)
        {
            var arrgeometries = new List<IGeometry>();

            string nextToken = GetNextEmptyOrOpener(tokenizer);
            if (nextToken.Equals("EMPTY"))
                return new GeometryCollection(arrgeometries.ToArray());

            arrgeometries.Add(ReadGeometryTaggedText(tokenizer));
            nextToken = GetNextCloserOrComma(tokenizer);
            while (nextToken.Equals(","))
            {
                arrgeometries.Add(ReadGeometryTaggedText(tokenizer));
                nextToken = GetNextCloserOrComma(tokenizer);
            }

            GeometryCollection geometries = new GeometryCollection(arrgeometries.ToArray());
            return geometries;
        }
    }
}