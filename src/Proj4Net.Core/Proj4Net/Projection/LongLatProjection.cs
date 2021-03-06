/*
Copyright 2006 Jerry Huxtable

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;

namespace Proj4Net.Core.Projection
{

    ///<summary>
    /// A "projection" for geodetic coordinates in Decimal Degrees.
    /// </summary>
    public class LongLatProjection : Projection
    {
        // TODO: implement projection methods (which are basically just no-ops)
        /*
        public Point2D.Double transformRadians( Point2D.Double src, Point2D.Double dst ) {
            dst.x = src.x;
            dst.y = src.y;
            return dst;
        }
        */

        public override String ToString()
        {
            return "LongLat";
        }

        public override void Initialize()
        {
            base.Initialize();
            // units are always in Decimal Degrees
            Unit = Units.Units.Degrees;
            _totalScale = 1.0;
        }

    }
}