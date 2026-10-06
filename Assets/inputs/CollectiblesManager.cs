using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    List<Collectible> listeCollectibles;
    List<Collectible> availableCollectibles;

    Collectible prefabCollectible;


    [SerializeField]
    int numberGeneratedObject;



    private void Start()
    {
        listeCollectibles = new List<Collectible>();
        availableCollectibles = listeCollectibles;

        for (int i = 0; i < listeCollectibles.Count; ++i)
        {
            listeCollectibles.Add(generateCollectible());
        }
    }

    public Collectible getFreeCollectible()
    {
        if (availableCollectibles.Count > 0)
        {
            Collectible newCollectible = availableCollectibles[0];
            availableCollectibles.RemoveAt(0);
            return newCollectible;
        }
        return generateCollectible();
    }

    public void recycleCollectible(Collectible obj)
    {
        availableCollectibles.Add(obj);
    }

    private Collectible generateCollectible() { 
        Collectible newCollectible = Instantiate(prefabCollectible);
        listeCollectibles.Add(newCollectible);
        return newCollectible;

    }




}
