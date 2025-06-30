using System;
using UnityEngine;

namespace MyUtilities
{
    public static class RandomUtilities 
    {
        /// <summary>
        /// Возвращает случайный элемент с учетом весов
        /// </summary>
        /// <param name="weightedItems">Пары (элемент, вес)</param>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <returns>Случайно выбранный элемент</returns>
        public static T WeightedChoice<T>(params (T item, float weight)[] weightedItems)
        {
            if (weightedItems == null || weightedItems.Length == 0)
                throw new ArgumentException("Items collection cannot be null or empty");

            // Рассчет общего веса
            float totalWeight = 0f;
            foreach (var (_, weight) in weightedItems)
            {
                if (weight < 0)
                    throw new ArgumentException("Weight cannot be negative");
                totalWeight += weight;
            }

            if (totalWeight <= 0)
                throw new InvalidOperationException("Total weight must be positive");

            // Генерация случайной точки
            float randomPoint = UnityEngine.Random.Range(0f, totalWeight);

            // Поиск соответствующего элемента
            foreach (var (item, weight) in weightedItems)
            {
                if (randomPoint < weight)
                    return item;

                randomPoint -= weight;
            }

            // Фолбэк (должно сработать только при ошибках округления)
            return weightedItems[weightedItems.Length - 1].item;
        }

        /// <summary>
        /// Возвращает случайный индекс с учетом весов
        /// </summary>
        /// <param name="weights">Массив весов</param>
        /// <returns>Случайный индекс</returns>
        public static int WeightedIndex(float[] weights)
        {
            if (weights == null || weights.Length == 0)
                throw new ArgumentException("Weights array cannot be null or empty");

            float totalWeight = 0f;
            foreach (float w in weights)
            {
                if (w < 0) throw new ArgumentException("Weight cannot be negative");
                totalWeight += w;
            }

            if (totalWeight <= 0)
                throw new InvalidOperationException("Total weight must be positive");

            float randomPoint = UnityEngine.Random.Range(0f, totalWeight);

            for (int i = 0; i < weights.Length; i++)
            {
                if (randomPoint < weights[i])
                    return i;

                randomPoint -= weights[i];
            }

            return weights.Length - 1;
        }
    }
}
