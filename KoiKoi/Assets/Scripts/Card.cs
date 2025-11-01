using UnityEngine;


namespace KoiKoiProject
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")] //это создает нам возможность делать карты по шаблону прямо в юнити
    public class Card : ScriptableObject
    {
        public string cardName;
        public CardType cardType;
        public Month month;
        public enum CardType
        {
            hikari, // благородные
            tane, // животные
            tanzaku, // ленточки
            kasu // простые
        }

        public enum Month
        {
            January,   // сосна
            February,  // слива
            March,     // сакура
            April,     // глициния
            May,       // ирис
            June,      // пион
            July,      // клевер
            August,    // сусуки
            September, // хризантема
            October,   // клён
            November,  // ива
            December   // павлония
        }
    }

}
