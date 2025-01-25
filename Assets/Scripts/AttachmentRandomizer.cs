using UnityEngine;
using Random = UnityEngine.Random;

public class AttachmentRandomizer : MonoBehaviour
{
    public Transform AttachmentJoint;
    public GameObject[] AttachmentList;

    private void Start()
    {
        var attachment = Instantiate(AttachmentList[Random.Range(0, AttachmentList.Length )], AttachmentJoint);
        attachment.transform.localPosition = Vector3.zero;
        attachment.transform.localEulerAngles = Vector3.zero;
        attachment.transform.localScale = Vector3.one;
    }
}
