using UnityEngine;

public static class YakuChecker
{
    public static int CheckYaku(PlayerController player)
    {
        int points = 0;

        if (player.kasu.Count >= 10)
        {
            points += 1 + (player.kasu.Count - 10);
        }

        if (player.tanzaku.Count >= 5)
        {
            points += 1 + (player.tanzaku.Count - 5);
        }

        if (player.tane.Count >= 5)
        {
            points += 1 + (player.tane.Count - 5);
        }

        if (player.hikari.Count >= 3)
        {
            points += 5;
        }

        return points;
    }
}