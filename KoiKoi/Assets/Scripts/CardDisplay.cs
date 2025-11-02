using UnityEngine;
using KoiKoiProject;

public class CardDisplay3D : MonoBehaviour
{
    [SerializeField] private MeshRenderer frontFaceRenderer;
    [SerializeField] private MeshRenderer backFaceRenderer;

    [SerializeField] private Material cardBackMaterial;
    [SerializeField] private Material cardFaceMaterial;

    private Card cardData;
    private bool isFaceUp = false;

    public void SetCard(Card card)
    {
        cardData = card;

        Material faceMat = new Material(cardFaceMaterial); //Мы создаём новый экземпляр материала на основе шаблона cardFaceMaterial.
        faceMat.mainTexture = card.cardSprite.texture; //Здесь мы устанавливаем текстуру для "лица" карты.

        frontFaceRenderer.material = faceMat; //Это рендерер передней стороны карты. Мы присваиваем ему уникальный материал с нужной текстурой.
        backFaceRenderer.material = cardBackMaterial; //Здесь используем общий материал для всех карт, так как задняя сторона обычно одинаковая и менять её не нужно.
    }

    public void Flip(bool faceUp)
    {
        isFaceUp = faceUp;

        frontFaceRenderer.gameObject.SetActive(faceUp);
        backFaceRenderer.gameObject.SetActive(!faceUp);
    }

    public Card GetCardData() => cardData;
}
