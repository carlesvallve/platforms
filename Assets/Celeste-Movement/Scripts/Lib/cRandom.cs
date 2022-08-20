using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarlesModules {

  public class cRandom {
    // =====================================================================

    // public static T PickRandom<T>(this IEnumerable<T> lst, System.Func<T, float> weightPredicate, IRandom rng = null) {
    //   var arr = (lst is IList<T>) ? lst as IList<T> : lst.ToList();
    //   if (arr.Count == 0) return default(T);
    //   var weights = (from o in lst select weightPredicate(o)).ToArray();
    //   var total = weights.Sum();
    //   if (total <= 0) return arr[0];

    //   if (rng == null) rng = RandomUtil.Standard;
    //   float r = rng.Next();
    //   float s = 0f;

    //   int i;
    //   for (i = 0; i < weights.Length; i++) {
    //     s += weights[i] / total;
    //     if (s >= r) {
    //       return arr[i];
    //     }
    //   }

    //   //should only get here if last element had a zero weight, and the r was large
    //   i = arr.Count - 1;
    //   while (i > 0 || weights[i] <= 0f) i--;
    //   return arr[i];
    // }

    // Case Example

    // generate a random value between 0-10.
    // But, can I somehow generate random values but with specific probability?
    // 50 % chance for 0-5 to be generated, 
    // 30% of 6-8 to be generated 
    // 20% chance of 9-10 to be generated?

    // IntRange[] ranges = new IntRange[] { new IntRange(0, 6, 50f),new IntRange(6, 9, 30f),new IntRange(9, 11, 20f) };
    // int index = cRandom.RandomRange.Range(ranges);

    // =====================================================================

    public struct IntRange {
      public int Min;
      public int Max;
      public float Weight;

      public IntRange(int _min, int _max, float _weight) {
        Min = _min;
        Max = _max;
        Weight = _weight;
      }
    }

    public struct FloatRange {
      public float Min;
      public float Max;
      public float Weight;

      public FloatRange(int _min, int _max, float _weight) {
        Min = _min;
        Max = _max;
        Weight = _weight;
      }
    }

    public static class RandomRange {

      public static int Range(IntRange[] ranges) {
        if (ranges.Length == 0) throw new System.ArgumentException("At least one range must be included.");
        if (ranges.Length == 1) return Random.Range(ranges[0].Max, ranges[0].Min);

        float total = 0f;
        for (int i = 0; i < ranges.Length; i++) total += ranges[i].Weight;

        float r = Random.value;
        float s = 0f;

        int cnt = ranges.Length - 1;
        for (int i = 0; i < cnt; i++) {
          s += ranges[i].Weight / total;
          if (s >= r) {
            return Random.Range(ranges[i].Max, ranges[i].Min);
          }
        }

        return Random.Range(ranges[cnt].Max, ranges[cnt].Min);
      }

      public static float Range(FloatRange[] ranges) {
        if (ranges.Length == 0) throw new System.ArgumentException("At least one range must be included.");
        if (ranges.Length == 1) return Random.Range(ranges[0].Max, ranges[0].Min);

        float total = 0f;
        for (int i = 0; i < ranges.Length; i++) total += ranges[i].Weight;

        float r = Random.value;
        float s = 0f;

        int cnt = ranges.Length - 1;
        for (int i = 0; i < cnt; i++) {
          s += ranges[i].Weight / total;
          if (s >= r) {
            return Random.Range(ranges[i].Max, ranges[i].Min);
          }
        }

        return Random.Range(ranges[cnt].Max, ranges[cnt].Min);
      }

    }

    // =====================================================================

    public static Vector3 RandomPointInAnnulus3D(Vector3 origin, float minRadius, float maxRadius) {
      Vector2 p = cRandom.RandomPointInAnnulus2D(new Vector2(origin.x, origin.z), minRadius, maxRadius);
      return new Vector3(p.x, origin.y, p.y);
    }

    public static Vector2 RandomPointInAnnulus2D(Vector2 origin, float minRadius, float maxRadius) {
      Vector2 randomDirection = (Random.insideUnitCircle * origin).normalized;
      float randomDistance = Random.Range(minRadius, maxRadius);
      Vector2 point = origin + randomDirection * randomDistance;
      return point;
    }

    public static Quaternion GetRandomRotation(Vector3 axis) {
      return Quaternion.Euler(
        Random.Range(0f, 360f) * axis.x,
        Random.Range(0f, 360f) * axis.y,
        Random.Range(0f, 360f) * axis.z
      );
    }

    // =====================================================================

  }
}
