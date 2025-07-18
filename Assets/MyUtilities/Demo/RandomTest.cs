using MyUtilities;
using UnityEngine;


public class RandomTest : MonoBehaviour
{
    [System.Serializable]
    public struct WeightedColor
    {
        public Color color;
        [Range(0.1f, 100)] public float weight;
    }

    public WeightedColor[] colors;

    void Start()
    {
        // Тест распределения
        int[] counts = new int[colors.Length];
        int trials = 10000;

        for (int i = 0; i < trials; i++)
        {
            int index = RandomUtilities.WeightedIndex(GetWeights());
            counts[index]++;
        }

        // Вывод результатов
        for (int i = 0; i < colors.Length; i++)
        {
            float percentage = (float)counts[i] / trials * 100;
            DebugUtils.LogEditor($"{colors[i].color}: {percentage:F1}% (expected: {colors[i].weight / TotalWeight() * 100:F1}%)");
        }
    }

    private float[] GetWeights()
    {
        float[] weights = new float[colors.Length];
        for (int i = 0; i < colors.Length; i++)
            weights[i] = colors[i].weight;
        return weights;
    }

    private float TotalWeight()
    {
        float total = 0;
        foreach (var c in colors) total += c.weight;
        return total;
    }
}
