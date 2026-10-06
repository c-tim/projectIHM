using UnityEngine;

public class placeHolderCollectibles : MonoBehaviour
{

    [SerializeField]
    float cooldownRechargeCollectible;

    float timeSpentAfterUsedCollectible;

    // public delegate void simpleEventWithThis(placeHolderCollectibles obj);
    //public event simpleEventWithThis onReadyForNewCollectibles;

    protected Collectible heldCollectible;

    [SerializeField]
    private Collectible prefabCollectible;
    bool collectiveAcrivated;


    /*bool hasCollectible()
    {
        return heldCollectible != null;
    }*/
    private void Start()
    {
        generateCollectibleFromPrefab();
        spawnCollectibleAgain();
    }
    private void Update()
    {
        if (collectiveAcrivated)
        {
            return;
        }

        timeSpentAfterUsedCollectible += Time.deltaTime;
        if (timeSpentAfterUsedCollectible > cooldownRechargeCollectible)
        {
            // onReadyForNewCollectibles(this);
            spawnCollectibleAgain();

        }
    }

    public void spawnCollectibleAgain()
    {
        heldCollectible.transform.position = transform.position;
    }

    private void generateCollectibleFromPrefab()
    {
        heldCollectible = Instantiate(prefabCollectible);

    }

}