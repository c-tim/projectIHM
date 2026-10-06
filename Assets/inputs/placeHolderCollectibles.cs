using UnityEngine;

public class placeHolderCollectibles : MonoBehaviour
{

    [SerializeField]
    float cooldownRechargeCollectible;

    float timeSpentAfterUsedCollectible;

    public delegate void simpleEventWithThis(placeHolderCollectibles obj);
    public event simpleEventWithThis onReadyForNewCollectibles;

    protected Collectible heldCollectible;
    

    bool hasCollectible()
    {
        return heldCollectible != null;
    }

    private void Update()
    {
        if (hasCollectible())
        {
            return;
        }

        timeSpentAfterUsedCollectible += Time.deltaTime;
        if (timeSpentAfterUsedCollectible > cooldownRechargeCollectible)
        {
            onReadyForNewCollectibles(this);

        }
    }

}
