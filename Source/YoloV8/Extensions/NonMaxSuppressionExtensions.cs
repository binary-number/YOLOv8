﻿namespace Compunet.YoloV8.Extensions;

public static class NonMaxSuppressionExtensions
{
    public static IReadOnlyList<T> NonMaxSuppression<T>(this IReadOnlyList<T> boxes,
                                                        Func<T, Rectangle> rectangleSelector,
                                                        Func<T, float> confidenceSelector,
                                                        float threshold)
    {
        var sorted = boxes.OrderByDescending(confidenceSelector).ToArray();
        var count = sorted.Length;

        var activeCount = count;
        var isActiveBoxes = Enumerable.Repeat(true, count).ToArray();

        var suppresed = new List<T>();

        for (int i = 0; i < count; i++)
        {
            if (isActiveBoxes[i])
            {
                var boxA = sorted[i];

                suppresed.Add(boxA);

                for (var j = i + 1; j < count; j++)
                {
                    if (isActiveBoxes[j])
                    {
                        var boxB = sorted[j];

                        if (CalculateIoU(rectangleSelector(boxA), rectangleSelector(boxB)) > threshold)
                        {
                            isActiveBoxes[j] = false;
                            activeCount--;

                            if (activeCount <= 0)
                                break;
                        }
                    }
                }

                if (activeCount <= 0)
                    break;
            }
        }

        return suppresed;
    }

    private static float CalculateIoU(Rectangle first, Rectangle second)
    {
        var areaA = Area(first);

        if (areaA <= 0f)
            return 0f;

        var areaB = Area(second);

        if (areaB <= 0f)
            return 0f;

        var intersection = Rectangle.Intersect(first, second);
        var intersectionArea = Area(intersection);

        return (float)intersectionArea / (areaA + areaB - intersectionArea);
    }

    private static int Area(Rectangle rectangle)
    {
        return rectangle.Width * rectangle.Height;
    }
}