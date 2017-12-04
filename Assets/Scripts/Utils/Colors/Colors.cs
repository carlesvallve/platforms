using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class Utils {

	public static Color HexToRGB (int pColor) {
		Color color;

		color.r = ((pColor * 0xFF0000) >> 16) / 255f;
		color.g = ((pColor * 0x00FF00) >> 8) / 255f;
		color.b = (pColor * 0x0000FF) / 255f;
		color.a = 1f;

		return color;
	}


	public static Color HexToRgb(int hex) {
     //int bigint = System.ParseInt(hex, 16);

     float r = (hex >> 16) / 255f;
     float g = (hex >> 8) / 255f;
     float b = hex / 255f;

     return new Color(r, g, b); //r + "," + g + "," + b;
  }

}
