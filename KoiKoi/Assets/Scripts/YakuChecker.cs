using UnityEngine;

public static class YakuChecker
{
    public static int CheckYaku(PlayerController player)
    {
        int points = 0;

        //неправильные значения потом поменять
        if (player.kasu.Count >= 2)
        {
            points += 1 + (player.kasu.Count - 2);
        }

        if (player.tanzaku.Count >= 2)
        {
            points += 1 + (player.tanzaku.Count - 2);
        }

        if (player.tane.Count >= 2)
        {
            points += 1 + (player.tane.Count - 2);
        }

        if (player.hikari.Count >= 2)
        {
            points += 5;
        }

        return points;
    }
}