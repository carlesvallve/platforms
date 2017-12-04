using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;


public static class Strings {

	public static string UppercaseFirst (string s) {
		if (string.IsNullOrEmpty(s)) { return string.Empty; }
		return char.ToUpper(s[0]) + s.Substring(1);
	}


	public static string ListToString<T>(this IList<T> ts) {
		string str ="[";
		for (int i = 0; i < ts.Count; i++) {
			str += ts[i];
			if (i < ts.Count -1) { str += ", "; };
		}
		str += "]";

		return str;
	}
}
