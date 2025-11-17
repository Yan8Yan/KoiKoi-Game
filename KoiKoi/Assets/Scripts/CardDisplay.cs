using UnityEngine;
using KoiKoiProject;

public class CardDisplay3D : MonoBehaviour
{
    [SerializeField] private MeshRenderer frontFaceRenderer;
    [SerializeField] private MeshRenderer backFaceRenderer;
    [SerializeField] private Card cardData;

    [SerializeField] private Material cardBackMaterial;
    [SerializeField] private Material cardFaceMaterial;

    private bool isFaceUp = false;

    public void SetCard(Card card)
    {
        cardData = card;

        Material faceMat = new Material(cardFaceMaterial);
        faceMat.mainTexture = card.cardSprite.texture;
        frontFaceRenderer.material = faceMat;
        backFaceRenderer.material = cardBackMaterial;
    }


    public void Flip(bool faceUp)
    {
        isFaceUp = faceUp;

        frontFaceRenderer.gameObject.SetActive(faceUp);
        backFaceRenderer.gameObject.SetActive(!faceUp);
    }

    public Card GetCardData() => cardData;
}
