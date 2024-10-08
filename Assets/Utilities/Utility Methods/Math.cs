using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UtilityMethods
{
    public static class Math
    {

        /// <summary>
        /// Takes a value from a given input range and maps it to a new value within a desired output range
        /// </summary>
        /// <param name="value">Current input value</param>
        /// <param name="inputMinValue">Minimum value in input range</param>
        /// <param name="inputMaxValue">Maximum value in input range</param>
        /// <param name="outputMinValue">Minimum value in output range</param>
        /// <param name="outputMaxValue">Maximum value in output range</param>
        /// <returns></returns>
        public static float Remap(this float value,
                                  float inputMinValue, float inputMaxValue,
                                  float outputMinValue, float outputMaxValue)
        {
            return (value - inputMinValue) / (inputMaxValue - inputMinValue) * (outputMaxValue - outputMinValue) + outputMinValue;
        }

        public static Vector2 PolarToCartesianClockwise(float radius, float angle)
        {
            return PolarToCartesianCounterclockwise(radius, -angle);
        }
        public static Vector2 PolarToCartesianCounterclockwise(float radius, float angle)
        {
            angle = angle * Mathf.Deg2Rad;
            Vector2 returnVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return returnVector * radius;
        }

        public static Vector3 RandomPointOnPlane(Vector3 position, Vector3 normal, float radius)
        {
            Vector3 randomPoint;

            do
            {
                randomPoint = Vector3.Cross(Random.insideUnitSphere, normal);
            } while (randomPoint == Vector3.zero);

            randomPoint.Normalize();
            randomPoint *= radius;
            randomPoint += position;

            return randomPoint;
        }

        public static bool ProbabilityCheck(int successPercentage)
        {
            int randomNumber = Random.Range(0, 101);
            return randomNumber <= successPercentage;
        }

        public static bool ProbabilityCheck(int successThreshold, int totalChance)
        {
            int randomNumber = Random.Range(0, totalChance + 1);
            return randomNumber <= successThreshold;
        }
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }

}