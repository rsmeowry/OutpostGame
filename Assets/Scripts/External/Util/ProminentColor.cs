/* 
 * FileName: ProminentColor.cs
 * Author:   Murillo Pugliesi
 * GitHub:   https://github.com/Mukarillo
 */

// Licensed under the MIT license
// Modified by me

 using System;
 using System.Collections.Generic;
 using System.Linq;
 using UnityEngine;

 namespace External.Util {
    public class ColorAmount {
        public Color32 Color;
        public int Amount;

        public ColorAmount(Color32 color, int amount) {
            Color = color;
            Amount = amount;
        }
    }
    
    public static class ProminentColor {
        private static readonly List<Color32> ColorList = new();
        private static List<ColorAmount> _pixelColorAmount = new();
        
        /// <summary>
        /// Gets a list of the main colors from image.
        /// </summary>
        /// <returns>Returns a list of Color32 elements, with size lower or equal maxColorAmount.</returns>
        /// <param name="texture">Texture to get colors.</param>
        /// <param name="maxColorAmount">Max color amount.</param>
        /// <param name="colorLimiterPercentage">Value used to compare color amounts. If color amount percentage is less than this value, it will not be marked as valid color.</param>
        /// <param name="toleranceUniteColors">Value used to compare colors and unite them if is a match, useful to discard gradients.</param>
        /// <param name="minimiumColorPercentage">Value used to remove colors that have lower amount percentage compared with full image.</param>
        public static List<Color32> GetColors32FromImage(Texture2D texture, int maxColorAmount, float colorLimiterPercentage, int toleranceUniteColors, float minimiumColorPercentage) {
            if (texture == null) throw new Exception("Texture null");
            if (maxColorAmount <= 0) throw new Exception("maxColorAmount must be higher than 0");

            ColorList.Clear();
            _pixelColorAmount.Clear();

            var pixels = texture.GetPixels32();

            for (int i = 0; i < pixels.Length; i += 1) {
                var px = pixels[i];
                if (px.a < 255) continue;
                var c = _pixelColorAmount.Find(x => x.Color.Equals(px));
                if (c == null)
                    _pixelColorAmount.Add(new ColorAmount(px, 1));
                else
                    c.Amount++;
            }

            if (_pixelColorAmount.Count <= 0) return null;

            _pixelColorAmount = UniteSimilarColors(_pixelColorAmount, toleranceUniteColors);

            _pixelColorAmount = _pixelColorAmount.OrderByDescending(x => x.Amount).ToList();

            var totalAmount = _pixelColorAmount.Sum(x => x.Amount);

            var lastAmount = _pixelColorAmount[0].Amount;
            ColorList.Add(_pixelColorAmount[0].Color);
            _pixelColorAmount.RemoveAt(0);

            for (int i = 0; i < _pixelColorAmount.Count; i++) {
                if (_pixelColorAmount.Count <= i || ColorList.Count >= maxColorAmount || _pixelColorAmount[i].Amount < totalAmount / minimiumColorPercentage) continue;

                if ((float) _pixelColorAmount[i].Amount / lastAmount * 100f > (i == 0 ? 5f : colorLimiterPercentage)) {
                    ColorList.Add(_pixelColorAmount[i].Color);
                    lastAmount = _pixelColorAmount[i].Amount;
                }
            }

            return ColorList;
        }

        /// <summary>
        /// Gets a list of the main colors from image.
        /// </summary>
        /// <returns>Returns a list of strings (Hexadecimal colors) elements, with size lower or equal maxColorAmount.</returns>
        /// <param name="texture">Texture to get colors.</param>
        /// <param name="maxColorAmount">Max color amount.</param>
        /// <param name="colorLimiterPercentage">Value used to compare color amounts. If color amount percentage is less than this value, it will not be marked as valid color.</param>
        /// <param name="toleranceUniteColors">Value used to compare colors and unite them if is a match, useful to discard gradients.</param>
        /// <param name="minimiumColorPercentage">Value used to remove colors that have lower amount percentage compared with full image.</param>
        public static List<string> GetHexColorsFromImage(Texture2D texture, int maxColorAmount, float colorLimiterPercentage, int toleranceUniteColors, float minimiumColorPercentage) {
            var color32List = GetColors32FromImage(texture, maxColorAmount, colorLimiterPercentage, toleranceUniteColors, minimiumColorPercentage);
            if (color32List == null) return null;

            List<string> hexColors = new List<string>();
            color32List.ForEach(x => hexColors.Add(ColorToHex(x)));

            return hexColors;
        }

        private static List<ColorAmount> UniteSimilarColors(List<ColorAmount> colorAmounts, int tolerance = 30, bool replaceSimilarColors = false) {
            List<ColorAmount> toReturn = new List<ColorAmount>();

            bool found;
            foreach (var t1 in colorAmounts)
            {
                found = false;
                foreach (var t in toReturn)
                {
                    if (ColorTest(t1.Color, t.Color, tolerance)) {
                        if (replaceSimilarColors) {
                            if (GetColorSaturation(t.Color) < GetColorSaturation(t1.Color)) t.Color = t1.Color;
                        }

                        t.Amount += t1.Amount;
                        found = true;
                    }
                }

                if (!found) toReturn.Add(t1);
            }

            return toReturn;
        }

        private static bool ColorTest(Color32 c1, Color32 c2, float tol) {
            float diffRed = Mathf.Abs(c1.r - c2.r);
            float diffGreen = Mathf.Abs(c1.g - c2.g);
            float diffBlue = Mathf.Abs(c1.b - c2.b);

            float diffPercentage = ((diffRed / 255f) + (diffGreen / 255f) + (diffBlue / 255f)) / 3 * 100;

            return diffPercentage < tol;
        }

        private static string ColorToHex(Color32 color) {
            return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }

        private static double GetColorSaturation(Color32 color) {
            int max = Math.Max(color.r, Math.Max(color.g, color.b));
            int min = Math.Min(color.r, Math.Min(color.g, color.b));
            return (max == 0) ? 0 : 1d - (1d * min / max);
        }
    }
}